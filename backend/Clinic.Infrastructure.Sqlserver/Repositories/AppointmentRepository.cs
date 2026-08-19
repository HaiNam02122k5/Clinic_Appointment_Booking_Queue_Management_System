using Clinic.Application.Common.Models;
using Clinic.Application.Contracts;
using Clinic.Application.Interfaces;
using Clinic.Domain.Entities;
using Clinic.Domain.Enums;
using Clinic.Infrastructure.Sqlserver.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Infrastructure.Sqlserver.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly ApplicationDbContext _context;
        public AppointmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Appointment>> GetAppointmentsByPatientIdAsync(Guid patientId, string category)
        {
            var query = _context.Appointments
                .Include(a => a.WorkSchedule)
                    .ThenInclude(ws => ws.Doctor)
                        .ThenInclude(d => d.Employee)
                            .ThenInclude(e => e.Person)
                .Include(a => a.Patient)
                    .ThenInclude(p => p.Person)
                .Where(a => a.IsDeleted == false && a.PatientId == patientId);

            // Apply category filter if provided
            query = category.ToLower() switch
            {
                "upcoming" => query.Where(a => new[] { AppointmentStatus.Pending, AppointmentStatus.Confirmed, AppointmentStatus.CheckedIn }.Contains(a.Status)),
                "completed" => query.Where(a => a.Status == AppointmentStatus.Completed),
                "cancelled" => query.Where(a => a.Status == AppointmentStatus.Cancelled),
                _ => query
            };

            var count = await query.CountAsync();
            var items = await query.OrderByDescending(a => a.WorkSchedule.Date).ThenByDescending(a => a.TimeSlot).AsNoTracking().ToListAsync();

            return new PagedResult<Appointment>
            (
                items: items,
                totalCount: count
            );
        }

        public async Task<Appointment?> GetByIdAsync(Guid appointmentId)
        {
            return await _context.Appointments.Include(a => a.WorkSchedule).ThenInclude(ws => ws.Doctor).ThenInclude(d => d.Employee).ThenInclude(e => e.Person)
                .Include(a => a.Patient).ThenInclude(p => p.Person)
                .FirstOrDefaultAsync(a => a.Id == appointmentId && a.IsDeleted == false);
        }

        public Task<bool> IsTimeSlotTakenAsync(Guid doctorId, DateTime timeSlot)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ExistsForDoctorAndPatientAsync(Guid doctorId, Guid patientId)
        {
            return await _context.Appointments
                .AnyAsync(a => a.PatientId == patientId && a.WorkSchedule.DoctorId == doctorId);
        }

        public async Task<TotalAppointmentSummaryDto> GetTotalAppointmentSummaryAsync(DateOnly startDate, DateOnly endDate, Guid doctorId, Guid specialtyId)
        {
            var query = _context.Appointments
                .Include(a => a.WorkSchedule)
                    .ThenInclude(ws => ws.Doctor)
                        .ThenInclude(d => d.WorkHistories.Where(wh => wh.IsDeleted == false && wh.EndDate == null))
                            .ThenInclude(d => d.Specialty)
                .Include(a => a.QueueTicket)
                .Where(a => a.IsDeleted == false && a.WorkSchedule.Date >= startDate && a.WorkSchedule.Date <= endDate);

            if (doctorId != Guid.Empty)
            {
                query = query.Where(a => a.WorkSchedule.DoctorId == doctorId);
            }
            else if (specialtyId != Guid.Empty)
            {
                query = query.Where(a => a.WorkSchedule.Doctor.WorkHistories.Any(wh => wh.SpecialtyId == specialtyId));
            }

            var result = await query.GroupBy(a => 1).Select(g => new TotalAppointmentSummaryDto
                {
                    AppointmentCount = g.Count(),
                    AppointmentOnlineCount = g.Count(a => a.IsWalkIn == false),
                    CompletedAppointments = g.Count(a => a.Status == AppointmentStatus.Completed),
                    CanceledAppointments = g.Count(a => a.Status == AppointmentStatus.Cancelled),
                    NoShowAppointments = g.Count(a => a.Status == AppointmentStatus.NoShow),
                    CancellationRate = g.Count(a => a.IsWalkIn == false) == 0 ? 0 : (double)g.Count(a => a.Status == AppointmentStatus.Cancelled && a.IsWalkIn == false) / g.Count(a => a.IsWalkIn == false),
                    AverageWaitingMinutes = g.Where(a => a.Status == AppointmentStatus.Completed).Average(a => EF.Functions.DateDiffMinute(a.QueueTicket.CheckInTime, a.QueueTicket.CalledAt)) ?? 0
                }).FirstOrDefaultAsync();

            return result;
        }
    }
}

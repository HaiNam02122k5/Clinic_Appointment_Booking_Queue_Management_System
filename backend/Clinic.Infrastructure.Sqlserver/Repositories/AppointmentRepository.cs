using Clinic.Application.Common.Models;
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

        public async Task<PagedResult<Appointment>> GetAppointmentsByPatientIdAsync(Guid PatientId, string category)
        {
            var query = _context.Appointments.Where(a => a.IsDeleted == false && a.PatientId == PatientId);

            // Apply category filter if provided
            query = category.ToLower() switch
            {
                "upcoming" => query.Where(a => new[] { AppointmentStatus.Pending, AppointmentStatus.Confirmed, AppointmentStatus.CheckedIn }.Contains(a.Status)),
                "completed" => query.Where(a => a.Status == AppointmentStatus.Completed),
                "cancelled" => query.Where(a => a.Status == AppointmentStatus.Cancelled),
                _ => query
            };

            var count = await query.CountAsync();
            var items = await query.AsNoTracking().ToListAsync();

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
    }
}

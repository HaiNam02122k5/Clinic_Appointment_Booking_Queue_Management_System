using Clinic.Application.Interfaces;
using Clinic.Domain.Entities;
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

        public async Task<Appointment?> GetByIdAsync(Guid id)
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.WorkSchedule)
                // Bắt buộc load QueueTicket: Appointment.Cancel() cần biết trạng thái vé hàng đợi
                // hiện tại (Waiting/Called/InProgress/...) để quyết định có được hủy hay không,
                // và để cascade hủy vé khi hợp lệ. Thiếu Include này là nguyên nhân gốc khiến
                // Appointment chuyển Cancelled trong khi QueueTicket vẫn "sống" (bug hàng đợi ảo).
                .Include(a => a.QueueTicket)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task AddAsync(Appointment appointment)
        {
            await _context.Appointments.AddAsync(appointment);
        }

        public async Task UpdateAsync(Appointment appointment)
        {
            _context.Appointments.Update(appointment);
        }

        public async Task<List<Appointment>> GetByPatientIdAsync(Guid patientId)
        {
            return await _context.Appointments
                .Include(a => a.WorkSchedule)
                    .ThenInclude(w => w.Doctor)
                        .ThenInclude(d => d.Employee)
                            .ThenInclude(e => e.Person)
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.TimeSlot)
                .ToListAsync();
        }

        public async Task<bool> ExistsForDoctorAndPatientAsync(Guid doctorId, Guid patientId)
        {
            return await _context.Appointments
                .AnyAsync(a => a.PatientId == patientId && a.WorkSchedule.DoctorId == doctorId);
        }
    }
}
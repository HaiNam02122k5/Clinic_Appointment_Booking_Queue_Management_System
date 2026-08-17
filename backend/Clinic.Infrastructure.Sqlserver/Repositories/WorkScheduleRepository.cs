using Clinic.Application.Interfaces;
using Clinic.Domain.Entities;
using Clinic.Domain.Enums;
using Clinic.Infrastructure.Sqlserver.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Infrastructure.Sqlserver.Repositories
{
    public class WorkScheduleRepository : IWorkScheduleRepository
    {
        private readonly ApplicationDbContext _context;

        public WorkScheduleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<WorkSchedule?> GetByIdAsync(Guid id)
        {
            return await _context.WorkSchedules
                .Include(w => w.Appointments)
                .Include(w => w.Doctor)
                    .ThenInclude(d => d.Employee)
                        .ThenInclude(e => e.Person)
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<List<WorkSchedule>> GetAvailableSlotsAsync(Guid? doctorId, Guid? specialtyId, DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.WorkSchedules
                .Include(w => w.Appointments)
                .Include(w => w.Doctor)
                    .ThenInclude(d => d.Employee)
                        .ThenInclude(e => e.Person)
                .Where(w => w.Status == WorkScheduleStatus.Active && w.ShiftEnd > DateTime.UtcNow);

            if (doctorId.HasValue)
            {
                query = query.Where(w => w.DoctorId == doctorId.Value);
            }

            if (specialtyId.HasValue)
            {
                query = query.Where(w => w.Doctor.WorkHistories.Any(wh =>
                    wh.SpecialtyId == specialtyId.Value && wh.Status == WorkHistoryStatus.Active));
            }

            if (fromDate.HasValue)
            {
                query = query.Where(w => w.ShiftStart >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(w => w.ShiftStart <= toDate.Value);
            }

            // Chỉ lấy khung giờ còn chỗ trống (số lịch hẹn chưa hủy < giới hạn/khung giờ).
            query = query.Where(w => w.Appointments.Count(a => a.Status != AppointmentStatus.Cancelled) < w.PatientLimitPerSlot);

            return await query.OrderBy(w => w.ShiftStart).ToListAsync();
        }
    }
}
using Clinic.Application.Interfaces;
using Clinic.Domain.Entities;
using Clinic.Domain.Enums;
using Clinic.Infrastructure.Sqlserver.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Clinic.Infrastructure.Sqlserver.Repositories
{
    // Use for both WorkSchedule and ShiftRequest
    public class WorkScheduleRepository : IWorkScheduleRepository
    {
        private readonly ApplicationDbContext _context;

        public WorkScheduleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===== Nhóm chức năng: đặt lịch khám (booking) =====

        public async Task<WorkSchedule?> GetByIdAsync(Guid id)
        {
            return await _context.WorkSchedules
                .Include(w => w.Appointments)
                .Include(w => w.Doctor)
                    .ThenInclude(d => d.Employee)
                        .ThenInclude(e => e.Person)
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task LockAsync(Guid workScheduleId, CancellationToken cancellationToken = default)
        {
            // Khóa ghi (UPDLOCK, HOLDLOCK) đúng 1 dòng WorkSchedule, giữ tới khi transaction
            // hiện tại COMMIT/ROLLBACK. Request thứ 2 cũng gọi LockAsync cho cùng
            // workScheduleId sẽ bị chặn ngay tại câu SELECT này cho tới khi request thứ 1
            // xong, nên GetByIdAsync gọi ngay sau đó luôn thấy đúng số Appointments mới nhất -
            // không còn cửa sổ "đếm rồi mới insert" để 2 request cùng lọt qua như trước.
            // Cột output phải đặt tên "Value" vì SqlQuery<int> map theo quy ước này.
            await _context.Database
                .SqlQuery<int>($"SELECT 1 AS Value FROM WorkSchedules WITH (UPDLOCK, HOLDLOCK) WHERE Id = {workScheduleId}")
                .ToListAsync(cancellationToken);
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
                    wh.SpecialtyId == specialtyId.Value && wh.EndDate == null));
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

        // ===== Nhóm chức năng: quản lý ca trực bác sĩ (shift request) =====

        public async Task AddShiftRequestAsync(ShiftRequest shiftRequest)
        {
            await _context.ShiftRequests.AddAsync(shiftRequest);
        }

        public async Task AddWorkScheduleAsync(WorkSchedule workSchedule)
        {
            await _context.WorkSchedules.AddAsync(workSchedule);
        }

        public async Task<IEnumerable<WorkSchedule>> GetPlannedSchedulesByDoctorIdAsync(Guid? doctorId, DateOnly startDate, DateOnly endDate)
        {
            // Check if the time range exceeds 1 month
            if (startDate.AddMonths(1) < endDate)
            {
                throw new ArgumentException("The time range cannot exceed 1 month.");
            }

            return await _context.WorkSchedules
                .Where(ws => ws.DoctorId == doctorId && ws.IsDeleted == false && ws.Status != Domain.Enums.WorkScheduleStatus.Cancelled &&
                    ((DateOnly.FromDateTime(ws.ShiftStart) >= startDate && DateOnly.FromDateTime(ws.ShiftStart) <= endDate) ||
                    (DateOnly.FromDateTime(ws.ShiftEnd) >= startDate && DateOnly.FromDateTime(ws.ShiftEnd) <= endDate))
                ).AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<ShiftRequest>> GetRequestedSchedulesByDoctorIdAsync(Guid? doctorId, DateOnly startDate, DateOnly endDate)
        {
            // Check if the time range exceeds 1 month
            if (startDate.AddMonths(1) < endDate)
            {
                throw new ArgumentException("The time range cannot exceed 1 month.");
            }
            if (doctorId == null)
            {
                throw new ArgumentException("DoctorId cannot be null.");
            }

            return await _context.ShiftRequests
                .Where(sr => sr.DoctorId == doctorId && sr.IsDeleted == false && sr.Status != Domain.Enums.ShiftRequestStatus.Cancelled &&
                    ((DateOnly.FromDateTime(sr.ShiftStart) >= startDate && DateOnly.FromDateTime(sr.ShiftStart) <= endDate) ||
                    (DateOnly.FromDateTime(sr.ShiftEnd) >= startDate && DateOnly.FromDateTime(sr.ShiftEnd) <= endDate))
                ).AsNoTracking().ToListAsync();
        }

        public async Task<ShiftRequest?> GetShiftRequestByIdAsync(Guid scheduleId)
        {
            return await _context.ShiftRequests.Include(sr => sr.Doctor)
                .FirstOrDefaultAsync(sr => sr.Id == scheduleId && sr.IsDeleted == false);
        }

        public async Task<WorkSchedule?> GetWorkScheduleByIdAsync(Guid scheduleId)
        {
            return await _context.WorkSchedules.Include(ws => ws.Doctor).Include(ws => ws.Appointments)
                .FirstOrDefaultAsync(ws => ws.Id == scheduleId && ws.IsDeleted == false);
        }

        public async Task<bool> HasDuplicateShiftRequest(Guid doctorId, DateTime startTime, DateTime endTime, Guid? currentSRId = null)
        {
            var hasDuplicate = await _context.ShiftRequests
                .AnyAsync(sr => sr.DoctorId == doctorId && sr.IsDeleted == false && sr.Status != Domain.Enums.ShiftRequestStatus.Cancelled &&
                    sr.ShiftStart == startTime && sr.ShiftEnd == endTime && (currentSRId == null || sr.Id != currentSRId));
            return hasDuplicate;
        }

        public async Task<bool> HasOverlappingWorkSchedule(Guid doctorId, DateTime startTime, DateTime endTime, Guid? currentWSId = null)
        {
            var hasOverlapping = await _context.WorkSchedules
                .AnyAsync(ws => ws.DoctorId == doctorId && ws.IsDeleted == false && ws.Status != Domain.Enums.WorkScheduleStatus.Cancelled &&
                    ws.ShiftStart < endTime && ws.ShiftEnd > startTime && (currentWSId == null || ws.Id != currentWSId));
            return hasOverlapping;
        }
    }
}
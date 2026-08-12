using Clinic.Application.Interfaces;
using Clinic.Domain.Entities;
using Clinic.Infrastructure.Sqlserver.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

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

        public async Task AddShiftRequestAsync(ShiftRequest shiftRequest)
        {
            await _context.ShiftRequests.AddAsync(shiftRequest);
        }

        public async Task AddWorkScheduleAsync(WorkSchedule workSchedule)
        {
            await _context.WorkSchedules.AddAsync(workSchedule);
        }

        public async Task<IEnumerable<WorkSchedule>> GetPlannedSchedulesByDoctorIdAsync(Guid doctorId, DateOnly startDate, DateOnly endDate)
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

        public async Task<IEnumerable<ShiftRequest>> GetRequestedSchedulesByDoctorIdAsync(Guid doctorId, DateOnly startDate, DateOnly endDate)
        {
            // Check if the time range exceeds 1 month
            if (startDate.AddMonths(1) < endDate)
            {
                throw new ArgumentException("The time range cannot exceed 1 month.");
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

        public async Task<bool> HasDuplicateShiftRequest(Guid doctorId, DateTime startTime, DateTime endTime)
        {
            var hasDuplicate = await _context.ShiftRequests
                .AnyAsync(sr => sr.DoctorId == doctorId && sr.IsDeleted == false && sr.Status != Domain.Enums.ShiftRequestStatus.Cancelled &&
                    sr.ShiftStart == startTime && sr.ShiftEnd == endTime);
            return hasDuplicate;
        }

        public async Task<bool> HasOverlappingWorkSchedule(Guid doctorId, DateTime startTime, DateTime endTime)
        {
            var hasOverlapping = await _context.WorkSchedules
                .AnyAsync(ws => ws.DoctorId == doctorId && ws.IsDeleted == false && ws.Status != Domain.Enums.WorkScheduleStatus.Cancelled &&
                    ws.ShiftStart < endTime && ws.ShiftEnd > startTime);
            return hasOverlapping;
        }
    }
}

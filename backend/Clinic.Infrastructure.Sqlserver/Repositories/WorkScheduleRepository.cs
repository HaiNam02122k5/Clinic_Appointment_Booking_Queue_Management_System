using Clinic.Application.Interfaces;
using Clinic.Domain.Entities;
using Clinic.Domain.Enums;
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

        public async Task<IEnumerable<WorkSchedule>> GetDoctorSchedulesWithAppointmentByDateAsync(Guid doctorId, DateOnly date)
        {
            return await _context.WorkSchedules.Include(ws => ws.Appointments.Where(a => a.IsDeleted == false && a.Status != AppointmentStatus.Cancelled))
                .Where(ws =>
                    ws.DoctorId == doctorId &&
                    ws.IsDeleted == false &&
                    ws.Status == WorkScheduleStatus.Active &&
                    ws.Date == date)
                .AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<WorkSchedule>> GetPlannedSchedulesByDoctorIdAsync(Guid doctorId, DateOnly startDate, DateOnly endDate)
        {
            // Check if the time range exceeds 1 month
            if (startDate.AddMonths(1) < endDate)
            {
                throw new ArgumentException("The time range cannot exceed 1 month.");
            }

            return await _context.WorkSchedules
                .Where(ws =>
                    ws.DoctorId == doctorId &&
                    ws.IsDeleted == false &&
                    ws.Status != WorkScheduleStatus.Cancelled &&
                    ws.Date >= startDate &&
                    ws.Date <= endDate
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
                .Where(sr =>
                    sr.DoctorId == doctorId &&
                    sr.IsDeleted == false &&
                    sr.Status != ShiftRequestStatus.Cancelled &&
                    sr.Date >= startDate &&
                    sr.Date <= endDate
                ).AsNoTracking().ToListAsync();
        }

        public async Task<ShiftRequest?> GetShiftRequestByIdAsync(Guid scheduleId)
        {
            return await _context.ShiftRequests.Include(sr => sr.Doctor)
                .FirstOrDefaultAsync(sr => sr.Id == scheduleId && sr.IsDeleted == false);
        }

        public async Task<WorkSchedule?> GetWorkScheduleByIdAsync(Guid scheduleId)
        {
            return await _context.WorkSchedules.Include(ws => ws.Doctor).ThenInclude(d => d.Employee).ThenInclude(e => e.Person)
                .Include(ws => ws.Appointments.Where(a => a.IsDeleted == false && a.Status != AppointmentStatus.Cancelled))
                .FirstOrDefaultAsync(ws => ws.Id == scheduleId && ws.IsDeleted == false);
        }

        public async Task<bool> HasDuplicateShiftRequest(Guid doctorId, DateOnly date, TimeOnly startTime, TimeOnly endTime)
        {
            var hasDuplicate = await _context.ShiftRequests
                .AnyAsync(sr => sr.DoctorId == doctorId && sr.IsDeleted == false && sr.Status != Domain.Enums.ShiftRequestStatus.Cancelled &&
                    sr.Date == date && sr.ShiftStart == startTime && sr.ShiftEnd == endTime);
            return hasDuplicate;
        }

        public async Task<bool> HasOverlappingWorkSchedule(Guid doctorId, DateOnly date, TimeOnly startTime, TimeOnly endTime)
        {
            var hasOverlapping = await _context.WorkSchedules
                .AnyAsync(ws => ws.DoctorId == doctorId && ws.IsDeleted == false && ws.Status != Domain.Enums.WorkScheduleStatus.Cancelled &&
                    ws.Date == date && ws.ShiftStart < endTime && ws.ShiftEnd > startTime);
            return hasOverlapping;
        }
    }
}

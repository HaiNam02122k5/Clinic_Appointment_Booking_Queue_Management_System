using Clinic.Application.Interfaces;
using Clinic.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.Application.UnitTests.Common
{
    public class FakeWorkScheduleRepository : IWorkScheduleRepository
    {
        private readonly List<WorkSchedule> _workSchedules = [];
        private readonly List<ShiftRequest> _shiftRequests = [];
        public async Task AddShiftRequestAsync(ShiftRequest shiftRequest)
        {
            _shiftRequests.Add(shiftRequest);
        }

        public async Task AddWorkScheduleAsync(WorkSchedule workSchedule)
        {
            _workSchedules.Add(workSchedule);
        }

        public async Task<IEnumerable<WorkSchedule>> GetPlannedSchedulesByDoctorIdAsync(Guid doctorId, DateOnly startDate, DateOnly endDate)
        {
            // Check if the time range exceeds 1 month
            if (startDate.AddMonths(1) < endDate)
            {
                throw new ArgumentException("The time range cannot exceed 1 month.");
            }

            return _workSchedules.Where(ws => ws.DoctorId == doctorId &&
                ((DateOnly.FromDateTime(ws.ShiftStart) >= startDate && DateOnly.FromDateTime(ws.ShiftStart) <= endDate) ||
                (DateOnly.FromDateTime(ws.ShiftEnd) >= startDate && DateOnly.FromDateTime(ws.ShiftEnd) <= endDate))
            ).ToList();
        }

        public async Task<IEnumerable<ShiftRequest>> GetRequestedSchedulesByDoctorIdAsync(Guid doctorId, DateOnly startDate, DateOnly endDate)
        {
            // Check if the time range exceeds 1 month
            if (startDate.AddMonths(1) < endDate)
            {
                throw new ArgumentException("The time range cannot exceed 1 month.");
            }

            return _shiftRequests.Where(sr => sr.DoctorId == doctorId &&
                ((DateOnly.FromDateTime(sr.ShiftStart) >= startDate && DateOnly.FromDateTime(sr.ShiftStart) <= endDate) ||
                (DateOnly.FromDateTime(sr.ShiftEnd) >= startDate && DateOnly.FromDateTime(sr.ShiftEnd) <= endDate))
            ).ToList();
        }

        public async Task<ShiftRequest?> GetShiftRequestByIdAsync(Guid scheduleId)
        {
            return _shiftRequests.FirstOrDefault(sr => sr.Id == scheduleId && sr.IsDeleted == false);
        }

        public async Task<WorkSchedule?> GetWorkScheduleByIdAsync(Guid scheduleId)
        {
            return _workSchedules.FirstOrDefault(ws => ws.Id == scheduleId && ws.IsDeleted == false);
        }

        public async Task<bool> HasDuplicateShiftRequest(Guid doctorId, DateTime startTime, DateTime endTime)
        {
            return _shiftRequests.Any(sr => sr.DoctorId == doctorId &&
                ((sr.ShiftStart == endTime && sr.ShiftEnd == startTime)));
        }

        public async Task<bool> HasOverlappingWorkSchedule(Guid doctorId, DateTime startTime, DateTime endTime)
        {
            return _workSchedules.Any(ws => ws.DoctorId == doctorId &&
                ((ws.ShiftStart < endTime && ws.ShiftEnd > startTime)));
        }
    }
}

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
        public Task AddShiftRequestAsync(ShiftRequest shiftRequest)
        {
            throw new NotImplementedException();
        }

        public Task AddWorkScheduleAsync(WorkSchedule workSchedule)
        {
            throw new NotImplementedException();
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

        public Task<ShiftRequest?> GetShiftRequestByIdAsync(Guid scheduleId)
        {
            throw new NotImplementedException();
        }

        public Task<WorkSchedule?> GetWorkScheduleByIdAsync(Guid scheduleId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasDuplicateShiftRequest(Guid doctorId, DateTime startTime, DateTime endTime)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasOverlappingWorkSchedule(Guid doctorId, DateTime startTime, DateTime endTime)
        {
            throw new NotImplementedException();
        }
    }
}

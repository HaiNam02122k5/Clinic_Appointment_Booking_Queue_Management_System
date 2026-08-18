using Clinic.Application.Interfaces;
using Clinic.Domain.Entities;
using Clinic.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.Application.UnitTests.Common
{
    public class FakeWorkScheduleRepository : IWorkScheduleRepository
    {
        private readonly List<WorkSchedule> _workSchedules = [];
        private readonly List<ShiftRequest> _shiftRequests = [];

        // ===== Nhóm chức năng: đặt lịch khám (booking) =====

        public Task<WorkSchedule?> GetByIdAsync(Guid id)
        {
            var result = _workSchedules.FirstOrDefault(w => w.Id == id);
            return Task.FromResult(result);
        }

        /// <summary>
        /// Fake trong bộ nhớ, không có DB thật nên không thể lock thật - no-op để interface
        /// biên dịch được và test có thể gọi CreateAppointmentHandler bình thường.
        /// </summary>
        public Task LockAsync(Guid workScheduleId, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>Helper cho test: seed sẵn 1 WorkSchedule để GetByIdAsync tìm thấy.</summary>
        public void Add(WorkSchedule workSchedule)
        {
            _workSchedules.Add(workSchedule);
        }

        public Task<List<WorkSchedule>> GetAvailableSlotsAsync(Guid? doctorId, Guid? specialtyId, DateTime? fromDate, DateTime? toDate)
        {
            var query = _workSchedules
                .Where(w => w.Status == WorkScheduleStatus.Active && w.ShiftEnd > DateTime.UtcNow);

            if (doctorId.HasValue)
            {
                query = query.Where(w => w.DoctorId == doctorId.Value);
            }

            if (specialtyId.HasValue)
            {
                query = query.Where(w => w.Doctor != null && w.Doctor.WorkHistories.Any(wh =>
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

            query = query.Where(w => w.Appointments.Count(a => a.Status != AppointmentStatus.Cancelled) < w.PatientLimitPerSlot);

            var result = query.OrderBy(w => w.ShiftStart).ToList();
            return Task.FromResult(result);
        }

        // ===== Nhóm chức năng: quản lý ca trực bác sĩ (shift request) =====

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
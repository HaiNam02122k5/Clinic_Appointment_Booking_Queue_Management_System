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
        public async Task AddShiftRequestAsync(ShiftRequest shiftRequest)
        {
            _shiftRequests.Add(shiftRequest);
        }

        public async Task AddWorkScheduleAsync(WorkSchedule workSchedule)
        {
            _workSchedules.Add(workSchedule);
        }

        public async Task<IEnumerable<WorkSchedule>> GetDoctorSchedulesWithAppointmentByDateAsync(Guid doctorId, DateOnly date)
        {
            List<WorkSchedule> result = _workSchedules
                .Where(ws => ws.DoctorId == doctorId &&
                ws.IsDeleted == false &&
                ws.Status == WorkScheduleStatus.Active &&
                ws.Date == date).ToList();
            List<WorkSchedule> returnAns = new List<WorkSchedule>();
            foreach (var schedule in result)
            {
                // workaround to filter inclusion to preserve object stored in context, but lose the reference to other entity
                var ws = new WorkSchedule(schedule.Id, schedule.DoctorId, schedule.Date, schedule.ShiftStart, schedule.ShiftEnd, schedule.PatientLimit, schedule.Status, schedule.CreatedAt, schedule.UpdatedAt, schedule.IsDeleted);
                foreach (var appointment in schedule.Appointments)
                {
                    if (!appointment.IsDeleted && appointment.Status != AppointmentStatus.Cancelled)
                    {
                        ws.AddAppointment(appointment);
                    }
                }
                returnAns.Add(ws);
            }
            return returnAns;
        }

        public async Task<IEnumerable<WorkSchedule>> GetDoctorSchedulesWithAppointmentByDateAsync(Guid? doctorId, DateOnly date)
        {
            List<WorkSchedule> result = _workSchedules
                .Where(ws => ws.DoctorId == doctorId &&
                ws.IsDeleted == false &&
                ws.Status == WorkScheduleStatus.Active &&
                ws.Date == date).ToList();
            List<WorkSchedule> returnAns = new List<WorkSchedule>();
            foreach (var schedule in result)
            {
                // workaround to filter inclusion to preserve object stored in context, but lose the reference to other entity
                var ws = new WorkSchedule(schedule.Id, schedule.DoctorId, schedule.Date, schedule.ShiftStart, schedule.ShiftEnd, schedule.PatientLimit, schedule.Status, schedule.CreatedAt, schedule.UpdatedAt, schedule.IsDeleted);
                foreach (var appointment in schedule.Appointments)
                {
                    if (!appointment.IsDeleted && appointment.Status != AppointmentStatus.Cancelled)
                    {
                        ws.AddAppointment(appointment);
                    }
                }
                returnAns.Add(ws);
            }
            return returnAns;
        }

        public async Task<IEnumerable<WorkSchedule>> GetPlannedSchedulesByDoctorIdAsync(Guid? doctorId, DateOnly startDate, DateOnly endDate)
        {
            // Check if the time range exceeds 1 month
            if (startDate.AddMonths(1) < endDate)
            {
                throw new ArgumentException("The time range cannot exceed 1 month.");
            }

            return _workSchedules.Where(ws => ws.DoctorId == doctorId &&
                ws.Date >= startDate && ws.Date <= endDate && ws.IsDeleted == false && ws.Status != WorkScheduleStatus.Cancelled
            ).ToList();
        }

        public async Task<IEnumerable<ShiftRequest>> GetRequestedSchedulesByDoctorIdAsync(Guid? doctorId, DateOnly startDate, DateOnly endDate)
        {
            // Check if the time range exceeds 1 month
            if (startDate.AddMonths(1) < endDate)
            {
                throw new ArgumentException("The time range cannot exceed 1 month.");
            }

            return _shiftRequests.Where(sr => sr.DoctorId == doctorId &&
                sr.Date >= startDate && sr.Date <= endDate && sr.IsDeleted == false && sr.Status != ShiftRequestStatus.Cancelled
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

        public async Task<bool> HasDuplicateShiftRequest(Guid doctorId, DateOnly date, TimeOnly startTime, TimeOnly endTime)
        {
            return _shiftRequests.Any(sr => sr.DoctorId == doctorId &&
                sr.Date == date &&
                ((sr.ShiftStart == startTime && sr.ShiftEnd == endTime)));
        }

        public async Task<bool> HasOverlappingWorkSchedule(Guid doctorId, DateOnly date, TimeOnly startTime, TimeOnly endTime)
        {
            return _workSchedules.Any(ws => ws.DoctorId == doctorId &&
                ws.Date == date &&
                ((ws.ShiftStart < endTime && ws.ShiftEnd > startTime)));
        }
    }
}

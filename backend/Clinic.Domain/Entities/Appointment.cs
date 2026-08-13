using Clinic.Domain.Common;
using Clinic.Domain.Enums;
using System;

namespace Clinic.Domain.Entities
{
    public class Appointment : BaseEntity
    {
        private const int CancelLimitHours = 2;
        private readonly TimeSpan VietnamTimeOffset = TimeSpan.FromHours(7); // Giờ Việt Nam (UTC+7)

        public Guid PatientId { get; protected set; }
        public Patient Patient { get; protected set; } = null!;

        public Guid WorkScheduleId { get; protected set; }
        public WorkSchedule WorkSchedule { get; protected set; } = null!;

        public string? Reason { get; protected set; }

        public TimeOnly TimeSlot { get; protected set; }

        public AppointmentStatus Status { get; protected set; } = AppointmentStatus.Pending;

        public bool IsWalkIn { get; protected set; } = false;

        /// <summary>0..1 - chỉ có sau khi bệnh nhân check-in.</summary>
        public QueueTicket? QueueTicket { get; protected set; }

        private Appointment() { } // For EF Core

        public Appointment(Patient patient, WorkSchedule workSchedule, TimeOnly timeSlot, bool isWalkIn = false)
        {
            Patient = patient ?? throw new ArgumentNullException(nameof(patient));
            PatientId = patient.Id;
            WorkSchedule = workSchedule ?? throw new ArgumentNullException(nameof(workSchedule));
            WorkScheduleId = workSchedule.Id;
            TimeSlot = timeSlot;
            IsWalkIn = isWalkIn;
        }

        public void AdminCancelWithReason()
        {
            throw new NotImplementedException();
        }

        public void Update(WorkSchedule workSchedule, TimeOnly timeSlot)
        {
            if (Status is AppointmentStatus.Completed or AppointmentStatus.Cancelled)
            {
                throw new InvalidOperationException($"{Enum.GetName(Status)} appointments can't be updated.");
            }
            WorkSchedule = workSchedule ?? throw new ArgumentNullException(nameof(workSchedule));
            WorkScheduleId = workSchedule.Id;
            TimeSlot = timeSlot;
        }

        public void Cancel()
        {
            if (Status is AppointmentStatus.Completed or AppointmentStatus.Cancelled)
            {
                throw new InvalidOperationException($"{Enum.GetName(Status)} appointments can't be cancelled.");
            }
            var utcSlot = new TimeConverter().ConvertToUtc(new DateTime(WorkSchedule.Date, TimeSlot));
            if (DateTime.UtcNow.AddHours(CancelLimitHours) > utcSlot)
            {
                throw new InvalidOperationException($"Appointments can only be cancelled at least {CancelLimitHours} hours before the scheduled time.");
            }
            Status = AppointmentStatus.Cancelled;
        }
    }
}

using Clinic.Domain.Common;
using Clinic.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.Domain.Entities
{
    public class AppointmentSnapshot
    {
        public Guid Id { get; protected set; }
        public Guid AppointmentId { get; protected set; }
        public Guid OldWorkScheduleId { get; protected set; }
        public TimeOnly TimeSlot { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public Guid CreatedByUserId { get; protected set; }
        public string Reason { get; protected set; }
        public AppointmentStatus Status { get; protected set; }

        // Navigation properties
        public Appointment Appointment { get; protected set; }
        public WorkSchedule OldWorkSchedule { get; protected set; }
        public User Creator { get; protected set; }

        private AppointmentSnapshot() { } // For EF Core

        public AppointmentSnapshot(Appointment appointment)
        {
            Id = Guid.NewGuid();
            Appointment = appointment;
            AppointmentId = appointment.Id;
            OldWorkScheduleId = appointment.WorkScheduleId;
            TimeSlot = appointment.TimeSlot;
            CreatedAt = appointment.CreatedAt;
            Status = appointment.Status;
            Reason = appointment.Reason;
            CreatedByUserId = appointment.CreatedByUserId;
        }
    }
}

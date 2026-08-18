using Clinic.Domain.Common;
using Clinic.Domain.Enums;
using System;
using System.Collections.Generic;

namespace Clinic.Domain.Entities
{
    /// <summary>Một khung giờ làm việc của bác sĩ, giới hạn số bệnh nhân tối đa.</summary>
    public class WorkSchedule : BaseEntity
    {
        public Guid DoctorId { get; set; }
        public Doctor Doctor { get; set; } = null!;

        public DateTime ShiftStart { get; set; }

        public DateTime ShiftEnd { get; set; }

        public int PatientLimitPerSlot { get; set; }

        public WorkScheduleStatus Status { get; set; } = WorkScheduleStatus.Active;

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

        public WorkSchedule(Doctor doctor, DateTime shiftStart, DateTime shiftEnd, int patientLimitPerSlot)
        {
            if (doctor == null) throw new ArgumentNullException(nameof(doctor));
            if (shiftStart < DateTime.UtcNow) throw new ArgumentException("Shift start time must be in the future.");
            if (shiftStart >= shiftEnd) throw new ArgumentException("Shift start time must be before shift end time.");
            if (patientLimitPerSlot <= 0) throw new ArgumentException("Patient limit per slot must be greater than zero.");
            Doctor = doctor;
            DoctorId = doctor.Id;
            ShiftStart = shiftStart;
            ShiftEnd = shiftEnd;
            PatientLimitPerSlot = patientLimitPerSlot;
        }

        public WorkSchedule(Guid id, Guid doctorId, DateTime shiftStart, DateTime shiftEnd, int patientLimitPerSlot, WorkScheduleStatus status, DateTime createdAt, DateTime? updatedAt, bool isDeleted)
            : base(id, createdAt, updatedAt, isDeleted)
        {
            DoctorId = doctorId;
            ShiftStart = shiftStart;
            ShiftEnd = shiftEnd;
            PatientLimitPerSlot = patientLimitPerSlot;
            Status = status;
        }

        public void UpdateShift(DateTime newShiftStart, DateTime newShiftEnd, int newPatientLimitPerSlot)
        {
            if (ShiftStart <= DateTime.UtcNow && newShiftStart != ShiftStart) throw new InvalidOperationException("Cannot update the start time of a shift that has already started.");
            if (newShiftStart != ShiftStart && newShiftStart < DateTime.UtcNow) throw new ArgumentException("New shift start time must be in the future.");
            if (newShiftStart >= newShiftEnd) throw new ArgumentException("New shift start time must be before new shift end time.");
            if (newPatientLimitPerSlot <= 0) throw new ArgumentException("New patient limit per slot must be greater than zero.");
            ShiftStart = newShiftStart;
            ShiftEnd = newShiftEnd;
            PatientLimitPerSlot = newPatientLimitPerSlot;
            MarkUpdated();
        }

        public void UpdateStatus(WorkScheduleStatus newStatus)
        {
            if (ShiftEnd <= DateTime.UtcNow) throw new InvalidOperationException("Cannot update a shift that has already ended.");
            if (Status != newStatus)
            {
                Status = newStatus;
                MarkUpdated();
            }
        }

        public void Delete()
        {
            if (ShiftStart <= DateTime.UtcNow) throw new InvalidOperationException("Cannot delete a shift that has already started.");
            IsDeleted = true;
            MarkUpdated();
        }
    }
}

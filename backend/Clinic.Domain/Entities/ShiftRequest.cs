using Clinic.Domain.Common;
using Clinic.Domain.Enums;
using System;

namespace Clinic.Domain.Entities
{
    public class ShiftRequest : BaseEntity
    {
        public Guid DoctorId { get; protected set; }
        public Doctor Doctor { get; protected set; } = null!;

        public DateTime ShiftStart { get; protected set; }

        public DateTime ShiftEnd { get; protected set; }

        public int PatientLimit { get; protected set; }

        public string? Reason { get; protected set; }

        public ShiftRequestStatus Status { get; protected set; } = ShiftRequestStatus.Pending;

        public ShiftRequest(Doctor doctor, DateTime shiftStart, DateTime shiftEnd, int patientLimit, string? reason)
        {
            if (doctor == null) throw new ArgumentNullException(nameof(doctor));
            if (shiftStart >= shiftEnd) throw new ArgumentException("Shift start time must be before shift end time.");
            if (shiftStart < DateTime.UtcNow) throw new ArgumentException("Shift start time must be in the future.");
            if (patientLimit <= 0) throw new ArgumentException("Patient limit per slot must be greater than zero.");
            Doctor = doctor;
            DoctorId = doctor.Id;
            ShiftStart = shiftStart;
            ShiftEnd = shiftEnd;
            PatientLimit = patientLimit;
            Reason = reason;
        }

        public ShiftRequest(Guid id, Guid doctorId, DateTime shiftStart, DateTime shiftEnd, int patientLimit, string? reason, ShiftRequestStatus status, DateTime createdAt, DateTime? updatedAt, bool isDeleted)
            : base(id, createdAt, updatedAt, isDeleted)
        {
            DoctorId = doctorId;
            ShiftStart = shiftStart;
            ShiftEnd = shiftEnd;
            PatientLimit = patientLimit;
            Reason = reason;
            Status = status;
        }

        public void UpdateShift(DateTime newShiftStart, DateTime newShiftEnd, int newPatientLimit, string? newReason)
        {
            if (ShiftStart <= DateTime.UtcNow) throw new InvalidOperationException("Cannot update a shift that has already started.");
            if (Status != ShiftRequestStatus.Pending) throw new InvalidOperationException("Only pending shift requests can be updated.");
            if (newShiftStart >= newShiftEnd) throw new ArgumentException("New shift start time must be before new shift end time.");
            if (newShiftStart < DateTime.UtcNow) throw new ArgumentException("New shift start time must be in the future.");
            if (newPatientLimit <= 0) throw new ArgumentException("Patient limit per slot must be greater than zero.");
            ShiftStart = newShiftStart;
            ShiftEnd = newShiftEnd;
            PatientLimit = newPatientLimit;
            Reason = newReason;
            MarkUpdated();
        }

        public void Delete()
        {
            IsDeleted = true;
            MarkUpdated();
        }

        public void Approve()
        {
            if (Status != ShiftRequestStatus.Pending) throw new InvalidOperationException("Only pending shift requests can be approved.");
            Status = ShiftRequestStatus.Approved;
            Doctor.AddWorkSchedule(new WorkSchedule(Doctor, ShiftStart, ShiftEnd, PatientLimit));
            MarkUpdated();
        }

        public void Cancel()
        {
            if (Status != ShiftRequestStatus.Pending) throw new InvalidOperationException("Only pending shift requests can be canceled.");
            Status = ShiftRequestStatus.Cancelled;
            MarkUpdated();
        }

        public void Reject()
        {
            if (Status != ShiftRequestStatus.Pending) throw new InvalidOperationException("Only pending shift requests can be rejected.");
            Status = ShiftRequestStatus.Rejected;
            MarkUpdated();
        }
    }
}

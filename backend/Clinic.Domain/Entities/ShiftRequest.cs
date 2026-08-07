using Clinic.Domain.Common;
using Clinic.Domain.Enums;
using System;

namespace Clinic.Domain.Entities
{
    public class ShiftRequest : BaseEntity
    {
        public Guid DoctorId { get; set; }
        public Doctor Doctor { get; set; } = null!;

        public DateTime ShiftStart { get; set; }

        public DateTime ShiftEnd { get; set; }

        public string? Reason { get; set; }

        public ShiftRequestStatus Status { get; set; } = ShiftRequestStatus.Pending;

    }
}

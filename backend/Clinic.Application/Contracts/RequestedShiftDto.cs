using Clinic.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.Application.Contracts
{
    public class RequestedShiftDto
    {
        public Guid Id { get; set; }
        public Guid DoctorId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int PatientLimitPerSlot { get; set; }
        public string? Reason { get; set; }
        public ShiftRequestStatus Status { get; set; }
    }
}

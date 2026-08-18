using Clinic.Domain.Enums;

namespace Clinic.Application.Contracts
{
    public class WorkScheduleDto
    {
        public Guid Id { get; set; }
        public Guid DoctorId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int PatientLimitPerSlot { get; set; }
        public WorkScheduleStatus Status { get; set; }
    }
}

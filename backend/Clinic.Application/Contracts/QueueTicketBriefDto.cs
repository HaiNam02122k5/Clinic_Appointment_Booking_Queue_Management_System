using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.Application.Contracts
{
    public class QueueTicketBriefDto
    {
        public Guid Id { get; set; }
        public Guid AppointmentId { get; set; }
        public string DoctorName { get; set; }
        public string PatientName { get; set; }
        public string SpecialtyName { get; set; }
        public int QueueNumber { get; set; }
        public DateTime CheckInTime { get; set; }
    }
}

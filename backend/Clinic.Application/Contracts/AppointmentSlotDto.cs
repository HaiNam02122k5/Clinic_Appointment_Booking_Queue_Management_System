using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.Application.Contracts
{
    public class AppointmentSlotDto
    {
        public Guid DoctorId { get; set; }
        public DateTime StartTime { get; set; }
    }
}

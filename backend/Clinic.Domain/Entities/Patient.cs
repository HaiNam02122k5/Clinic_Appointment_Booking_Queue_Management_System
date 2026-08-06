using Clinic.Domain.Common;
using System;
using System.Collections.Generic;

namespace Clinic.Domain.Entities
{
    public class Patient : BaseEntity
    {
        /// <summary>FK, UNIQUE. Cho phép Person chưa từng có User (khách vãng lai).</summary>
        public Guid PersonId { get; set; }
        public Person Person { get; set; } = null!;

        public string? InsuranceNumber { get; set; }

        public string? EmergencyContact { get; set; }

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}

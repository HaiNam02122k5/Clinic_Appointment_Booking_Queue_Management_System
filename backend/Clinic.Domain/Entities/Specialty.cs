using Clinic.Domain.Common;
using System.Collections.Generic;

namespace Clinic.Domain.Entities
{
    public class Specialty : BaseEntity
    {
        /// <summary>UNIQUE.</summary>
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    }
}

using Clinic.Domain.Common;
using System.Collections.Generic;

namespace Clinic.Domain.Entities
{
    public class Specialty : BaseEntity
    {
        /// <summary>UNIQUE.</summary>
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        /// <summary>
        /// Không còn navigation trực tiếp tới Doctor (đã bỏ Doctor.SpecialtyId).
        /// Muốn tìm bác sĩ thuộc chuyên khoa này, query qua WorkHistory
        /// (WHERE SpecialtyId = this.Id AND Status == Active).
        /// </summary>
        public ICollection<WorkHistory> WorkHistories { get; set; } = new List<WorkHistory>();
    }
}

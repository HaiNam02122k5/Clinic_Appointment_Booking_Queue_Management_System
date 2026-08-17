using System.ComponentModel.DataAnnotations;

namespace Clinic.API.Models
{
    public class AdminUpdateEmployeeRequest : UpdateEmployeeRequest
    {
        [Required(ErrorMessage = "EmployeeId is required.")]
        public Guid EmployeeId { get; set; }
        [Required(ErrorMessage = "At least one role is required.")]
        public List<string> Roles { get; set; }
    }
}

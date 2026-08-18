using System.ComponentModel.DataAnnotations;

namespace Clinic.API.Models
{
    public class CreateShiftRequest
    {
        [Required(ErrorMessage = "StartTime is required.")]
        public DateTime StartTime { get; set; }
        [Required(ErrorMessage = "EndTime is required.")]
        public DateTime EndTime { get; set; }
        [Required(ErrorMessage = "PatientLimitPerSlot is required."), Range(1, 50, ErrorMessage = "PatientLimitPerSlot must be between 1 and 50.")]
        public int PatientLimitPerSlot { get; set; }
    }
}

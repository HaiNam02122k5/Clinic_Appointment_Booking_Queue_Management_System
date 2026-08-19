using System.ComponentModel.DataAnnotations;

namespace Clinic.API.Models
{
    public class CreateShiftSuggestionRequest
    {
        [Required(ErrorMessage = "Start time is required.")]
        public DateTime StartTime { get; set; }
        [Required(ErrorMessage = "End time is required.")]
        public DateTime EndTime { get; set; }
        [Required(ErrorMessage = "Patient limit per slot is required.")]
        public int PatientLimitPerSlot { get; set; }
        [Required(ErrorMessage = "Reason is required.")]
        public string Reason { get; set; }
    }
}

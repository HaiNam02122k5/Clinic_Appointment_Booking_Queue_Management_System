using System.ComponentModel.DataAnnotations;

namespace Clinic.API.Models
{
    public class ReceptionistCreateAppointmentRequest : CreateAppointmentRequest
    {
        [Required(ErrorMessage = "PatientId is required.")]
        public Guid PatientId { get; set; }
    }
}

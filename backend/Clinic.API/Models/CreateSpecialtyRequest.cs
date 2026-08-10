using System.ComponentModel.DataAnnotations;

namespace Clinic.API.Models
{
    public class CreateSpecialtyRequest
    {
        [Required(ErrorMessage = "Name is required."), MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; }

        [MaxLength(10000, ErrorMessage = "Description cannot exceed 10000 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Established date is required.")]
        public DateOnly EstablishedDate { get; set; }
    }
}

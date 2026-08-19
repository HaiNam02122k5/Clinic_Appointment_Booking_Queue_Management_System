using System.Text.Json.Serialization;

namespace Clinic.Application.Contracts
{
    public class AppointmentDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("doctorId")]
        public Guid DoctorId { get; set; }

        [JsonPropertyName("doctorName")]
        public string DoctorName { get; set; } = string.Empty;

        [JsonPropertyName("timeSlot")]
        public DateTime TimeSlot { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("reason")]
        public string? Reason { get; set; }

        [JsonPropertyName("isWalkIn")]
        public bool IsWalkIn { get; set; }
    }
}
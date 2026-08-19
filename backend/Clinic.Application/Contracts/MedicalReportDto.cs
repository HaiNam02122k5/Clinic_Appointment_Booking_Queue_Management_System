using System.Text.Json.Serialization;

namespace Clinic.Application.Contracts
{
    public class MedicalReportDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("doctorName")]
        public string DoctorName { get; set; } = string.Empty;

        [JsonPropertyName("examDate")]
        public DateTime ExamDate { get; set; }

        [JsonPropertyName("symptoms")]
        public string? Symptoms { get; set; }

        [JsonPropertyName("diagnosis")]
        public string? Diagnosis { get; set; }

        [JsonPropertyName("prescription")]
        public string? Prescription { get; set; }

        [JsonPropertyName("notes")]
        public string? Notes { get; set; }
    }
}
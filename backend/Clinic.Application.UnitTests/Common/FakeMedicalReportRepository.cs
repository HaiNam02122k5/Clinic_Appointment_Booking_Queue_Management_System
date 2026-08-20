using Clinic.Application.Interfaces;
using Clinic.Domain.Entities;

namespace Clinic.Application.UnitTests.Common
{
    public class FakeMedicalReportRepository : IMedicalReportRepository
    {
        private readonly List<MedicalReport> _reports = [];

        public void Add(MedicalReport report)
        {
            _reports.Add(report);
        }

        public Task<MedicalReport?> GetByIdWithOwnershipAsync(Guid id)
        {
            var report = _reports.FirstOrDefault(r => r.Id == id && !r.IsDeleted);
            return Task.FromResult(report);
        }

        public Task<List<MedicalReport>> GetHistoryByPatientIdAsync(Guid patientId)
        {
            var history = _reports
                .Where(r => !r.IsDeleted && r.QueueTicket?.Appointment?.PatientId == patientId)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            return Task.FromResult(history);
        }
    }
}

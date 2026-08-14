using Clinic.Application.Interfaces;
using Clinic.Domain.Entities;

namespace Clinic.Application.UnitTests.Common
{
    public class FakeAppointmentRepository : IAppointmentRepository
    {
        private readonly List<Appointment> _appointments = [];

        public async Task<Appointment?> GetByIdAsync(Guid id)
        {
            return _appointments.FirstOrDefault(a => a.Id == id);
        }

        public async Task AddAsync(Appointment appointment)
        {
            _appointments.Add(appointment);
        }

        public async Task UpdateAsync(Appointment appointment)
        {
            return;
        }

        public async Task<bool> ExistsForDoctorAndPatientAsync(Guid doctorId, Guid patientId)
        {
            return _appointments.Any(a => a.PatientId == patientId && a.WorkSchedule.DoctorId == doctorId);
        }
    }
}
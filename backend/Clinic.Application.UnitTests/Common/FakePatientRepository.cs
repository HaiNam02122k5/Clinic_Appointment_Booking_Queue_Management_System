using Clinic.Application.Interfaces;
using Clinic.Domain.Entities;

namespace Clinic.Application.UnitTests.Common
{
    public class FakePatientRepository : IPatientRepository
    {
        private readonly List<Patient> _patients = [];

        public Task<Patient?> GetByPersonIdAsync(Guid personId)
        {
            return Task.FromResult(_patients.FirstOrDefault(p => p.PersonId == personId));
        }

        public Task<Patient?> GetByIdAsync(Guid id)
        {
            return Task.FromResult(_patients.FirstOrDefault(p => p.Id == id));
        }

        public Task AddAsync(Patient patient)
        {
            _patients.Add(patient);
            return Task.CompletedTask;
        }
    }
}
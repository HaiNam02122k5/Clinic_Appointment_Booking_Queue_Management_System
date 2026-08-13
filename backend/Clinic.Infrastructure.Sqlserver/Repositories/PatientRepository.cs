using Clinic.Application.Interfaces;
using Clinic.Domain.Entities;
using Clinic.Infrastructure.Sqlserver.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Infrastructure.Sqlserver.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly ApplicationDbContext _context;
        public PatientRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Patient?> GetByIdAsync(Guid? patientId)
        {
            return await _context.Patients.Include(p => p.Person).ThenInclude(p => p.User).FirstOrDefaultAsync(p => p.Id == patientId);
        }

        public async Task<Patient?> GetPatientByUserIdAsync(Guid userId)
        {
            return await _context.Patients.Include(p => p.Person).ThenInclude(p => p.User).FirstOrDefaultAsync(p => p.Person.User.Id == userId);
        }
    }
}

using Clinic.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.Application.Interfaces
{
    public interface IPatientRepository
    {
        Task<Patient?> GetByIdAsync(Guid? patientId);

        /// <summary>
        /// Gets a patient by their associated user ID. Used for retrieving patient information based on the their user account.
        /// </summary>
        Task<Patient?> GetPatientByUserIdAsync(Guid userId);
    }
}

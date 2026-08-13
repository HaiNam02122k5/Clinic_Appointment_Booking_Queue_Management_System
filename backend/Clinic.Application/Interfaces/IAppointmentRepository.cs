using Clinic.Application.Common.Models;
using Clinic.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.Application.Interfaces
{
    public interface IAppointmentRepository
    {
        /// <summary>
        /// Gets a paged list of appointments for a specific patient, filtered by category.
        /// </summary>
        Task<PagedResult<Appointment>> GetAppointmentsByPatientIdAsync(Guid id, string category);
        Task<Appointment?> GetByIdAsync(Guid appointmentId);
        Task<bool> IsTimeSlotTakenAsync(Guid doctorId, DateTime timeSlot);
    }
}

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
        Task<PagedResult<Appointment>> GetAppointmentsByPatientIdAsync(Guid patientId, string category);
        Task<Appointment?> GetByIdAsync(Guid appointmentId);
        Task<bool> IsTimeSlotTakenAsync(Guid doctorId, DateTime timeSlot);
        /// <summary>
        /// Kiểm tra bác sĩ có từng/đang phụ trách ca khám nào của bệnh nhân này không
        /// (tồn tại Appointment mà WorkSchedule.DoctorId = doctorId và PatientId = patientId).
        /// Dùng để scope "related" cho medical-report.view/patient-history.view của Doctor.
        /// </summary>
        Task<bool> ExistsForDoctorAndPatientAsync(Guid doctorId, Guid patientId);
    }
}

using Clinic.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.Application.Interfaces
{
    public interface IAppointmentRepository
    {
        /// <summary>
        /// Gets an appointment by its unique identifier (ID). Trả về null nếu không tìm thấy.
        /// </summary>
        Task<Appointment?> GetByIdAsync(Guid id);

        /// <summary>
        /// Adds a new appointment to the system.
        /// </summary>
        Task AddAsync(Appointment appointment);

        /// <summary>
        /// Updates an existing appointment's information in the system.
        /// </summary>
        Task UpdateAsync(Appointment appointment);

        /// <summary>
        /// Lấy toàn bộ lịch hẹn của 1 bệnh nhân, kèm WorkSchedule -> Doctor -> Employee -> Person
        /// (để hiển thị tên bác sĩ), sắp xếp lịch hẹn mới nhất trước.
        /// </summary>
        Task<List<Appointment>> GetByPatientIdAsync(Guid patientId);

        /// <summary>
        /// Kiểm tra bác sĩ có từng/đang phụ trách ca khám nào của bệnh nhân này không
        /// (tồn tại Appointment mà WorkSchedule.DoctorId = doctorId và PatientId = patientId).
        /// Dùng để scope "related" cho medical-report.view/patient-history.view của Doctor.
        /// </summary>
        Task<bool> ExistsForDoctorAndPatientAsync(Guid doctorId, Guid patientId);
    }
}
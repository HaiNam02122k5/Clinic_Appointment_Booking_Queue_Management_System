using Clinic.Application.Common.Models;
using Clinic.Application.Interfaces;
using Clinic.Domain.Entities;
using Clinic.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Application.UnitTests.Common
{
    public class FakeAppointmentRepository : IAppointmentRepository
    {
        private readonly List<Appointment> _appointments = [];

        public Task<bool> ExistsForDoctorAndPatientAsync(Guid doctorId, Guid patientId)
        {
            throw new NotImplementedException();
        }

        public async Task<PagedResult<Appointment>> GetAppointmentsByPatientIdAsync(Guid patientId, string category)
        {
            var query = _appointments
                .Where(a => a.IsDeleted == false && a.PatientId == patientId);

            // Apply category filter if provided
            query = category.ToLower() switch
            {
                "upcoming" => query.Where(a => new[] { AppointmentStatus.Pending, AppointmentStatus.Confirmed, AppointmentStatus.CheckedIn }.Contains(a.Status)),
                "completed" => query.Where(a => a.Status == AppointmentStatus.Completed),
                "cancelled" => query.Where(a => a.Status == AppointmentStatus.Cancelled),
                _ => query
            };

            var items = query.OrderByDescending(a => a.WorkSchedule.Date).ThenByDescending(a => a.TimeSlot).ToList();

            return new PagedResult<Appointment>
            (
                items: items,
                totalCount: items.Count
            );
        }

        public async Task<Appointment?> GetByIdAsync(Guid appointmentId)
        {
            return _appointments.FirstOrDefault(a => a.Id == appointmentId && !a.IsDeleted);
        }

        public Task<bool> IsTimeSlotTakenAsync(Guid doctorId, DateTime timeSlot)
        {
            throw new NotImplementedException();
        }

        internal async Task AddAsync(Appointment appointment)
        {
            _appointments.Add(appointment);
        }
    }
}

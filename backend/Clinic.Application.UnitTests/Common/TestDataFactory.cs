using Clinic.Domain.Entities;
using Clinic.Domain.Enums;

namespace Clinic.Application.UnitTests.Common
{
    public class TestDataFactory
    {
        public static List<Role> RoleSet => new List<Role>
        {
            new Role(Guid.NewGuid(), "Admin", "Administrator role", DateTime.UtcNow, DateTime.UtcNow, false),
            new Role(Guid.NewGuid(), "Doctor", "Doctor role", DateTime.UtcNow, DateTime.UtcNow, false),
            new Role(Guid.NewGuid(), "Receptionist", "Receptionist role", DateTime.UtcNow, DateTime.UtcNow, false),
            new Role(Guid.NewGuid(), "Patient", "Patient role", DateTime.UtcNow, DateTime.UtcNow, false),
        };

        public static Person CreatePerson(string fullName = "John Doe", string? phoneNumber = "1234567890", string? email = "", string? address = "")
        {
            return new Person(fullName, phoneNumber, email, DateOnly.FromDateTime(DateTime.UtcNow), Gender.Male, address);
        }

        public static User CreateUser(string username = "testuser", string passwordHash = "hashedpassword", Person? person = null)
        {
            var newUser = new User(username, passwordHash, person ?? CreatePerson());
            return newUser;
        }

        /// <summary>
        /// Tạo 1 QueueTicket hợp lệ, liên kết 2 chiều với appointment (giống CheckInHandler thật).
        /// </summary>
        public static QueueTicket CreateQueueTicket(Appointment appointment, int queueNumber = 1, bool priority = false)
        {
            var queueTicket = new QueueTicket
            {
                AppointmentId = appointment.Id,
                Appointment = appointment,
                QueueNumber = queueNumber,
                CheckInTime = DateTime.UtcNow
            };
            appointment.QueueTicket = queueTicket;

            if (priority)
            {
                queueTicket.SetPriority(true);
            }

            return queueTicket;
        }
        public static RefreshToken CreateRefreshToken(string hashedToken, User user)
        {
            var refreshToken = new RefreshToken(hashedToken, DateTime.UtcNow.AddDays(7), user);
            return refreshToken;
        }

        /// <summary>
        /// Tạo 1 Appointment hợp lệ (kèm WorkSchedule gắn với doctorId) để dùng cho test
        /// Confirm/CheckIn/Cancel. Status mặc định là Pending (trạng thái khởi tạo của Appointment);
        /// truyền confirmed = true để có sẵn Appointment ở trạng thái Confirmed (phục vụ test CheckIn),
        /// hoặc checkedIn = true để có sẵn Appointment ở trạng thái CheckedIn (phục vụ test Cancel
        /// sau khi đã có QueueTicket).
        /// </summary>
        public static Appointment CreateAppointment(Guid? patientId = null, Guid? doctorId = null, bool confirmed = false, bool checkedIn = false)
        {
            var workSchedule = new WorkSchedule
            {
                DoctorId = doctorId ?? Guid.NewGuid(),
                ShiftStart = DateTime.UtcNow.AddHours(1),
                ShiftEnd = DateTime.UtcNow.AddHours(2),
                PatientLimitPerSlot = 5,
            };

            var appointment = new Appointment
            {
                PatientId = patientId ?? Guid.NewGuid(),
                WorkScheduleId = workSchedule.Id,
                WorkSchedule = workSchedule,
                TimeSlot = DateTime.UtcNow.AddHours(1),
            };

            if (confirmed || checkedIn)
            {
                appointment.Confirm();
            }

            if (checkedIn)
            {
                appointment.CheckIn();
            }

            return appointment;
        }
    }
}
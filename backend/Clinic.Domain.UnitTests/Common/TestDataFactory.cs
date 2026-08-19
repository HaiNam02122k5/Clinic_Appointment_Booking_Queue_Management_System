using Clinic.Domain.Entities;
using Clinic.Domain.Enums;
using System.Diagnostics.CodeAnalysis;

namespace Clinic.Domain.UnitTests.Common
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

        public static User CreateUser(string username = "testuser", string passwordHash = "hashedpassword", Person person = null)
        {
            var newUser = new User(username, passwordHash, person ?? CreatePerson());
            return newUser;
        }

        /// <summary>
        /// Tạo 1 Appointment hợp lệ (kèm WorkSchedule) để dùng cho test Confirm/CheckIn/Cancel.
        /// Status mặc định là Pending; truyền confirmed = true / checkedIn = true để có sẵn
        /// Appointment ở trạng thái tương ứng.
        /// </summary>
        public static Appointment CreateAppointment(Guid? patientId = null, Guid? doctorId = null, bool confirmed = false, bool checkedIn = false)
        {
            // WorkSchedule giờ bắt buộc gắn với 1 Doctor object (không chỉ DoctorId), nên
            // dựng 1 Doctor "giả" qua constructor reconstruct để giữ đúng Id đã truyền vào,
            // tránh phải dựng cả Employee/Specialty chỉ để phục vụ test Appointment.
            var doctor = new Doctor(
                id: doctorId ?? Guid.NewGuid(),
                employeeId: Guid.NewGuid(),
                licenseNumber: "TEST-LICENSE",
                qualification: "MD",
                experienceYears: 0,
                biography: null,
                status: DoctorStatus.Active,
                createdAt: DateTime.UtcNow,
                updatedAt: DateTime.UtcNow,
                isDeleted: false
            );

            var workSchedule = new WorkSchedule(
                doctor,
                DateTime.UtcNow.AddHours(1),
                DateTime.UtcNow.AddHours(2),
                5
            );

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
                appointment.CheckIn(DateTime.UtcNow);
            }

            return appointment;
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
    }
}
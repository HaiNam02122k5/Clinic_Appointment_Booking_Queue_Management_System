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

        public static Person CreatePerson(string fullName = "John Doe", string? phoneNumber = "1234567890", string? email = "", string? address = "", DateOnly? dateOfBirth = null, string? userRole = null)
        {
            var person = new Person(fullName, phoneNumber, email, dateOfBirth ?? DateOnly.FromDateTime(DateTime.UtcNow), Gender.Male, address);
            if (!string.IsNullOrEmpty(userRole))
            {
                var user = CreateUser(userRole, $"{fullName.Replace(" ", "").ToLower()}user", "hashedpassword", person);
                person.User = user;
            }
            return person;
        }

        public static User CreateUser(string role = "Admin", string username = "testuser", string passwordHash = "hashedpassword", Person? person = null)
        {
            person ??= CreatePerson();
            var newUser = new User(username, passwordHash, person);
            person.User = newUser;
            newUser.AssignRole(RoleSet.FirstOrDefault(r => r.Name == role) ?? RoleSet.First());
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
        public static Appointment CreateAppointment(Guid? patientId = null, Guid? doctorId = null, bool confirmed = false, bool checkedIn = false, DateTime? timeSlot = null)
        {
            // WorkSchedule giờ bắt buộc gắn với 1 Doctor object (không chỉ DoctorId), nên
            // dựng 1 Doctor "giả" qua constructor reconstruct để giữ đúng Id đã truyền vào,
            // tránh phải dựng cả Employee/Specialty chỉ để phục vụ test Appointment/Queue.
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
                TimeSlot = timeSlot ?? DateTime.UtcNow.AddHours(1),
            };

            if (confirmed || checkedIn)
            {
                appointment.Confirm();
            }

            if (checkedIn)
            {
                // Lưu ý: nếu truyền timeSlot khác ngày hôm nay kèm checkedIn: true, dòng này sẽ tự
                // ném ConflictException ngay trong lúc dựng dữ liệu test (đúng theo domain rule mới) -
                // muốn test case "check-in lịch ngày khác" thì dùng confirmed: true + timeSlot khác
                // ngày, rồi tự gọi handler.Handle(...) để assert exception, không dùng checkedIn: true.
                appointment.CheckIn(DateTime.UtcNow);
            }

            return appointment;
        }

        public static Specialty CreateSpecialty(string? name = "Cardiology", string? description = "Heart specialist")
        {
            var specialty = new Specialty(name, description, DateOnly.FromDateTime(DateTime.UtcNow));
            return specialty;
        }

        public static Employee CreateEmployee(Person? person = null, string role = "Admin")
        {
            person ??= CreatePerson(userRole: role);
            var newEmployee = new Employee(person, DateOnly.FromDateTime(DateTime.UtcNow));
            person.Employee = newEmployee;
            return newEmployee;
        }

        internal static Doctor CreateDoctor(Employee? employee = null, Specialty? specialty = null, string? licenseNumber = "ABC123", string? qualification = "MD", string? bio = "", int yoe = 0)
        {
            var doctor = new Doctor(employee ?? CreateEmployee(role: "Doctor"), licenseNumber, qualification, specialty ?? CreateSpecialty(), yoe, bio);
            doctor.Employee?.AssignDoctor(doctor);
            return doctor;
        }
    }
}
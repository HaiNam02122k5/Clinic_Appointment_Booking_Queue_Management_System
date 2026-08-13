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

        public static Person CreatePerson(string fullName = "John Doe", string? phoneNumber = "1234567890", string? email = "", string? address = "", DateOnly? dateOfBirth = null)
        {
            return new Person(fullName, phoneNumber, email, dateOfBirth ?? DateOnly.FromDateTime(DateTime.UtcNow), Gender.Male, address);
        }

        public static User CreateUser(string username = "testuser", string passwordHash = "hashedpassword", Person? person = null)
        {
            person ??= CreatePerson();
            var newUser = new User(username, passwordHash, person);
            person.User = newUser;
            return newUser;
        }

        public static RefreshToken CreateRefreshToken(string hashedToken, User user)
        {
            var refreshToken = new RefreshToken(hashedToken, DateTime.UtcNow.AddDays(7), user);
            return refreshToken;
        }

        public static Specialty CreateSpecialty(string? name = "Cardiology", string? description = "Heart specialist")
        {
            var specialty = new Specialty(name, description, DateOnly.FromDateTime(DateTime.UtcNow));
            return specialty;
        }

        public static Employee CreateEmployee(Person? person = null)
        {
            person ??= CreatePerson();
            var newEmployee = new Employee(person, DateOnly.FromDateTime(DateTime.UtcNow));
            person.Employee = newEmployee;
            return newEmployee;
        }

        internal static Doctor CreateDoctor(Employee? employee = null, Specialty? specialty = null, string? licenseNumber = "ABC123", string? qualification = "MD", string? bio = "", int yoe = 0)
        {
            var doctor = new Doctor(employee ?? CreateEmployee(), licenseNumber, qualification, specialty ?? CreateSpecialty(), yoe, bio);
            return doctor;
        }

        internal static Patient CreatePatient(Person? person = null)
        {
            person ??= CreatePerson();
            return new Patient
            {
                Person = person,
                PersonId = person.Id,
                InsuranceNumber = "INS123456",
                EmergencyContact = "Jane Doe - 0987654321"
            };
        }

        internal static WorkSchedule CreateWorkSchedule(Doctor? doctor = null)
        {
            return new WorkSchedule(doctor ?? CreateDoctor(), DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), new TimeOnly(9, 0), new TimeOnly(17, 0), 10);
        }

        internal static Appointment CreateAppointment(Patient? patient = null, WorkSchedule? workSchedule = null, TimeOnly? timeSlot = null)
        {
            return new Appointment(patient ?? CreatePatient(), workSchedule ?? CreateWorkSchedule(), timeSlot ?? new TimeOnly(10, 0));
        }
    }
}

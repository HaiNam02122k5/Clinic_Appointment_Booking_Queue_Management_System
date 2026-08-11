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
            var newEmployee = new Employee(person ?? CreatePerson(), DateOnly.FromDateTime(DateTime.UtcNow));
            return newEmployee;
        }

        internal static Doctor CreateDoctor(Employee? employee = null, Specialty? specialty = null, string? licenseNumber = "ABC123", string? qualification = "MD", string? bio = "", int yoe = 0)
        {
            var doctor = new Doctor(employee ?? CreateEmployee(), licenseNumber, qualification, specialty ?? CreateSpecialty(), yoe, bio);
            return doctor;
        }
    }
}

using Clinic.Domain.Entities;

namespace Clinic.Domain.UnitTests
{
    public class ShiftRequestTests
    {
        [Fact]
        public void TestCreateShiftRequestValid()
        {
            var person = new Person("Full name", "00000000", "email@example.com", DateOnly.FromDateTime(DateTime.UtcNow), Enums.Gender.Male, "");
            var employee = new Employee(person, DateOnly.FromDateTime(DateTime.UtcNow));
            var doctor = new Doctor(employee, "123ABC", "Tien si", new Specialty("name", "", DateOnly.FromDateTime(DateTime.UtcNow)), 0);
            var shift = new ShiftRequest(doctor, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2), 5, "");
            Assert.Equal(doctor, shift.Doctor);
            Assert.Equal(5, shift.PatientLimitPerSlot);
        }

        [Fact]
        public void TestCreateShiftRequestInvalid()
        {
            var person = new Person("Full name", "00000000", "email@example.com", DateOnly.FromDateTime(DateTime.UtcNow), Enums.Gender.Male, "");
            var employee = new Employee(person, DateOnly.FromDateTime(DateTime.UtcNow));
            var doctor = new Doctor(employee, "123ABC", "Tien si", new Specialty("name", "", DateOnly.FromDateTime(DateTime.UtcNow)), 0);

            Assert.Throws<ArgumentException>(() => new ShiftRequest(doctor, DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(1), 5, ""));
            Assert.Throws<ArgumentException>(() => new ShiftRequest(doctor, DateTime.UtcNow.AddDays(-2), DateTime.UtcNow.AddDays(1), 5, ""));
            Assert.Throws<ArgumentNullException>(() => new ShiftRequest(null!, DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(3), 5, ""));
            Assert.Throws<ArgumentException>(() => new ShiftRequest(doctor, DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(3), -1, ""));
        }

        [Fact]
        public void TestUpdateShiftRequestInvalid()
        {
            var person = new Person("Full name", "00000000", "email@example.com", DateOnly.FromDateTime(DateTime.UtcNow), Enums.Gender.Male, "");
            var employee = new Employee(person, DateOnly.FromDateTime(DateTime.UtcNow));
            var doctor = new Doctor(employee, "123ABC", "Tien si", new Specialty("name", "", DateOnly.FromDateTime(DateTime.UtcNow)), 0);
            var shift = new ShiftRequest(doctor, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2), 5, "");

            Assert.Throws<ArgumentException>(() => shift.UpdateShift(DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(1), 5, ""));
            Assert.Throws<ArgumentException>(() => shift.UpdateShift(DateTime.UtcNow.AddDays(-2), DateTime.UtcNow.AddDays(1), 5, ""));
            Assert.Throws<ArgumentException>(() => shift.UpdateShift(DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(3), -1, ""));

            var nowShift = new ShiftRequest(doctor, DateTime.UtcNow.AddSeconds(1), DateTime.UtcNow.AddDays(1), 5, "");
            Thread.Sleep(1500);
            Assert.Throws<InvalidOperationException>(() => nowShift.UpdateShift(DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(3), 5, ""));

            Assert.Empty(doctor.WorkSchedules);
            shift.Approve();
            Assert.Single(doctor.WorkSchedules);

            Assert.Throws<InvalidOperationException>(() => shift.UpdateShift(DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(3), 5, ""));

            Assert.Throws<InvalidOperationException>(() => shift.Cancel());
            Assert.Throws<InvalidOperationException>(() => shift.Approve());
            Assert.Throws<InvalidOperationException>(() => shift.Reject());
        }
    }
}

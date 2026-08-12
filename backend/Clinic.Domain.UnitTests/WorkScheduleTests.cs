using Clinic.Domain.Entities;

namespace Clinic.Domain.UnitTests
{
    public class WorkScheduleTests
    {
        [Fact]
        public void TestCreateWorkScheduleValid()
        {
            var person = new Person("Full name", "00000000", "email@example.com", DateOnly.FromDateTime(DateTime.UtcNow), Enums.Gender.Male, "");
            var employee = new Employee(person, DateOnly.FromDateTime(DateTime.UtcNow));
            var doctor = new Doctor(employee, "123ABC", "Tien si", new Specialty("name", "", DateOnly.FromDateTime(DateTime.UtcNow)), 0);

            // Act & Assert
            var workSchedule = new WorkSchedule(doctor, DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2), 5);
            Assert.Equal(doctor, workSchedule.Doctor);
        }

        [Fact]
        public void TestCreateWorkScheduleInvalid()
        {
            var person = new Person("Full name", "00000000", "email@example.com", DateOnly.FromDateTime(DateTime.UtcNow), Enums.Gender.Male, "");
            var employee = new Employee(person, DateOnly.FromDateTime(DateTime.UtcNow));
            var doctor = new Doctor(employee, "123ABC", "Tien si", new Specialty("name", "", DateOnly.FromDateTime(DateTime.UtcNow)), 0);

            Assert.Throws<ArgumentException>(() => new WorkSchedule(doctor, DateTime.UtcNow.AddHours(2), DateTime.UtcNow.AddHours(1), 5));
            Assert.Throws<ArgumentException>(() => new WorkSchedule(doctor, DateTime.UtcNow.AddHours(-2), DateTime.UtcNow.AddHours(1), 5));
            Assert.Throws<ArgumentException>(() => new WorkSchedule(doctor, DateTime.UtcNow.AddHours(2), DateTime.UtcNow.AddHours(3), -1));
        }

        [Fact]
        public void TestUpdateWorkSchedule()
        {
            var person = new Person("Full name", "00000000", "email@example.com", DateOnly.FromDateTime(DateTime.UtcNow), Enums.Gender.Male, "");
            var employee = new Employee(person, DateOnly.FromDateTime(DateTime.UtcNow));
            var doctor = new Doctor(employee, "123ABC", "Tien si", new Specialty("name", "", DateOnly.FromDateTime(DateTime.UtcNow)), 0);

            var workSchedule = new WorkSchedule(doctor, DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2), 5);
            workSchedule.UpdateShift(DateTime.UtcNow.AddHours(3), DateTime.UtcNow.AddHours(4), 10);

            Assert.True(workSchedule.ShiftStart > DateTime.UtcNow.AddHours(2));
            Assert.True(workSchedule.ShiftEnd > DateTime.UtcNow.AddHours(3));
            Assert.Equal(10, workSchedule.PatientLimitPerSlot);
        }

        [Fact]
        public void TestUpdateWorkScheduleInvalid()
        {
            var person = new Person("Full name", "00000000", "email@example.com", DateOnly.FromDateTime(DateTime.UtcNow), Enums.Gender.Male, "");
            var employee = new Employee(person, DateOnly.FromDateTime(DateTime.UtcNow));
            var doctor = new Doctor(employee, "123ABC", "Tien si", new Specialty("name", "", DateOnly.FromDateTime(DateTime.UtcNow)), 0);
            var workSchedule = new WorkSchedule(doctor, DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2), 5);

            Assert.Throws<ArgumentException>(() => workSchedule.UpdateShift(DateTime.UtcNow.AddHours(2), DateTime.UtcNow.AddHours(1), 5));
            Assert.Throws<ArgumentException>(() => workSchedule.UpdateShift(DateTime.UtcNow.AddHours(-2), DateTime.UtcNow.AddHours(1), 5));
            Assert.Throws<ArgumentException>(() => workSchedule.UpdateShift(DateTime.UtcNow.AddHours(2), DateTime.UtcNow.AddHours(3), -1));

            var nowShift = new WorkSchedule(doctor, DateTime.UtcNow.AddSeconds(1), DateTime.UtcNow.AddHours(2), 5);
            Thread.Sleep(2000); // Wait for 2 seconds to ensure the shift has started
            Assert.Throws<InvalidOperationException>(() => nowShift.UpdateShift(DateTime.UtcNow.AddHours(2), DateTime.UtcNow.AddHours(3), 10));
            nowShift.UpdateShift(nowShift.ShiftStart, nowShift.ShiftEnd.AddHours(1), 15); // This should be valid since we are not changing the start time
            Assert.Equal(15, nowShift.PatientLimitPerSlot);
            Assert.Throws<InvalidOperationException>(() => nowShift.Delete());
        }

        [Fact]
        public void TestDeleteWorkSchedule()
        {
            var person = new Person("Full name", "00000000", "email@example.com", DateOnly.FromDateTime(DateTime.UtcNow), Enums.Gender.Male, "");
            var employee = new Employee(person, DateOnly.FromDateTime(DateTime.UtcNow));
            var doctor = new Doctor(employee, "123ABC", "Tien si", new Specialty("name", "", DateOnly.FromDateTime(DateTime.UtcNow)), 0);
            var workSchedule = new WorkSchedule(doctor, DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2), 5);
            workSchedule.Delete();
            Assert.True(workSchedule.IsDeleted);

            var nowShift = new WorkSchedule(doctor, DateTime.UtcNow.AddSeconds(1), DateTime.UtcNow.AddHours(2), 5);
            Thread.Sleep(2000); // Wait for 2 seconds to ensure the shift has started
            Assert.Throws<InvalidOperationException>(() => nowShift.Delete());
        }
    }
}

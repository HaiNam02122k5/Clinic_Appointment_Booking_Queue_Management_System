using Clinic.Domain.Entities;

namespace Clinic.Domain.UnitTests
{
    public class WorkScheduleTests
    {
        [Fact]
        public void TestCreateWorkScheduleValid()
        {
            var now = DateTime.UtcNow.AddHours(7);
            var person = new Person("Full name", "00000000", "email@example.com", DateOnly.FromDateTime(now), Enums.Gender.Male, "");
            var employee = new Employee(person, DateOnly.FromDateTime(now));
            var doctor = new Doctor(employee, "123ABC", "Tien si", new Specialty("name", "", DateOnly.FromDateTime(now)), 0);

            // Act & Assert
            var workSchedule = new WorkSchedule(doctor, DateOnly.FromDateTime(now), TimeOnly.FromDateTime(now.AddHours(1)), TimeOnly.FromDateTime(now.AddHours(2)), 5);
            Assert.Equal(doctor, workSchedule.Doctor);
        }

        [Fact]
        public void TestCreateWorkScheduleInvalid()
        {
            var now = DateTime.UtcNow.AddHours(7);
            var person = new Person("Full name", "00000000", "email@example.com", DateOnly.FromDateTime(now), Enums.Gender.Male, "");
            var employee = new Employee(person, DateOnly.FromDateTime(now));
            var doctor = new Doctor(employee, "123ABC", "Tien si", new Specialty("name", "", DateOnly.FromDateTime(now)), 0);

            Assert.Throws<ArgumentException>(() => new WorkSchedule(doctor, DateOnly.FromDateTime(now), TimeOnly.FromDateTime(now.AddHours(2)), TimeOnly.FromDateTime(now.AddHours(1)), 5));
            Assert.Throws<ArgumentException>(() => new WorkSchedule(doctor, DateOnly.FromDateTime(now), TimeOnly.FromDateTime(now.AddHours(-1)), TimeOnly.FromDateTime(now.AddHours(2)), 5));
            Assert.Throws<ArgumentException>(() => new WorkSchedule(doctor, DateOnly.FromDateTime(now), TimeOnly.FromDateTime(now.AddHours(2)), TimeOnly.FromDateTime(now.AddHours(3)), -1));
        }

        [Fact]
        public void TestUpdateWorkSchedule()
        {
            var now = DateTime.UtcNow.AddHours(7);
            var person = new Person("Full name", "00000000", "email@example.com", DateOnly.FromDateTime(now), Enums.Gender.Male, "");
            var employee = new Employee(person, DateOnly.FromDateTime(now));
            var doctor = new Doctor(employee, "123ABC", "Tien si", new Specialty("name", "", DateOnly.FromDateTime(now)), 0);

            var workSchedule = new WorkSchedule(doctor, DateOnly.FromDateTime(now), TimeOnly.FromDateTime(now.AddHours(1)), TimeOnly.FromDateTime(now.AddHours(2)), 5);
            workSchedule.UpdateShift(DateOnly.FromDateTime(now), TimeOnly.FromDateTime(now.AddHours(3)), TimeOnly.FromDateTime(now.AddHours(4)), 10);

            Assert.True(workSchedule.ShiftStart > TimeOnly.FromDateTime(now.AddHours(2)));
            Assert.True(workSchedule.ShiftEnd > TimeOnly.FromDateTime(now.AddHours(3)));
            Assert.Equal(10, workSchedule.PatientLimit);
        }

        [Fact]
        public void TestUpdateWorkScheduleInvalid()
        {
            var now = DateTime.UtcNow.AddHours(7);
            var person = new Person("Full name", "00000000", "email@example.com", DateOnly.FromDateTime(now), Enums.Gender.Male, "");
            var employee = new Employee(person, DateOnly.FromDateTime(now));
            var doctor = new Doctor(employee, "123ABC", "Tien si", new Specialty("name", "", DateOnly.FromDateTime(now)), 0);
            var workSchedule = new WorkSchedule(doctor, DateOnly.FromDateTime(now), TimeOnly.FromDateTime(now.AddHours(1)), TimeOnly.FromDateTime(now.AddHours(2)), 5);

            Assert.Throws<ArgumentException>(() => workSchedule.UpdateShift(DateOnly.FromDateTime(now), TimeOnly.FromDateTime(now.AddHours(2)), TimeOnly.FromDateTime(now.AddHours(1)), 5));
            Assert.Throws<ArgumentException>(() => workSchedule.UpdateShift(DateOnly.FromDateTime(now), TimeOnly.FromDateTime(now.AddHours(-2)), TimeOnly.FromDateTime(now.AddHours(1)), 5));
            Assert.Throws<ArgumentException>(() => workSchedule.UpdateShift(DateOnly.FromDateTime(now), TimeOnly.FromDateTime(now.AddHours(2)), TimeOnly.FromDateTime(now.AddHours(3)), -1));

            var nowShift = new WorkSchedule(doctor, DateOnly.FromDateTime(now), TimeOnly.FromDateTime(now.AddSeconds(1)), TimeOnly.FromDateTime(now.AddHours(2)), 5);
            Thread.Sleep(2000); // Wait for 2 seconds to ensure the shift has started
            Assert.Throws<InvalidOperationException>(() => nowShift.UpdateShift(DateOnly.FromDateTime(now), TimeOnly.FromDateTime(now.AddHours(2)), TimeOnly.FromDateTime(now.AddHours(3)), 10));
            nowShift.UpdateShift(DateOnly.FromDateTime(now), nowShift.ShiftStart, nowShift.ShiftEnd.AddHours(1), 15); // This should be valid since we are not changing the start time
            Assert.Equal(15, nowShift.PatientLimit);
            Assert.Throws<InvalidOperationException>(() => nowShift.Delete());
        }

        [Fact]
        public void TestDeleteWorkSchedule()
        {
            var now = DateTime.UtcNow.AddHours(7);
            var person = new Person("Full name", "00000000", "email@example.com", DateOnly.FromDateTime(DateTime.UtcNow), Enums.Gender.Male, "");
            var employee = new Employee(person, DateOnly.FromDateTime(DateTime.UtcNow));
            var doctor = new Doctor(employee, "123ABC", "Tien si", new Specialty("name", "", DateOnly.FromDateTime(DateTime.UtcNow)), 0);
            var workSchedule = new WorkSchedule(doctor, DateOnly.FromDateTime(now), TimeOnly.FromDateTime(now.AddHours(1)), TimeOnly.FromDateTime(now.AddHours(2)), 5);
            workSchedule.Delete();
            Assert.True(workSchedule.IsDeleted);

            var nowShift = new WorkSchedule(doctor, DateOnly.FromDateTime(now), TimeOnly.FromDateTime(now.AddSeconds(1)), TimeOnly.FromDateTime(now.AddHours(2)), 5);
            Thread.Sleep(2000); // Wait for 2 seconds to ensure the shift has started
            Assert.Throws<InvalidOperationException>(() => nowShift.Delete());
        }
    }
}

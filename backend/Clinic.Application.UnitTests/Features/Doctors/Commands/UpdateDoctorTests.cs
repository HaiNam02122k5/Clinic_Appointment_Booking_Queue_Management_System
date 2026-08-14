using Clinic.Application.Common.Exceptions;
using Clinic.Application.Features.Doctors.Commands;
using Clinic.Application.UnitTests.Common;
using Clinic.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.Application.UnitTests.Features.Doctors.Commands
{
    public class UpdateDoctorTests
    {
        [Fact]
        public async Task UpdateDoctor_ShouldUpdateDoctorInfo()
        {
            // Arrange
            var doctorRepository = new FakeDoctorRepository();
            var unitOfWork = new FakeUnitOfWork();
            var userRepo = new FakeUserRepository();
            var handler = new UpdateDoctorCommandHandler(doctorRepository, userRepo, unitOfWork);
            var doctorPerson = TestDataFactory.CreatePerson(fullName: "Dr. John Doe");
            var user = TestDataFactory.CreateUser(person: doctorPerson);
            user.AssignRole(TestDataFactory.RoleSet.First(r => r.Name == "Doctor"));
            var doctor = TestDataFactory.CreateDoctor(employee: TestDataFactory.CreateEmployee(person: doctorPerson));
            var admin = TestDataFactory.CreateUser(username: "admin", person: TestDataFactory.CreatePerson(fullName: "Admin User"));
            admin.AssignRole(TestDataFactory.RoleSet.First(r => r.Name == "Admin"));
            await userRepo.AddAsync(user);
            await userRepo.AddAsync(admin);
            await doctorRepository.AddAsync(doctor);
            var command = new UpdateDoctorCommand(
                admin.Id,
                "Updated Name",
                "1234567890",
                "abc@gmail.com",
                new DateOnly(1990, 1, 1),
                Domain.Enums.Gender.Female,
                "AB C123 st",
                "ABC456",
                "MD",
                10,
                "Updated biography",
                doctor.Id
            );
            await handler.Handle(command, CancellationToken.None);
            Assert.Equal("Updated biography", doctor.Biography);
            Assert.Equal("Updated Name", doctor.Employee.Person.FullName);
            
            // self update
            var command2 = new UpdateDoctorCommand(
                user.Id,
                "Updated Name",
                "1234567890",
                "abc@gmail.com",
                new DateOnly(1990, 1, 1),
                Domain.Enums.Gender.Female,
                "AB C123 st",
                "ABC456",
                "MD",
                15,
                "Updated biography"
            );
            await handler.Handle(command2, CancellationToken.None);
            Assert.Equal(15, doctor.ExperienceYears);
        }

        [Fact]
        public async Task UpdateDoctor_ShouldThrowException_WhenDoctorNotFound()
        {
            // Arrange
            var doctorRepository = new FakeDoctorRepository();
            var unitOfWork = new FakeUnitOfWork();
            var userRepo = new FakeUserRepository();
            var handler = new UpdateDoctorCommandHandler(doctorRepository, userRepo, unitOfWork);
            var doctorPerson = TestDataFactory.CreatePerson(fullName: "Dr. John Doe");
            var user = TestDataFactory.CreateUser(person: doctorPerson);
            user.AssignRole(TestDataFactory.RoleSet.First(r => r.Name == "Doctor"));
            var doctor = TestDataFactory.CreateDoctor(employee: TestDataFactory.CreateEmployee(person: doctorPerson));
            var admin = TestDataFactory.CreateUser(username: "admin", person: TestDataFactory.CreatePerson(fullName: "Admin User"));
            admin.AssignRole(TestDataFactory.RoleSet.First(r => r.Name == "Admin"));
            await userRepo.AddAsync(user);
            await userRepo.AddAsync(admin);
            await doctorRepository.AddAsync(doctor);
            var command = new UpdateDoctorCommand(
                admin.Id,
                "Updated Name",
                "1234567890",
                "abc@gmail.com",
                new DateOnly(1990, 1, 1),
                Domain.Enums.Gender.Female,
                "AB C123 st",
                "ABC456",
                "MD",
                10,
                "Updated biography",
                Guid.NewGuid()
            );
            await Assert.ThrowsAsync<NotFoundException>(async () => await handler.Handle(command, CancellationToken.None));

            // self update, admin not a doctor
            var command2 = new UpdateDoctorCommand(
                admin.Id,
                "Updated Name",
                "1234567890",
                "abc@gmail.com",
                new DateOnly(1990, 1, 1),
                Domain.Enums.Gender.Female,
                "AB C123 st",
                "ABC456",
                "MD",
                10,
                "Updated biography"
            );
            await Assert.ThrowsAsync<NotFoundException>(async () => await handler.Handle(command2, CancellationToken.None));
        }

        [Fact]
        public async Task UpdateDoctor_ShouldThrowException_WhenUserNotAuthorized()
        {
            // Arrange
            var doctorRepository = new FakeDoctorRepository();
            var unitOfWork = new FakeUnitOfWork();
            var userRepo = new FakeUserRepository();
            var handler = new UpdateDoctorCommandHandler(doctorRepository, userRepo, unitOfWork);
            var doctorPerson = TestDataFactory.CreatePerson(fullName: "Dr. John Doe");
            var user = TestDataFactory.CreateUser(person: doctorPerson);
            user.AssignRole(TestDataFactory.RoleSet.First(r => r.Name == "Doctor"));
            var doctor = TestDataFactory.CreateDoctor(employee: TestDataFactory.CreateEmployee(person: doctorPerson));
            var admin = TestDataFactory.CreateUser(username: "admin", person: TestDataFactory.CreatePerson(fullName: "Admin User"));
            admin.AssignRole(TestDataFactory.RoleSet.First(r => r.Name == "Admin"));
            await userRepo.AddAsync(user);
            await userRepo.AddAsync(admin);
            await doctorRepository.AddAsync(doctor);
            var command = new UpdateDoctorCommand(
                null,
                "Updated Name",
                "1234567890",
                "abc@gmail.com",
                new DateOnly(1990, 1, 1),
                Domain.Enums.Gender.Female,
                "AB C123 st",
                "ABC456",
                "MD",
                10,
                "Updated biography"
            );
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await handler.Handle(command, CancellationToken.None));
        }
    }
}

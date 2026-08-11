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
            var handler = new UpdateDoctorCommandHandler(doctorRepository, unitOfWork);
            var doctor = TestDataFactory.CreateDoctor();
            await doctorRepository.AddAsync(doctor);
            var command = new UpdateDoctorCommand(
                doctor.Id,
                "Updated Name",
                "1234567890",
                "abc@gmail.com",
                new DateOnly(1990, 1, 1),
                Domain.Enums.Gender.Female,
                "AB C123 st",
                "ABC456",
                "MD",
                Domain.Enums.DoctorStatus.Active,
                10,
                "Updated biography"
            );
            await handler.Handle(command, CancellationToken.None);
            Assert.Equal("Updated biography", doctor.Biography);
            Assert.Equal("Updated Name", doctor.Employee.Person.FullName);
        }

        [Fact]
        public async Task UpdateDoctor_ShouldThrowException_WhenDoctorNotFound()
        {
            // Arrange
            var doctorRepository = new FakeDoctorRepository();
            var unitOfWork = new FakeUnitOfWork();
            var handler = new UpdateDoctorCommandHandler(doctorRepository, unitOfWork);
            var command = new UpdateDoctorCommand(
                Guid.NewGuid(),
                "Updated Name",
                "1234567890",
                "abc@gmail.com",
                new DateOnly(1990, 1, 1),
                Domain.Enums.Gender.Female,
                "AB C123 st",
                "ABC456",
                "MD",
                Domain.Enums.DoctorStatus.Active,
                10,
                "Updated biography"
            );
            await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
        }
    }
}

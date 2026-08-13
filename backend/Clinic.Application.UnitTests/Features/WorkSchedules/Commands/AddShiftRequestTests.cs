using Clinic.Application.Common.Exceptions;
using Clinic.Application.Features.WorkSchedules.Commands;
using Clinic.Application.UnitTests.Common;
using Clinic.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.Application.UnitTests.Features.WorkSchedules.Commands
{
    public class AddShiftRequestTests
    {
        [Fact]
        public async Task TestAddDoctorShiftRequest()
        {
            var doctorRepository = new FakeDoctorRepository();
            var workScheduleRepository = new FakeWorkScheduleRepository();
            var unitOfWork = new FakeUnitOfWork();
            var handler = new AddDoctorShiftRequestCommandHandler(doctorRepository, workScheduleRepository, unitOfWork);
            var doctor = TestDataFactory.CreateDoctor();
            var now = DateTime.UtcNow.AddHours(7);
            await doctorRepository.AddAsync(doctor);
            var schedule1 = new ShiftRequest(doctor, DateOnly.FromDateTime(now.AddDays(1)), TimeOnly.FromDateTime(now.AddDays(1)), TimeOnly.FromDateTime(now.AddDays(1).AddHours(1)), 5, "");
            doctor.AddShiftRequest(schedule1);
            await workScheduleRepository.AddShiftRequestAsync(schedule1);

            var command = new AddDoctorShiftRequestCommand(
                doctor.Id, DateOnly.FromDateTime(now.AddDays(2)), TimeOnly.FromDateTime(now.AddDays(2)), TimeOnly.FromDateTime(now.AddDays(2).AddHours(1)), 5, "");
            var result = await handler.Handle(command, CancellationToken.None);

            Assert.NotNull(workScheduleRepository.GetShiftRequestByIdAsync(result.Id));
            Assert.Equal(2, doctor.ShiftRequests.Count);

            // Test duplicate shift request
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                var overlappingCommand = new AddDoctorShiftRequestCommand(
                    doctor.Id, DateOnly.FromDateTime(now.AddDays(1)), TimeOnly.FromDateTime(now.AddDays(1)), TimeOnly.FromDateTime(now.AddDays(1).AddHours(1)), 5, "");
                await handler.Handle(overlappingCommand, CancellationToken.None);
            });
        }

        [Fact]
        public async Task TestAddNonExistentDoctorSchedule()
        {
            var doctorRepository = new FakeDoctorRepository();
            var workScheduleRepository = new FakeWorkScheduleRepository();
            var unitOfWork = new FakeUnitOfWork();
            var handler = new AddDoctorShiftRequestCommandHandler(doctorRepository, workScheduleRepository, unitOfWork);
            var now = DateTime.UtcNow.AddHours(7);
            var command = new AddDoctorShiftRequestCommand(
                Guid.NewGuid(), DateOnly.FromDateTime(now.AddDays(2)), TimeOnly.FromDateTime(now.AddDays(2)), TimeOnly.FromDateTime(now.AddDays(2).AddHours(1)), 5, "");
            await Assert.ThrowsAsync<NotFoundException>(async () =>
            {
                await handler.Handle(command, CancellationToken.None);
            });
        }
    }
}

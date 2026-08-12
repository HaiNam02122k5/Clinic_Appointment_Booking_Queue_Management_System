using Clinic.Application.Common.Exceptions;
using Clinic.Application.Features.WorkSchedules.Commands;
using Clinic.Application.UnitTests.Common;
using Clinic.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.Application.UnitTests.Features.WorkSchedules.Commands
{
    public class UpdateScheduleTests
    {
        [Fact]
        public async Task TestUpdateWorkSchedule()
        {
            // Arrange
            var workScheduleRepository = new FakeWorkScheduleRepository();
            var unitOfWork = new FakeUnitOfWork();
            var handler = new UpdateWorkScheduleCommandHandler(workScheduleRepository, unitOfWork);
            var doctor = TestDataFactory.CreateDoctor();
            var workSchedule = new WorkSchedule(doctor, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(1).AddHours(1), 5);
            await workScheduleRepository.AddWorkScheduleAsync(workSchedule);

            // Act
            var command = new UpdateWorkScheduleCommand(workSchedule.Id, DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(2).AddHours(1), 10);
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(workSchedule.Id, result);
            Assert.Equal(10, workSchedule.PatientLimit);
        }

        [Fact]
        public async Task TestUpdateNonExistingSchedule()
        {
            // Arrange
            var workScheduleRepository = new FakeWorkScheduleRepository();
            var unitOfWork = new FakeUnitOfWork();
            var handler = new UpdateWorkScheduleCommandHandler(workScheduleRepository, unitOfWork);
            var doctor = TestDataFactory.CreateDoctor();
            var workSchedule = new WorkSchedule(doctor, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(1).AddHours(1), 5);
            await workScheduleRepository.AddWorkScheduleAsync(workSchedule);

            // Act
            var command = new UpdateWorkScheduleCommand(Guid.NewGuid(), DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(2).AddHours(1), 10);
            await Assert.ThrowsAsync<NotFoundException>(async () =>
            {
                await handler.Handle(command, CancellationToken.None);
            });

            workSchedule.Delete(); // Mark the work schedule as deleted
            await Assert.ThrowsAsync<NotFoundException>(async () =>
            {
                await handler.Handle(new UpdateWorkScheduleCommand(workSchedule.Id, DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(2).AddHours(1), 10), CancellationToken.None);
            });
        }
    }
}

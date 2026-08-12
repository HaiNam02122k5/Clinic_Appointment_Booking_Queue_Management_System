using Clinic.Application.Common.Exceptions;
using Clinic.Application.Features.WorkSchedules.Commands;
using Clinic.Application.UnitTests.Common;
using Clinic.Domain.Entities;
using Clinic.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.Application.UnitTests.Features.WorkSchedules.Commands
{
    public class CancelScheduleTests
    {
        [Fact]
        public async Task TestCancelWorkSchedule()
        {
            // Arrange
            var workScheduleRepository = new FakeWorkScheduleRepository();
            var unitOfWork = new FakeUnitOfWork();
            var handler = new CancelWorkScheduleCommandHandler(workScheduleRepository, unitOfWork);
            var doctor = TestDataFactory.CreateDoctor();
            var workSchedule = new WorkSchedule(doctor, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(1).AddHours(1), 5);
            await workScheduleRepository.AddWorkScheduleAsync(workSchedule);

            // Act
            var command = new CancelWorkScheduleCommand(workSchedule.Id, "Cancellation reason");
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(workSchedule.Id, result);
            Assert.True(workSchedule.Status == WorkScheduleStatus.Cancelled);
        }

        [Fact]
        public async Task TestCancelNonExistingSchedule()
        {
            // Arrange
            var workScheduleRepository = new FakeWorkScheduleRepository();
            var unitOfWork = new FakeUnitOfWork();
            var handler = new CancelWorkScheduleCommandHandler(workScheduleRepository, unitOfWork);
            var doctor = TestDataFactory.CreateDoctor();
            var workSchedule = new WorkSchedule(doctor, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(1).AddHours(1), 5);
            await workScheduleRepository.AddWorkScheduleAsync(workSchedule);

            // Act
            var command = new CancelWorkScheduleCommand(Guid.NewGuid(), "Cancellation reason");
            await Assert.ThrowsAsync<NotFoundException>(async () =>
            {
                await handler.Handle(command, CancellationToken.None);
            });

            workSchedule.Delete(); // Mark the work schedule as deleted
            await Assert.ThrowsAsync<NotFoundException>(async () =>
            {
                await handler.Handle(new CancelWorkScheduleCommand(workSchedule.Id, "Cancellation reason"), CancellationToken.None);
            });
        }
    }
}

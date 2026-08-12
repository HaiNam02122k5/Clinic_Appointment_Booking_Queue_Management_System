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
    public class UpdateRequestTests
    {
        [Fact]
        public async Task TestUpdateRequest()
        {
            // Arrange
            var workScheduleRepository = new FakeWorkScheduleRepository();
            var unitOfWork = new FakeUnitOfWork();
            var handler = new UpdateShiftRequestCommandHandler(workScheduleRepository, unitOfWork);
            var doctor = TestDataFactory.CreateDoctor();
            var shiftRequest = new ShiftRequest(doctor, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(1).AddHours(1), 5, "");
            await workScheduleRepository.AddShiftRequestAsync(shiftRequest);

            // Act
            var command = new UpdateShiftRequestCommand(shiftRequest.Id, doctor.Id, DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(2).AddHours(1), 10, "");
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(shiftRequest.Id, result);
            Assert.Equal(10, shiftRequest.PatientLimit);
            Assert.True(shiftRequest.ShiftStart > DateTime.UtcNow.AddDays(1));
        }

        [Fact]
        public async Task TestUpdateNonExistingRequest()
        {
            // Arrange
            var workScheduleRepository = new FakeWorkScheduleRepository();
            var unitOfWork = new FakeUnitOfWork();
            var handler = new UpdateShiftRequestCommandHandler(workScheduleRepository, unitOfWork);
            var doctor = TestDataFactory.CreateDoctor();
            var shiftRequest = new ShiftRequest(doctor, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(1).AddHours(1), 5, "");
            await workScheduleRepository.AddShiftRequestAsync(shiftRequest);

            // Update non-existing request
            var command = new UpdateShiftRequestCommand(Guid.NewGuid(), doctor.Id, DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(2).AddHours(1), 10, "");
            await Assert.ThrowsAsync<NotFoundException>(async () =>
            {
                await handler.Handle(command, CancellationToken.None);
            });
        }

        [Fact]
        public async Task TestUpdateUnauthorizedRequest()
        {
            // Arrange
            var workScheduleRepository = new FakeWorkScheduleRepository();
            var unitOfWork = new FakeUnitOfWork();
            var handler = new UpdateShiftRequestCommandHandler(workScheduleRepository, unitOfWork);
            var doctor = TestDataFactory.CreateDoctor();
            var doctor2 = TestDataFactory.CreateDoctor();
            var shiftRequest = new ShiftRequest(doctor, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(1).AddHours(1), 5, "");
            await workScheduleRepository.AddShiftRequestAsync(shiftRequest);

            // Update unauthorized request
            var command = new UpdateShiftRequestCommand(shiftRequest.Id, doctor2.Id, DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(2).AddHours(1), 10, "");
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            {
                await handler.Handle(command, CancellationToken.None);
            });
        }
    }
}

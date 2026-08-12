using Clinic.Application.Common.Exceptions;
using Clinic.Application.Features.WorkSchedules.Commands;
using Clinic.Application.UnitTests.Common;
using Clinic.Domain.Common.Exceptions;
using Clinic.Domain.Entities;
using Clinic.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.Application.UnitTests.Features.WorkSchedules.Commands
{
    public class CancelRequestTests
    {
        [Fact]
        public async Task TestCancelRequest()
        {
            // Arrange
            var workScheduleRepository = new FakeWorkScheduleRepository();
            var unitOfWork = new FakeUnitOfWork();
            var handler = new CancelRequestCommandHandler(workScheduleRepository, unitOfWork);
            var doctor = TestDataFactory.CreateDoctor();
            var now = DateTime.UtcNow;
            var shiftRequest = new ShiftRequest(doctor, now.AddDays(1), now.AddDays(1).AddHours(1), 5, "");
            await workScheduleRepository.AddShiftRequestAsync(shiftRequest);

            // Act
            var command = new CancelRequestCommand(doctor.Id, shiftRequest.Id);
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(shiftRequest.Id, result);
            Assert.Equal(ShiftRequestStatus.Cancelled, shiftRequest.Status);

            // Cancel already cancelled request
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await handler.Handle(command, CancellationToken.None);
            });
        }

        [Fact]
        public async Task TestCancelNonExistingRequest()
        {
            // Arrange
            var workScheduleRepository = new FakeWorkScheduleRepository();
            var unitOfWork = new FakeUnitOfWork();
            var handler = new CancelRequestCommandHandler(workScheduleRepository, unitOfWork);
            var doctor = TestDataFactory.CreateDoctor();
            var shiftRequest = new ShiftRequest(doctor, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(1).AddHours(1), 5, "");
            await workScheduleRepository.AddShiftRequestAsync(shiftRequest);

            // Cancel non-existing request
            var command = new CancelRequestCommand(doctor.Id, Guid.NewGuid());
            await Assert.ThrowsAsync<NotFoundException>(async () =>
            {
                await handler.Handle(command, CancellationToken.None);
            });
        }

        [Fact]
        public async Task TestCancelUnauthorizedRequest()
        {
            // Arrange
            var workScheduleRepository = new FakeWorkScheduleRepository();
            var unitOfWork = new FakeUnitOfWork();
            var handler = new CancelRequestCommandHandler(workScheduleRepository, unitOfWork);
            var doctor = TestDataFactory.CreateDoctor();
            var doctor2 = TestDataFactory.CreateDoctor();
            var shiftRequest = new ShiftRequest(doctor, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(1).AddHours(1), 5, "");
            await workScheduleRepository.AddShiftRequestAsync(shiftRequest);

            // Cancel unauthorized request
            var command = new CancelRequestCommand(doctor2.Id, shiftRequest.Id);
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            {
                await handler.Handle(command, CancellationToken.None);
            });
        }
    }
}

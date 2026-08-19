using Clinic.Application.Common.Exceptions;
using Clinic.Application.Features.Appointments.Commands;
using Clinic.Application.UnitTests.Common;
using Clinic.Domain.Common.Exceptions;
using Clinic.Domain.Entities;
using Clinic.Domain.Enums;

namespace Clinic.Application.UnitTests.Features.Appointments.Commands
{
    public class CreateAppointmentTests
    {
        [Fact]
        public async Task Handle_ValidRequest_ShouldCreateAppointmentAndReturnId()
        {
            // Arrange
            var workScheduleRepository = new FakeWorkScheduleRepository();
            var appointmentRepository = new FakeAppointmentRepository();
            var currentUser = new FakeCurrentUser();
            var unitOfWork = new FakeUnitOfWork();
            var handler = new CreateAppointmentHandler(workScheduleRepository, appointmentRepository, currentUser, unitOfWork);

            var patientId = Guid.NewGuid();
            currentUser.PatientId = patientId;

            var workSchedule = TestDataFactory.CreateWorkSchedule();
            workScheduleRepository.Add(workSchedule);

            var timeSlot = workSchedule.ShiftStart.AddMinutes(30);
            var command = new CreateAppointmentCommand(workSchedule.Id, timeSlot, "Routine checkup");

            // Act
            var appointmentId = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotEqual(Guid.Empty, appointmentId);
            Assert.True(unitOfWork.WasCommitted);
            var created = await appointmentRepository.GetByIdAsync(appointmentId);
            Assert.NotNull(created);
            Assert.Equal(patientId, created.PatientId);
            Assert.Equal(workSchedule.Id, created.WorkScheduleId);
            Assert.Equal(timeSlot, created.TimeSlot);
            Assert.Equal(AppointmentStatus.Pending, created.Status);
        }

        [Fact]
        public async Task Handle_UserNotPatient_ShouldThrowForbiddenException()
        {
            // Arrange
            var workScheduleRepository = new FakeWorkScheduleRepository();
            var appointmentRepository = new FakeAppointmentRepository();
            var currentUser = new FakeCurrentUser { PatientId = null };
            var unitOfWork = new FakeUnitOfWork();
            var handler = new CreateAppointmentHandler(workScheduleRepository, appointmentRepository, currentUser, unitOfWork);

            var workSchedule = TestDataFactory.CreateWorkSchedule();
            workScheduleRepository.Add(workSchedule);

            var command = new CreateAppointmentCommand(workSchedule.Id, workSchedule.ShiftStart.AddMinutes(30), null);

            // Act & Assert
            await Assert.ThrowsAsync<ForbiddenException>(() => handler.Handle(command, CancellationToken.None));
            Assert.False(unitOfWork.WasCommitted);
        }

        [Fact]
        public async Task Handle_WorkScheduleNotFound_ShouldThrowNotFoundExceptionAndRollback()
        {
            // Arrange
            var workScheduleRepository = new FakeWorkScheduleRepository();
            var appointmentRepository = new FakeAppointmentRepository();
            var currentUser = new FakeCurrentUser { PatientId = Guid.NewGuid() };
            var unitOfWork = new FakeUnitOfWork();
            var handler = new CreateAppointmentHandler(workScheduleRepository, appointmentRepository, currentUser, unitOfWork);

            var nonExistentScheduleId = Guid.NewGuid();
            var command = new CreateAppointmentCommand(nonExistentScheduleId, DateTime.UtcNow.AddDays(1), null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
            Assert.True(unitOfWork.WasRolledBack);
        }

        [Fact]
        public async Task Handle_WorkScheduleNotActive_ShouldThrowConflictException()
        {
            // Arrange
            var workScheduleRepository = new FakeWorkScheduleRepository();
            var appointmentRepository = new FakeAppointmentRepository();
            var currentUser = new FakeCurrentUser { PatientId = Guid.NewGuid() };
            var unitOfWork = new FakeUnitOfWork();
            var handler = new CreateAppointmentHandler(workScheduleRepository, appointmentRepository, currentUser, unitOfWork);

            var workSchedule = TestDataFactory.CreateWorkSchedule();
            workSchedule.Cancel("Doctor on leave");
            workScheduleRepository.Add(workSchedule);

            var command = new CreateAppointmentCommand(workSchedule.Id, workSchedule.ShiftStart.AddMinutes(30), null);

            // Act & Assert
            await Assert.ThrowsAsync<ConflictException>(() => handler.Handle(command, CancellationToken.None));
            Assert.True(unitOfWork.WasRolledBack);
        }

        [Fact]
        public async Task Handle_TimeSlotOutsideShift_ShouldThrowArgumentException()
        {
            // Arrange
            var workScheduleRepository = new FakeWorkScheduleRepository();
            var appointmentRepository = new FakeAppointmentRepository();
            var currentUser = new FakeCurrentUser { PatientId = Guid.NewGuid() };
            var unitOfWork = new FakeUnitOfWork();
            var handler = new CreateAppointmentHandler(workScheduleRepository, appointmentRepository, currentUser, unitOfWork);

            var workSchedule = TestDataFactory.CreateWorkSchedule();
            workScheduleRepository.Add(workSchedule);

            // Slot time before shift starts
            var invalidTimeSlot = workSchedule.ShiftStart.AddHours(-1);
            var command = new CreateAppointmentCommand(workSchedule.Id, invalidTimeSlot, null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(command, CancellationToken.None));
            Assert.True(unitOfWork.WasRolledBack);
        }

        [Fact]
        public async Task Handle_TimeSlotInPast_ShouldThrowArgumentException()
        {
            // Arrange
            var workScheduleRepository = new FakeWorkScheduleRepository();
            var appointmentRepository = new FakeAppointmentRepository();
            var currentUser = new FakeCurrentUser { PatientId = Guid.NewGuid() };
            var unitOfWork = new FakeUnitOfWork();
            var handler = new CreateAppointmentHandler(workScheduleRepository, appointmentRepository, currentUser, unitOfWork);

            // Reconstruct a schedule spanning past to future
            var workSchedule = new WorkSchedule(
                id: Guid.NewGuid(),
                doctorId: Guid.NewGuid(),
                shiftStart: DateTime.UtcNow.AddHours(-3),
                shiftEnd: DateTime.UtcNow.AddHours(3),
                patientLimitPerSlot: 5,
                status: WorkScheduleStatus.Active,
                createdAt: DateTime.UtcNow.AddDays(-1),
                updatedAt: null,
                isDeleted: false
            );
            workScheduleRepository.Add(workSchedule);

            var pastTimeSlotWithinShift = DateTime.UtcNow.AddHours(-1);
            var command = new CreateAppointmentCommand(workSchedule.Id, pastTimeSlotWithinShift, null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(command, CancellationToken.None));
            Assert.True(unitOfWork.WasRolledBack);
        }

        [Fact]
        public async Task Handle_SlotFullyBooked_ShouldThrowConflictException()
        {
            // Arrange
            var workScheduleRepository = new FakeWorkScheduleRepository();
            var appointmentRepository = new FakeAppointmentRepository();
            var currentUser = new FakeCurrentUser { PatientId = Guid.NewGuid() };
            var unitOfWork = new FakeUnitOfWork();
            var handler = new CreateAppointmentHandler(workScheduleRepository, appointmentRepository, currentUser, unitOfWork);

            var workSchedule = TestDataFactory.CreateWorkSchedule(limit: 2);
            workScheduleRepository.Add(workSchedule);

            // Fill capacity
            var app1 = new Appointment { PatientId = Guid.NewGuid(), WorkScheduleId = workSchedule.Id, WorkSchedule = workSchedule, TimeSlot = workSchedule.ShiftStart };
            var app2 = new Appointment { PatientId = Guid.NewGuid(), WorkScheduleId = workSchedule.Id, WorkSchedule = workSchedule, TimeSlot = workSchedule.ShiftStart };
            workSchedule.Appointments.Add(app1);
            workSchedule.Appointments.Add(app2);

            var command = new CreateAppointmentCommand(workSchedule.Id, workSchedule.ShiftStart.AddMinutes(30), null);

            // Act & Assert
            await Assert.ThrowsAsync<ConflictException>(() => handler.Handle(command, CancellationToken.None));
            Assert.True(unitOfWork.WasRolledBack);
        }

        [Fact]
        public async Task Handle_CancelledAppointmentsDoNotCountTowardsLimit_ShouldSucceed()
        {
            // Arrange
            var workScheduleRepository = new FakeWorkScheduleRepository();
            var appointmentRepository = new FakeAppointmentRepository();
            var currentUser = new FakeCurrentUser { PatientId = Guid.NewGuid() };
            var unitOfWork = new FakeUnitOfWork();
            var handler = new CreateAppointmentHandler(workScheduleRepository, appointmentRepository, currentUser, unitOfWork);

            var workSchedule = TestDataFactory.CreateWorkSchedule(limit: 1);
            workScheduleRepository.Add(workSchedule);

            // One cancelled appointment
            var cancelledApp = new Appointment
            {
                PatientId = Guid.NewGuid(),
                WorkScheduleId = workSchedule.Id,
                WorkSchedule = workSchedule,
                TimeSlot = workSchedule.ShiftStart
            };
            cancelledApp.Cancel();
            workSchedule.Appointments.Add(cancelledApp);

            var command = new CreateAppointmentCommand(workSchedule.Id, workSchedule.ShiftStart.AddMinutes(30), "After cancellation");

            // Act
            var appointmentId = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotEqual(Guid.Empty, appointmentId);
            Assert.True(unitOfWork.WasCommitted);
        }
    }
}

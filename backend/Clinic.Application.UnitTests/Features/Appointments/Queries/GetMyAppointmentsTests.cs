using Clinic.Application.Common.Exceptions;
using Clinic.Application.Features.Appointments.Queries;
using Clinic.Application.UnitTests.Common;

namespace Clinic.Application.UnitTests.Features.Appointments.Queries
{
    public class GetMyAppointmentsTests
    {
        [Fact]
        public async Task Handle_PatientWithAppointments_ShouldReturnMappedDtos()
        {
            // Arrange
            var appointmentRepository = new FakeAppointmentRepository();
            var currentUser = new FakeCurrentUser();
            var handler = new GetMyAppointmentsHandler(appointmentRepository, currentUser);

            var patientId = Guid.NewGuid();
            currentUser.PatientId = patientId;

            var appointment1 = TestDataFactory.CreateAppointment(patientId: patientId, timeSlot: DateTime.UtcNow.AddDays(1));
            var appointment2 = TestDataFactory.CreateAppointment(patientId: patientId, timeSlot: DateTime.UtcNow.AddDays(2));
            // Another patient's appointment
            var appointmentOther = TestDataFactory.CreateAppointment(patientId: Guid.NewGuid(), timeSlot: DateTime.UtcNow.AddDays(3));

            await appointmentRepository.AddAsync(appointment1);
            await appointmentRepository.AddAsync(appointment2);
            await appointmentRepository.AddAsync(appointmentOther);

            // Act
            var result = await handler.Handle(new GetMyAppointmentsQuery(), CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, a => a.Id == appointment1.Id);
            Assert.Contains(result, a => a.Id == appointment2.Id);
            Assert.DoesNotContain(result, a => a.Id == appointmentOther.Id);
        }

        [Fact]
        public async Task Handle_PatientWithoutAppointments_ShouldReturnEmptyList()
        {
            // Arrange
            var appointmentRepository = new FakeAppointmentRepository();
            var currentUser = new FakeCurrentUser { PatientId = Guid.NewGuid() };
            var handler = new GetMyAppointmentsHandler(appointmentRepository, currentUser);

            // Act
            var result = await handler.Handle(new GetMyAppointmentsQuery(), CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task Handle_UserWithoutPatientId_ShouldThrowForbiddenException()
        {
            // Arrange
            var appointmentRepository = new FakeAppointmentRepository();
            var currentUser = new FakeCurrentUser { PatientId = null };
            var handler = new GetMyAppointmentsHandler(appointmentRepository, currentUser);

            // Act & Assert
            await Assert.ThrowsAsync<ForbiddenException>(() =>
                handler.Handle(new GetMyAppointmentsQuery(), CancellationToken.None));
        }
    }
}

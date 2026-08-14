using Clinic.Application.Common.Exceptions;
using Clinic.Application.Features.Appointments.Commands;
using Clinic.Application.UnitTests.Common;
using Clinic.Domain.Entities;
using Clinic.Domain.Enums;

namespace Clinic.Application.UnitTests.Features.Appointments.Commands
{
    public class CancelAppointmentTests
    {
        [Fact]
        public async Task TestCancelAppointment()
        {
            var appointmentRepository = new FakeAppointmentRepository();
            var userRepository = new FakeUserRepository();
            var unitOfWork = new FakeUnitOfWork();
            var handler = new CancelAppointmentCommandHandler(appointmentRepository, userRepository, unitOfWork);
            var patient = TestDataFactory.CreatePatient();
            var workSchedule = TestDataFactory.CreateWorkSchedule(date: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)));
            var appointment = TestDataFactory.CreateAppointment(patient, workSchedule);
            var appointment2 = TestDataFactory.CreateAppointment(patient, workSchedule, new TimeOnly(11, 0));
            await appointmentRepository.AddAsync(appointment);
            await appointmentRepository.AddAsync(appointment2);
            await userRepository.AddAsync(patient.Person.User);

            // Patient cancels the appointment themselves
            var command = new CancelAppointmentCommand(AppointmentId: appointment.Id, patient.Person.User.Id);
            await handler.Handle(command, CancellationToken.None);
            Assert.Equal(AppointmentStatus.Cancelled, appointment.Status);
            Assert.Equal(patient.Person.User.Id, appointment.CancelledByUserId);

            // Receptionist cancels the appointment on behalf of the patient
            var receptionist = TestDataFactory.CreateEmployee(role: "Receptionist");
            await userRepository.AddAsync(receptionist.Person.User);
            var command2 = new CancelAppointmentCommand(AppointmentId: appointment2.Id, receptionist.Person.User.Id);
            await handler.Handle(command2, CancellationToken.None);
            Assert.Equal(AppointmentStatus.Cancelled, appointment2.Status);
            Assert.Equal(receptionist.Person.User.Id, appointment2.CancelledByUserId);
        }

        [Fact]
        public async Task TestCancelNonExistentAppointment()
        {
            var appointmentRepository = new FakeAppointmentRepository();
            var userRepository = new FakeUserRepository();
            var unitOfWork = new FakeUnitOfWork();
            var handler = new CancelAppointmentCommandHandler(appointmentRepository, userRepository, unitOfWork);
            var patient = TestDataFactory.CreatePatient();
            var receptionist = TestDataFactory.CreateEmployee(role: "Receptionist");
            var workSchedule = TestDataFactory.CreateWorkSchedule();
            var appointment = TestDataFactory.CreateAppointment(patient, workSchedule);
            await userRepository.AddAsync(patient.Person.User);
            await userRepository.AddAsync(receptionist.Person.User);
            await appointmentRepository.AddAsync(appointment);
            var command = new CancelAppointmentCommand(AppointmentId: Guid.NewGuid(), patient.Person.User.Id);
            await Assert.ThrowsAsync<NotFoundException>(async () => await handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task TestNonExistentCancelAppointment()
        {
            var appointmentRepository = new FakeAppointmentRepository();
            var userRepository = new FakeUserRepository();
            var unitOfWork = new FakeUnitOfWork();
            var handler = new CancelAppointmentCommandHandler(appointmentRepository, userRepository, unitOfWork);
            var patient = TestDataFactory.CreatePatient();
            var workSchedule = TestDataFactory.CreateWorkSchedule();
            var appointment = TestDataFactory.CreateAppointment(patient, workSchedule);
            await appointmentRepository.AddAsync(appointment);
            var command = new CancelAppointmentCommand(AppointmentId: appointment.Id, Guid.NewGuid());
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await handler.Handle(command, CancellationToken.None));

            var admin = TestDataFactory.CreateEmployee(role: "Admin");
            await userRepository.AddAsync(admin.Person.User);
            var command2 = new CancelAppointmentCommand(AppointmentId: appointment.Id, admin.Person.User.Id);
            await Assert.ThrowsAsync<ForbiddenException>(async () => await handler.Handle(command2, CancellationToken.None));
        }
    }
}

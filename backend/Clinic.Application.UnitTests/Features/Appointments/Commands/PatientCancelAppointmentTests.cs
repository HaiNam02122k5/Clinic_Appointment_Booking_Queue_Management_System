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
            var patientRepository = new FakePatientRepository();
            var unitOfWork = new FakeUnitOfWork();
            var handler = new CancelAppointmentCommandHandler(appointmentRepository, patientRepository, unitOfWork);
            var person = TestDataFactory.CreatePerson();
            var user = TestDataFactory.CreateUser(person: person);
            var patient = TestDataFactory.CreatePatient(person: person);
            var workSchedule = TestDataFactory.CreateWorkSchedule();
            var appointment = TestDataFactory.CreateAppointment(patient, workSchedule);
            var appointment2 = TestDataFactory.CreateAppointment(patient, workSchedule, new TimeOnly(11, 0));
            await appointmentRepository.AddAsync(appointment);
            await appointmentRepository.AddAsync(appointment2);
            await patientRepository.AddAsync(patient);

            // Patient cancels the appointment themselves
            var command = new CancelAppointmentCommand(AppointmentId: appointment.Id, patient.Person.User.Id);
            await handler.Handle(command, CancellationToken.None);
            Assert.Equal(AppointmentStatus.Cancelled, appointment.Status);
            Assert.Equal(user.Id, appointment.CancelledByUserId);

            // Receptionist cancels the appointment on behalf of the patient
            var receptionist = TestDataFactory.CreateUser();
            receptionist.AssignRole(TestDataFactory.RoleSet.First(r => r.Name == "Receptionist"));
            var command2 = new CancelAppointmentCommand(AppointmentId: appointment2.Id, receptionist.Id, patient.Id);
            await handler.Handle(command2, CancellationToken.None);
            Assert.Equal(AppointmentStatus.Cancelled, appointment2.Status);
            Assert.Equal(receptionist.Id, appointment2.CancelledByUserId);
        }

        [Fact]
        public async Task TestCancelNonExistentAppointment()
        {
            var appointmentRepository = new FakeAppointmentRepository();
            var patientRepository = new FakePatientRepository();
            var unitOfWork = new FakeUnitOfWork();
            var handler = new CancelAppointmentCommandHandler(appointmentRepository, patientRepository, unitOfWork);
            var person = TestDataFactory.CreatePerson();
            var user = TestDataFactory.CreateUser(person: person);
            var patient = TestDataFactory.CreatePatient(person: person);
            var workSchedule = TestDataFactory.CreateWorkSchedule();
            var appointment = TestDataFactory.CreateAppointment(patient, workSchedule);
            await appointmentRepository.AddAsync(appointment);
            var command = new CancelAppointmentCommand(AppointmentId: appointment.Id, Guid.NewGuid());
            await Assert.ThrowsAsync<NotFoundException>(async () => await handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task TestNonExistentCancelAppointment()
        {
            var appointmentRepository = new FakeAppointmentRepository();
            var patientRepository = new FakePatientRepository();
            var unitOfWork = new FakeUnitOfWork();
            var handler = new CancelAppointmentCommandHandler(appointmentRepository, patientRepository, unitOfWork);
            var person = TestDataFactory.CreatePerson();
            var user = TestDataFactory.CreateUser(person: person);
            var patient = TestDataFactory.CreatePatient(person: person);
            var workSchedule = TestDataFactory.CreateWorkSchedule();
            var appointment = TestDataFactory.CreateAppointment(patient, workSchedule);
            await appointmentRepository.AddAsync(appointment);
            var command = new CancelAppointmentCommand(AppointmentId: Guid.NewGuid(), patient.Person.User.Id);
            await Assert.ThrowsAsync<NotFoundException>(async () => await handler.Handle(command, CancellationToken.None));
        }
    }
}

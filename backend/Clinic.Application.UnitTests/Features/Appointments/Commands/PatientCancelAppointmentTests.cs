using Clinic.Application.Common.Exceptions;
using Clinic.Application.Features.Appointments.Commands;
using Clinic.Application.UnitTests.Common;
using Clinic.Domain.Entities;
using Clinic.Domain.Enums;

namespace Clinic.Application.UnitTests.Features.Appointments.Commands
{
    public class PatientCancelAppointmentTests
    {
        [Fact]
        public async Task TestPatientCancelAppointment()
        {
            var appointmentRepository = new FakeAppointmentRepository();
            var patientRepository = new FakePatientRepository();
            var unitOfWork = new FakeUnitOfWork();
            var handler = new PatientCancelAppointmentCommandHandler(appointmentRepository, patientRepository, unitOfWork);
            var person = TestDataFactory.CreatePerson();
            var user = TestDataFactory.CreateUser(person: person);
            var patient = TestDataFactory.CreatePatient(person: person);
            var workSchedule = TestDataFactory.CreateWorkSchedule();
            var appointment = TestDataFactory.CreateAppointment(patient, workSchedule);
            await appointmentRepository.AddAsync(appointment);
            await patientRepository.AddAsync(patient);
            var command = new PatientCancelAppointmentCommand(AppointmentId: appointment.Id, patient.Person.User.Id);
            await handler.Handle(command, CancellationToken.None);
            Assert.Equal(AppointmentStatus.Cancelled, appointment.Status);
        }

        [Fact]
        public async Task TestPatientCancelNonExistentAppointment()
        {
            var appointmentRepository = new FakeAppointmentRepository();
            var patientRepository = new FakePatientRepository();
            var unitOfWork = new FakeUnitOfWork();
            var handler = new PatientCancelAppointmentCommandHandler(appointmentRepository, patientRepository, unitOfWork);
            var person = TestDataFactory.CreatePerson();
            var user = TestDataFactory.CreateUser(person: person);
            var patient = TestDataFactory.CreatePatient(person: person);
            var workSchedule = TestDataFactory.CreateWorkSchedule();
            var appointment = TestDataFactory.CreateAppointment(patient, workSchedule);
            await appointmentRepository.AddAsync(appointment);
            var command = new PatientCancelAppointmentCommand(AppointmentId: appointment.Id, Guid.NewGuid());
            await Assert.ThrowsAsync<NotFoundException>(async () => await handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task TestNonExistentPatientCancelAppointment()
        {
            var appointmentRepository = new FakeAppointmentRepository();
            var patientRepository = new FakePatientRepository();
            var unitOfWork = new FakeUnitOfWork();
            var handler = new PatientCancelAppointmentCommandHandler(appointmentRepository, patientRepository, unitOfWork);
            var person = TestDataFactory.CreatePerson();
            var user = TestDataFactory.CreateUser(person: person);
            var patient = TestDataFactory.CreatePatient(person: person);
            var workSchedule = TestDataFactory.CreateWorkSchedule();
            var appointment = TestDataFactory.CreateAppointment(patient, workSchedule);
            await appointmentRepository.AddAsync(appointment);
            var command = new PatientCancelAppointmentCommand(AppointmentId: Guid.NewGuid(), patient.Person.User.Id);
            await Assert.ThrowsAsync<NotFoundException>(async () => await handler.Handle(command, CancellationToken.None));
        }
    }
}

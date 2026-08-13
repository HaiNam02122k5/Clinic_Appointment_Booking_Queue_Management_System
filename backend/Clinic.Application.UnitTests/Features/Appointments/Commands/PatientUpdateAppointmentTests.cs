using Clinic.Application.Common.Exceptions;
using Clinic.Application.Features.Appointments.Commands;
using Clinic.Application.UnitTests.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.Application.UnitTests.Features.Appointments.Commands
{
    public class PatientUpdateAppointmentTests
    {
        [Fact]
        public async Task TestPatientUpdateAppointment()
        {
            var workScheduleRepository = new FakeWorkScheduleRepository();
            var patientRepository = new FakePatientRepository();
            var appointmentRepository = new FakeAppointmentRepository();
            var unitOfWork = new FakeUnitOfWork();
            var handler = new PatientUpdateAppointmentCommandHandler(appointmentRepository, patientRepository, workScheduleRepository, unitOfWork);
            var person = TestDataFactory.CreatePerson();
            var user = TestDataFactory.CreateUser(person: person);
            var patient = TestDataFactory.CreatePatient(person: person);
            var workSchedule = TestDataFactory.CreateWorkSchedule();

            var appointment = TestDataFactory.CreateAppointment(patient: patient);
            await workScheduleRepository.AddWorkScheduleAsync(workSchedule);
            await patientRepository.AddAsync(patient);
            await appointmentRepository.AddAsync(appointment);

            var command = new PatientUpdateAppointmentCommand(user.Id, appointment.Id, workSchedule.Id, new TimeOnly(10, 0));
            var result = await handler.Handle(command, CancellationToken.None);
            Assert.NotNull(workSchedule.Appointments.FirstOrDefault(a => a.Id == result.Id));
        }

        [Fact]
        public async Task TestPatientUpdateAppointmentInNonExistentWorkSchedule()
        {
            var workScheduleRepository = new FakeWorkScheduleRepository();
            var patientRepository = new FakePatientRepository();
            var appointmentRepository = new FakeAppointmentRepository();
            var unitOfWork = new FakeUnitOfWork();
            var handler = new PatientUpdateAppointmentCommandHandler(appointmentRepository, patientRepository, workScheduleRepository, unitOfWork);
            var person = TestDataFactory.CreatePerson();
            var user = TestDataFactory.CreateUser(person: person);
            var patient = TestDataFactory.CreatePatient(person: person);
            var workSchedule = TestDataFactory.CreateWorkSchedule();
            var appointment = TestDataFactory.CreateAppointment(patient: patient);
            await workScheduleRepository.AddWorkScheduleAsync(workSchedule);
            await patientRepository.AddAsync(patient);
            await appointmentRepository.AddAsync(appointment);

            var command = new PatientUpdateAppointmentCommand(user.Id, appointment.Id, Guid.NewGuid(), new TimeOnly(10, 0));
            await Assert.ThrowsAsync<NotFoundException>(async () => await handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task TestNonExistentPatientUpdateAppointment()
        {
            var workScheduleRepository = new FakeWorkScheduleRepository();
            var patientRepository = new FakePatientRepository();
            var appointmentRepository = new FakeAppointmentRepository();
            var unitOfWork = new FakeUnitOfWork();
            var handler = new PatientUpdateAppointmentCommandHandler(appointmentRepository, patientRepository, workScheduleRepository, unitOfWork);
            var person = TestDataFactory.CreatePerson();
            var user = TestDataFactory.CreateUser(person: person);
            var patient = TestDataFactory.CreatePatient(person: person);
            var workSchedule = TestDataFactory.CreateWorkSchedule();
            var appointment = TestDataFactory.CreateAppointment(patient: patient);
            await workScheduleRepository.AddWorkScheduleAsync(workSchedule);
            await patientRepository.AddAsync(patient);
            await appointmentRepository.AddAsync(appointment);

            var command = new PatientUpdateAppointmentCommand(Guid.NewGuid(), appointment.Id, workSchedule.Id, new TimeOnly(10, 0));
            await Assert.ThrowsAsync<NotFoundException>(async () => await handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task TestPatientUpdateNonExistentAppointment()
        {
            var workScheduleRepository = new FakeWorkScheduleRepository();
            var patientRepository = new FakePatientRepository();
            var appointmentRepository = new FakeAppointmentRepository();
            var unitOfWork = new FakeUnitOfWork();
            var handler = new PatientUpdateAppointmentCommandHandler(appointmentRepository, patientRepository, workScheduleRepository, unitOfWork);
            var person = TestDataFactory.CreatePerson();
            var user = TestDataFactory.CreateUser(person: person);
            var patient = TestDataFactory.CreatePatient(person: person);
            var workSchedule = TestDataFactory.CreateWorkSchedule();
            var appointment = TestDataFactory.CreateAppointment(patient: patient);
            await workScheduleRepository.AddWorkScheduleAsync(workSchedule);
            await patientRepository.AddAsync(patient);
            await appointmentRepository.AddAsync(appointment);

            var command = new PatientUpdateAppointmentCommand(user.Id, Guid.NewGuid(), workSchedule.Id, new TimeOnly(10, 0));
            await Assert.ThrowsAsync<NotFoundException>(async () => await handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task TestUnauthorizedPatientUpdateAppointment()
        {
            var workScheduleRepository = new FakeWorkScheduleRepository();
            var patientRepository = new FakePatientRepository();
            var appointmentRepository = new FakeAppointmentRepository();
            var unitOfWork = new FakeUnitOfWork();
            var handler = new PatientUpdateAppointmentCommandHandler(appointmentRepository, patientRepository, workScheduleRepository, unitOfWork);
            var person = TestDataFactory.CreatePerson();
            var user = TestDataFactory.CreateUser(person: person);
            var patient = TestDataFactory.CreatePatient(person: person);
            var workSchedule = TestDataFactory.CreateWorkSchedule();
            var appointment = TestDataFactory.CreateAppointment();
            await workScheduleRepository.AddWorkScheduleAsync(workSchedule);
            await patientRepository.AddAsync(patient);
            await appointmentRepository.AddAsync(appointment);

            var command = new PatientUpdateAppointmentCommand(user.Id, appointment.Id, workSchedule.Id, new TimeOnly(10, 0));
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await handler.Handle(command, CancellationToken.None));
        }
    }
}

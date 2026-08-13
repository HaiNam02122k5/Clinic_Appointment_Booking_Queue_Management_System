using Clinic.Application.Common.Exceptions;
using Clinic.Application.Features.Appointments.Commands;
using Clinic.Application.UnitTests.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.Application.UnitTests.Features.Appointments.Commands
{
    public class UpdateAppointmentTests
    {
        [Fact]
        public async Task TestUpdateAppointment()
        {
            var workScheduleRepository = new FakeWorkScheduleRepository();
            var patientRepository = new FakePatientRepository();
            var appointmentRepository = new FakeAppointmentRepository();
            var unitOfWork = new FakeUnitOfWork();
            var handler = new UpdateAppointmentCommandHandler(appointmentRepository, patientRepository, workScheduleRepository, unitOfWork);
            var person = TestDataFactory.CreatePerson();
            var user = TestDataFactory.CreateUser(person: person);
            var patient = TestDataFactory.CreatePatient(person: person);
            var workSchedule = TestDataFactory.CreateWorkSchedule();

            var appointment = TestDataFactory.CreateAppointment(patient: patient); // Initial version
            await workScheduleRepository.AddWorkScheduleAsync(workSchedule);
            await patientRepository.AddAsync(patient);
            await appointmentRepository.AddAsync(appointment);

            var command = new UpdateAppointmentCommand(user.Id, appointment.Id, workSchedule.Id, new TimeOnly(10, 0), "Reason"); // 1st update
            var result = await handler.Handle(command, CancellationToken.None);
            Assert.NotNull(workSchedule.Appointments.FirstOrDefault(a => a.Id == result.Id));

            var receptionist = TestDataFactory.CreateUser();
            receptionist.AssignRole(TestDataFactory.RoleSet.First(r => r.Name == "Receptionist"));
            var command2 = new UpdateAppointmentCommand(receptionist.Id, appointment.Id, workSchedule.Id, new TimeOnly(11, 0), "Reason", patient.Id); // 2nd update by receptionist
            var result2 = await handler.Handle(command2, CancellationToken.None);
            Assert.Equal(receptionist.Id, appointment.CreatedByUserId);
            Assert.Equal(2, appointment.AppointmentSnapshots.Count); // => 2 snapshots created
        }

        [Fact]
        public async Task TestUpdateAppointmentInNonExistentWorkSchedule()
        {
            var workScheduleRepository = new FakeWorkScheduleRepository();
            var patientRepository = new FakePatientRepository();
            var appointmentRepository = new FakeAppointmentRepository();
            var unitOfWork = new FakeUnitOfWork();
            var handler = new UpdateAppointmentCommandHandler(appointmentRepository, patientRepository, workScheduleRepository, unitOfWork);
            var person = TestDataFactory.CreatePerson();
            var user = TestDataFactory.CreateUser(person: person);
            var patient = TestDataFactory.CreatePatient(person: person);
            var workSchedule = TestDataFactory.CreateWorkSchedule();
            var appointment = TestDataFactory.CreateAppointment(patient: patient);
            await workScheduleRepository.AddWorkScheduleAsync(workSchedule);
            await patientRepository.AddAsync(patient);
            await appointmentRepository.AddAsync(appointment);

            var command = new UpdateAppointmentCommand(user.Id, appointment.Id, Guid.NewGuid(), new TimeOnly(10, 0), "Reason");
            await Assert.ThrowsAsync<NotFoundException>(async () => await handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task TestNonExistentUpdateAppointment()
        {
            var workScheduleRepository = new FakeWorkScheduleRepository();
            var patientRepository = new FakePatientRepository();
            var appointmentRepository = new FakeAppointmentRepository();
            var unitOfWork = new FakeUnitOfWork();
            var handler = new UpdateAppointmentCommandHandler(appointmentRepository, patientRepository, workScheduleRepository, unitOfWork);
            var person = TestDataFactory.CreatePerson();
            var user = TestDataFactory.CreateUser(person: person);
            var patient = TestDataFactory.CreatePatient(person: person);
            var workSchedule = TestDataFactory.CreateWorkSchedule();
            var appointment = TestDataFactory.CreateAppointment(patient: patient);
            await workScheduleRepository.AddWorkScheduleAsync(workSchedule);
            await patientRepository.AddAsync(patient);
            await appointmentRepository.AddAsync(appointment);

            var command = new UpdateAppointmentCommand(Guid.NewGuid(), appointment.Id, workSchedule.Id, new TimeOnly(10, 0), "Reason");
            await Assert.ThrowsAsync<NotFoundException>(async () => await handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task TestUpdateNonExistentAppointment()
        {
            var workScheduleRepository = new FakeWorkScheduleRepository();
            var patientRepository = new FakePatientRepository();
            var appointmentRepository = new FakeAppointmentRepository();
            var unitOfWork = new FakeUnitOfWork();
            var handler = new UpdateAppointmentCommandHandler(appointmentRepository, patientRepository, workScheduleRepository, unitOfWork);
            var person = TestDataFactory.CreatePerson();
            var user = TestDataFactory.CreateUser(person: person);
            var patient = TestDataFactory.CreatePatient(person: person);
            var workSchedule = TestDataFactory.CreateWorkSchedule();
            var appointment = TestDataFactory.CreateAppointment(patient: patient);
            await workScheduleRepository.AddWorkScheduleAsync(workSchedule);
            await patientRepository.AddAsync(patient);
            await appointmentRepository.AddAsync(appointment);

            var command = new UpdateAppointmentCommand(user.Id, Guid.NewGuid(), workSchedule.Id, new TimeOnly(10, 0), "Reason");
            await Assert.ThrowsAsync<NotFoundException>(async () => await handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task TestUnauthorizedUpdateAppointment()
        {
            var workScheduleRepository = new FakeWorkScheduleRepository();
            var patientRepository = new FakePatientRepository();
            var appointmentRepository = new FakeAppointmentRepository();
            var unitOfWork = new FakeUnitOfWork();
            var handler = new UpdateAppointmentCommandHandler(appointmentRepository, patientRepository, workScheduleRepository, unitOfWork);
            var person = TestDataFactory.CreatePerson();
            var user = TestDataFactory.CreateUser(person: person);
            var patient = TestDataFactory.CreatePatient(person: person);
            var workSchedule = TestDataFactory.CreateWorkSchedule();
            var appointment = TestDataFactory.CreateAppointment();
            await workScheduleRepository.AddWorkScheduleAsync(workSchedule);
            await patientRepository.AddAsync(patient);
            await appointmentRepository.AddAsync(appointment);

            var command = new UpdateAppointmentCommand(user.Id, appointment.Id, workSchedule.Id, new TimeOnly(10, 0), "Reason");
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await handler.Handle(command, CancellationToken.None));
        }
    }
}

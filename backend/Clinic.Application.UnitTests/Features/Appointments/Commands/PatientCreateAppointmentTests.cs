using Clinic.Application.Common.Exceptions;
using Clinic.Application.Features.Appointments.Commands;
using Clinic.Application.UnitTests.Common;
using Clinic.Domain.Enums;

namespace Clinic.Application.UnitTests.Features.Appointments.Commands
{
    public class PatientCreateAppointmentTests
    {
        [Fact]
        public async Task TestPatientCreateAppointment()
        {
            var workScheduleRepository = new FakeWorkScheduleRepository();
            var patientRepository = new FakePatientRepository();
            var unitOfWork = new FakeUnitOfWork();
            var handler = new PatientCreateAppointmentCommandHandler(workScheduleRepository, patientRepository, unitOfWork);
            var person = TestDataFactory.CreatePerson();
            var user = TestDataFactory.CreateUser(person: person);
            var patient = TestDataFactory.CreatePatient(person: person);
            var workSchedule = TestDataFactory.CreateWorkSchedule();
            await workScheduleRepository.AddWorkScheduleAsync(workSchedule);
            await patientRepository.AddAsync(patient);

            var command = new PatientCreateAppointmentCommand(user.Id, workSchedule.Id, new TimeOnly(10, 0));
            var result = await handler.Handle(command, CancellationToken.None);
            Assert.NotNull(workSchedule.Appointments.FirstOrDefault(a => a.Id == result.Id));
        }

        [Fact]
        public async Task TestPatientCreateAppointmentInNonExistentWorkSchedule()
        {
            var workScheduleRepository = new FakeWorkScheduleRepository();
            var patientRepository = new FakePatientRepository();
            var unitOfWork = new FakeUnitOfWork();
            var handler = new PatientCreateAppointmentCommandHandler(workScheduleRepository, patientRepository, unitOfWork);
            var person = TestDataFactory.CreatePerson();
            var user = TestDataFactory.CreateUser(person: person);
            var patient = TestDataFactory.CreatePatient(person: person);
            var workSchedule = TestDataFactory.CreateWorkSchedule();
            await workScheduleRepository.AddWorkScheduleAsync(workSchedule);
            await patientRepository.AddAsync(patient);

            var command = new PatientCreateAppointmentCommand(user.Id, Guid.NewGuid(), new TimeOnly(10, 0));
            await Assert.ThrowsAsync<NotFoundException>(async () => await handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task TestNonExistentPatientCreateAppointment()
        {
            var workScheduleRepository = new FakeWorkScheduleRepository();
            var patientRepository = new FakePatientRepository();
            var unitOfWork = new FakeUnitOfWork();
            var handler = new PatientCreateAppointmentCommandHandler(workScheduleRepository, patientRepository, unitOfWork);
            var person = TestDataFactory.CreatePerson();
            var user = TestDataFactory.CreateUser(person: person);
            var patient = TestDataFactory.CreatePatient(person: person);
            var workSchedule = TestDataFactory.CreateWorkSchedule();
            await workScheduleRepository.AddWorkScheduleAsync(workSchedule);
            await patientRepository.AddAsync(patient);

            var command = new PatientCreateAppointmentCommand(Guid.NewGuid(), workSchedule.Id, new TimeOnly(10, 0));
            await Assert.ThrowsAsync<NotFoundException>(async () => await handler.Handle(command, CancellationToken.None));
        }
    }
}

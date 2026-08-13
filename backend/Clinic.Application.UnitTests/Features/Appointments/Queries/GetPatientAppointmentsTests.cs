using Clinic.Application.Common.Exceptions;
using Clinic.Application.Features.Appointments.Queries;
using Clinic.Application.UnitTests.Common;
using Clinic.Domain.Enums;

namespace Clinic.Application.UnitTests.Features.Appointments.Queries
{
    public class GetPatientAppointmentsTests
    {
        [Fact]
        public async Task TestGetPatientAppointments()
        {
            var patientRepository = new FakePatientRepository();
            var appointmentRepository = new FakeAppointmentRepository();
            var handler = new GetPatientAppointmentsQueryHandler(appointmentRepository, patientRepository);
            var person = TestDataFactory.CreatePerson();
            var user = TestDataFactory.CreateUser(person: person);
            var patient = TestDataFactory.CreatePatient(person: person);
            var workSchedule = TestDataFactory.CreateWorkSchedule();
            var appointment1 = TestDataFactory.CreateAppointment(patient: patient, workSchedule: workSchedule);
            var appointment2 = TestDataFactory.CreateAppointment(patient: patient, workSchedule: workSchedule, timeSlot: new TimeOnly(11, 0));
            appointment2.UpdateStatus(AppointmentStatus.Completed, user.Id);
            await appointmentRepository.AddAsync(appointment1);
            await appointmentRepository.AddAsync(appointment2);
            await patientRepository.AddAsync(patient);
            var query = new GetPatientAppointmentsQuery(user.Id);
            var result = await handler.Handle(query, CancellationToken.None);
            Assert.Single(result);
            Assert.Equal(AppointmentStatus.Pending, result[0].Status);

            var queryCompleted = new GetPatientAppointmentsQuery(user.Id, "Completed");
            var resultCompleted = await handler.Handle(queryCompleted, CancellationToken.None);
            Assert.Single(resultCompleted);
            Assert.Equal(AppointmentStatus.Completed, resultCompleted[0].Status);
        }

        [Fact]
        public async Task TestGetPatientAppointmentsForNonExistentPatient()
        {
            var patientRepository = new FakePatientRepository();
            var appointmentRepository = new FakeAppointmentRepository();
            var handler = new GetPatientAppointmentsQueryHandler(appointmentRepository, patientRepository);
            var query = new GetPatientAppointmentsQuery(Guid.NewGuid());
            await Assert.ThrowsAsync<NotFoundException>(async () => await handler.Handle(query, CancellationToken.None));
        }
    }
}

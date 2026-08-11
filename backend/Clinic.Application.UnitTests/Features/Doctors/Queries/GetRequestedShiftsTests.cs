using Clinic.Application.Common.Exceptions;
using Clinic.Application.Features.Doctors.Queries;
using Clinic.Application.UnitTests.Common;
using Clinic.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.Application.UnitTests.Features.Doctors.Queries
{
    public class GetRequestedShiftsTests
    {
        public class GetSchedulesTests
        {
            [Fact]
            public async Task TestGetRequestedShifts()
            {
                var doctorRepository = new FakeDoctorRepository();
                var handler = new GetDoctorRequestedShiftsHandler(doctorRepository);
                var doctor = TestDataFactory.CreateDoctor();
                await doctorRepository.AddAsync(doctor);
                var schedule1 = new ShiftRequest(doctor, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(1).AddHours(1), 5, "");
                var schedule2 = new ShiftRequest(doctor, DateTime.UtcNow.AddDays(10), DateTime.UtcNow.AddDays(10).AddHours(1), 5, "");
                doctor.AddShiftRequest(schedule1);
                doctor.AddShiftRequest(schedule2);
                var query = new GetDoctorRequestedShiftsQuery(doctor.Id, DateOnly.FromDateTime(DateTime.UtcNow.AddMinutes(-1)), DateOnly.FromDateTime(DateTime.UtcNow.AddDays(15)));
                var result = await handler.Handle(query, CancellationToken.None);
                Assert.Equal(2, result.Schedules.Count());

                var query2 = new GetDoctorRequestedShiftsQuery(doctor.Id, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)), DateOnly.FromDateTime(DateTime.UtcNow.AddDays(15)));
                var result2 = await handler.Handle(query2, CancellationToken.None);
                Assert.Single(result2.Schedules);

                var query3 = new GetDoctorRequestedShiftsQuery(doctor.Id, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(11)), DateOnly.FromDateTime(DateTime.UtcNow.AddDays(15)));
                var result3 = await handler.Handle(query3, CancellationToken.None);
                Assert.Empty(result3.Schedules);

                var query4 = new GetDoctorRequestedShiftsQuery(doctor.Id, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)), DateOnly.FromDateTime(DateTime.UtcNow.AddDays(40)));
                await Assert.ThrowsAsync<ArgumentException>(async () => await handler.Handle(query4, CancellationToken.None));
            }

            public async Task TestGetRequestedShiftsForNonExistingDoctor()
            {
                var doctorRepository = new FakeDoctorRepository();
                var handler = new GetDoctorRequestedShiftsHandler(doctorRepository);
                var query = new GetDoctorRequestedShiftsQuery(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow.AddMinutes(-1)), DateOnly.FromDateTime(DateTime.UtcNow.AddDays(15)));
                await Assert.ThrowsAsync<NotFoundException>(async () => await handler.Handle(query, CancellationToken.None));
            }
        }
    }
}

using Clinic.Application.Features.WorkSchedules.Queries;
using Clinic.Application.UnitTests.Common;
using Clinic.Domain.Entities;


namespace Clinic.Application.UnitTests.Features.WorkSchedules.Queries
{
    public class GetSchedulesForBookingTests
    {
        [Fact]
        public async Task TestGetSchedulesForBooking()
        {
            // Arrange
            var workScheduleRepository = new FakeWorkScheduleRepository();
            var handler = new GetDoctorSchedulesForBookingQueryHandler(workScheduleRepository);
            var doctor = TestDataFactory.CreateDoctor();
            var now = new DateTime(2026, 9, 5, 7, 0, 0, DateTimeKind.Unspecified);
            var schedule1 = new WorkSchedule(doctor, now.AddDays(1), now.AddDays(1).AddHours(4), 5);
            var schedule2 = new WorkSchedule(doctor, now.AddDays(1).AddHours(6), now.AddDays(1).AddHours(10), 5);
            doctor.AddWorkSchedule(schedule1);
            doctor.AddWorkSchedule(schedule2);
            await workScheduleRepository.AddWorkScheduleAsync(schedule1);
            await workScheduleRepository.AddWorkScheduleAsync(schedule2);
            // Act
            var query = new GetDoctorSchedulesForBookingQuery(doctor.Id, DateOnly.FromDateTime(now.AddDays(1)));
            var result = await handler.Handle(query, CancellationToken.None);
            // Assert
            // 2 schedules, each with 4 hours and 4 slots/hour (15-minute intervals), so total slots = 2 * 4 * 4 = 32
            Assert.Equal(2 * 4 * 4, result.Count);
        }
    }
}

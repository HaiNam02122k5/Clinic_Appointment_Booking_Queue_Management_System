using Clinic.Application.Contracts;
using Clinic.Application.Interfaces;
using Clinic.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.Application.Features.WorkSchedules.Queries
{
    // Use-case: Get available work schedules for a doctor on a specific date for booking
    public record GetDoctorSchedulesForBookingQuery(
        Guid DoctorId,
        DateOnly Date
    ) : IRequest<List<AppointmentSlotDto>>;
    public class GetDoctorSchedulesForBookingQueryHandler : IRequestHandler<GetDoctorSchedulesForBookingQuery, List<AppointmentSlotDto>>
    {
        private readonly IWorkScheduleRepository _workScheduleRepository;

        public GetDoctorSchedulesForBookingQueryHandler(IWorkScheduleRepository workScheduleRepository)
        {
            _workScheduleRepository = workScheduleRepository;
        }
        
        public async Task<List<AppointmentSlotDto>> Handle(GetDoctorSchedulesForBookingQuery request, CancellationToken cancellationToken)
        {
            var workSchedules = await _workScheduleRepository.GetDoctorSchedulesWithAppointmentByDateAsync(request.DoctorId, request.Date);
            var appointmentSlots = new List<AppointmentSlotDto>();
            foreach (var workSchedule in workSchedules)
            {
                var slotTime = workSchedule.ShiftStart;
                while (slotTime < workSchedule.ShiftEnd)
                {
                    // Check if the slot is already booked
                    if (!workSchedule.Appointments.Any(a => a.TimeSlot == slotTime))
                    {
                        appointmentSlots.Add(new AppointmentSlotDto
                        {
                            DoctorId = request.DoctorId,
                            StartTime = slotTime
                        });
                    }
                    slotTime = slotTime.AddMinutes(WorkSchedule.SlotIntervalMinutes);
                }
            }
            return appointmentSlots;
        }
    }
}

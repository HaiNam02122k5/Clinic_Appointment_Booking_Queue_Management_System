using Clinic.Application.Features.Appointments.Commands;
using Clinic.Application.Features.Appointments.Queries;
using Clinic.Application.Features.Queue.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.API.Controllers
{
    [ApiController]
    [Route("/appointments")]
    public class AppointmentsController : ControllerBase
    {
        private readonly ISender _sender;

        public AppointmentsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        [Authorize(Policy = "Permission:appointment.create")]
        public async Task<IActionResult> Create([FromBody] CreateAppointmentRequest request)
        {
            var appointmentId = await _sender.Send(new CreateAppointmentCommand(request.WorkScheduleId, request.TimeSlot, request.Reason));
            return StatusCode(StatusCodes.Status201Created, new { id = appointmentId });
        }

        [HttpPatch("{appointmentId}")]
        [Authorize(Policy = "Permission:appointment.update")]
        public IActionResult Patch([FromRoute] string appointmentId)
        {
            return NoContent();
        }

        [HttpPost("{appointmentId}/confirm")]
        [Authorize(Policy = "Permission:appointment.confirm")]
        public async Task<IActionResult> Confirm([FromRoute] Guid appointmentId)
        {
            await _sender.Send(new ConfirmAppointmentCommand(appointmentId));
            return NoContent();
        }

        [HttpPost("{appointmentId}/check-in")]
        [Authorize(Policy = "Permission:queue.check-in")]
        public async Task<IActionResult> CheckIn([FromRoute] Guid appointmentId)
        {
            await _sender.Send(new CheckInCommand(appointmentId));
            return NoContent();
        }

        [HttpPost("{appointmentId}/reschedule")]
        [Authorize(Policy = "Permission:appointment.reschedule")]
        public async Task<IActionResult> Reschedule([FromRoute] Guid appointmentId, [FromBody] RescheduleAppointmentRequest request)
        {
            await _sender.Send(new RescheduleAppointmentCommand(appointmentId, request.NewTimeSlot));
            return NoContent();
        }

        [HttpPost("{appointmentId}/cancel")]
        [Authorize(Policy = "Permission:appointment.cancel.own,appointment.cancel.any")]
        public async Task<IActionResult> Cancel([FromRoute] Guid appointmentId)
        {
            await _sender.Send(new CancelAppointmentCommand(appointmentId));
            return NoContent();
        }
    }

    public record RescheduleAppointmentRequest(DateTime NewTimeSlot);

    public record CreateAppointmentRequest(Guid WorkScheduleId, DateTime TimeSlot, string? Reason);

    [ApiController]
    [Route("/me/appointments")]
    public class MeAppointmentsController : ControllerBase
    {
        private readonly ISender _sender;

        public MeAppointmentsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        [Authorize(Policy = "Permission:appointment.view")]
        public async Task<IActionResult> GetMine()
        {
            var result = await _sender.Send(new GetMyAppointmentsQuery());
            return Ok(result);
        }
    }
}
using Clinic.Application.Features.Appointments.Commands;
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
        public IActionResult Create()
        {
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPatch("{appointmentId}")]
        [Authorize(Policy = "Permission:appointment.update")]
        public IActionResult Patch([FromRoute] string appointmentId)
        {
            return NoContent();
        }

        [HttpPost("{appointmentId}/confirm")]
        [Authorize(Policy = "Permission:appointment.confirm")]
        public IActionResult Confirm([FromRoute] string appointmentId) => NoContent();

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

    [ApiController]
    [Route("/me/appointments")]
    public class MeAppointmentsController : ControllerBase
    {
        [HttpGet]
        [Authorize(Policy = "Permission:appointment.view")]
        public IActionResult GetMine()
        {
            return Ok(new { message = "my appointments" });
        }
    }
}
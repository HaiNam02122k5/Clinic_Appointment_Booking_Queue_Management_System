using Clinic.API.Models;
using Clinic.Application.Contracts;
using Clinic.Application.Features.Appointments.Commands;
using Clinic.Application.Features.Appointments.Queries;
using Clinic.Application.Interfaces;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.API.Controllers
{
    [ApiController]
    [Route("/receptionist")]
    public class ReceptionistController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;

        public ReceptionistController(ISender sender, IMapper mapper, ICurrentUser currentUser)
        {
            _sender = sender;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        // For receptionist only
        [HttpPost("appointments")]
        [Authorize(Policy = "Permission:appointment.create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateAppointmentForPatient([FromBody] ReceptionistCreateAppointmentRequest request)
        {
            var userId = _currentUser.UserId;
            var command = new CreateAppointmentCommand(userId, request.WorkScheduleId, request.TimeSlot, request.Reason, request.PatientId);
            await _sender.Send(command);
            return StatusCode(StatusCodes.Status201Created);
        }

        // For receptionist only
        [HttpPatch("appointments/{appointmentId}")]
        [Authorize(Policy = "Permission:appointment.update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Patch([FromRoute] Guid appointmentId, [FromBody] UpdateAppointmentRequest request)
        {
            var userId = _currentUser.UserId;
            if (userId == null)
                return Unauthorized();
            var command = new UpdateAppointmentCommand((Guid)userId, appointmentId, request.NewWorkScheduleId, request.TimeSlot, request.Reason);
            await _sender.Send(command);
            return NoContent();
        }

        // Later can fill queue management, patient management, etc. for receptionist below
    }
}
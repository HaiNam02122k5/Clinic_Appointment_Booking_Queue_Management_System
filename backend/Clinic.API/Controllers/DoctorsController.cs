using Clinic.Application.Features.Queue.Commands;
using Clinic.Application.Features.Queue.Queries;
using Clinic.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.API.Controllers
{
    [ApiController]
    [Route("/doctors")]
    public class DoctorsController : ControllerBase
    {
        private readonly ICurrentUser _currentUser;
        private readonly ISender _sender;

        public DoctorsController(ICurrentUser currentUser, ISender sender)
        {
            _currentUser = currentUser;
            _sender = sender;
        }

        [HttpGet]
        [Authorize(Policy = "Permission:doctor.view")]
        public IActionResult GetAll()
        {
            return Ok(new { message = "list doctors" });
        }

        [HttpGet("{doctorId}")]
        [Authorize(Policy = "Permission:doctor.view")]
        public IActionResult GetById([FromRoute] string doctorId)
        {
            return Ok(new { id = doctorId });
        }

        [HttpPost]
        [Authorize(Policy = "Permission:doctor.create")]
        public IActionResult Create()
        {
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPut("{doctorId}")]
        [Authorize(Policy = "Permission:doctor.edit.any")]
        public IActionResult Update([FromRoute] string doctorId)
        {
            return NoContent();
        }

        [HttpPut("me")]
        [Authorize(Policy = "Permission:doctor.edit.own")]
        public IActionResult UpdateOwnProfile()
        {
            var userId = _currentUser.UserId;
            return NoContent();
        }

        [HttpPatch("{doctorId}/status")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateStatus([FromRoute] string doctorId)
        {
            return NoContent();
        }

        [HttpPost("{doctorId}/shifts")]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateShift([FromRoute] string doctorId)
        {
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpGet("{doctorId}/queue")]
        [Authorize(Policy = "Permission:doctor.queue.view,queue.view")]
        public async Task<IActionResult> GetQueue([FromRoute] Guid doctorId)
        {
            var result = await _sender.Send(new GetQueueByDoctorQuery(doctorId));
            return Ok(result);
        }

        [HttpPost("{doctorId}/queue/next")]
        [Authorize(Policy = "Permission:queue.call-next")]
        public async Task<IActionResult> CallNext([FromRoute] Guid doctorId)
        {
            var result = await _sender.Send(new CallNextQueueCommand(doctorId));
            return Ok(result);
        }
    }
}
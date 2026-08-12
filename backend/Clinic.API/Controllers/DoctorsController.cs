using Clinic.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.API.Controllers
{
    [ApiController]
    [Route("/doctors")]
    public class DoctorsController : ControllerBase
    {
        private readonly ICurrentUser _currentUser;

        public DoctorsController(ICurrentUser currentUser)
        {
            _currentUser = currentUser;
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
        [Authorize(Policy = "Permission:doctor.queue.view")]
        public IActionResult GetQueue([FromRoute] string doctorId)
        {
            return Ok(new { doctorId });
        }

        [HttpPost("{doctorId}/queue/next")]
        [Authorize(Policy = "Permission:queue.call-next")]
        public IActionResult CallNext([FromRoute] string doctorId)
        {
            return Ok(new { doctorId });
        }
    }
}
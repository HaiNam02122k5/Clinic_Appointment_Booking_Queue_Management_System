using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.API.Controllers
{
    [ApiController]
    [Route("/appointments")]
    public class AppointmentsController : ControllerBase
    {
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

        [HttpPost("{appointmentId}/cancel")]
        [Authorize(Policy = "Permission:appointment.cancel")]
        public IActionResult Cancel([FromRoute] string appointmentId)
        {
            return NoContent();
        }
    }

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

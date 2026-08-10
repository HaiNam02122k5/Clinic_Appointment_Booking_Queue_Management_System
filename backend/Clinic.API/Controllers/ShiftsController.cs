using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.API.Controllers
{
    [ApiController]
    [Route("/shifts")]
    public class ShiftsController : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAll()
        {
            return Ok(new { message = "list shifts" });
        }

        [HttpPut("{shiftId}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Update([FromRoute] string shiftId)
        {
            return NoContent();
        }

        [HttpPost("{shiftId}/cancel")]
        [Authorize(Roles = "Admin")]
        public IActionResult Cancel([FromRoute] string shiftId)
        {
            return NoContent();
        }

        [HttpPost("suggestions")]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateSuggestion()
        {
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPatch("suggestions/{suggestionId}")]
        [Authorize(Roles = "Admin")]
        public IActionResult PatchSuggestion([FromRoute] string suggestionId)
        {
            return NoContent();
        }

        [HttpDelete("suggestions")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteSuggestion()
        {
            return NoContent();
        }
    }
}

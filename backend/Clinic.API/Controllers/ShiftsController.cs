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
        [Authorize(Policy = "Permission:shift.manage")]
        public IActionResult Update([FromRoute] string shiftId)
        {
            return NoContent();
        }

        [HttpPost("{shiftId}/cancel")]
        [Authorize(Policy = "Permission:shift.manage")]
        public IActionResult Cancel([FromRoute] string shiftId)
        {
            return NoContent();
        }

        [HttpPost("suggestions")]
        [Authorize(Policy = "Permission:shift.self-manage")]
        public IActionResult CreateSuggestion()
        {
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPatch("suggestions/{suggestionId}")]
        [Authorize(Policy = "Permission:shift.suggestion.manage")]
        public IActionResult PatchSuggestion([FromRoute] string suggestionId)
        {
            return NoContent();
        }

        [HttpDelete("suggestions")]
        [Authorize(Policy = "Permission:shift.suggestion.manage")]
        public IActionResult DeleteSuggestion()
        {
            return NoContent();
        }
    }
}
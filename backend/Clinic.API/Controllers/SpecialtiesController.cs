using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.API.Controllers
{
    [ApiController]
    [Route("/specialties")]
    public class SpecialtiesController : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAll()
        {
            return Ok(new { message = "list specialties" });
        }

        [HttpGet("{specialtyId}")]
        [AllowAnonymous]
        public IActionResult GetById([FromRoute] string specialtyId)
        {
            return Ok(new { id = specialtyId });
        }

        [HttpPost]
        [Authorize(Policy = "Permission:specialty.manage")]
        public IActionResult Create()
        {
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPut("{specialtyId}")]
        [Authorize(Policy = "Permission:specialty.manage")]
        public IActionResult Update([FromRoute] string specialtyId)
        {
            return NoContent();
        }

        [HttpPatch("{specialtyId}/status")]
        [Authorize(Policy = "Permission:specialty.manage")]
        public IActionResult UpdateStatus([FromRoute] string specialtyId)
        {
            return NoContent();
        }
    }
}

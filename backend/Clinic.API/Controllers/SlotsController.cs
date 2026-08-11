using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.API.Controllers
{
    [ApiController]
    [Route("/slots")]
    public class SlotsController : ControllerBase
    {
        [HttpGet]
        [Authorize(Policy = "Permission:slot.view")]
        public IActionResult GetAll()
        {
            return Ok(new { message = "list slots" });
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.API.Controllers
{
    [ApiController]
    [Route("/admin")]
    public class AdminController : ControllerBase
    {
        [HttpGet("ping")]
        [Authorize(Roles = "Admin")]
        public IActionResult Ping()
        {
            return Ok(new { message = "pong" });
        }

        [HttpGet("roles")]
        [Authorize(Policy = "Permission:role.manage")]
        public IActionResult Roles()
        {
            // placeholder: in real app return roles/permissions management data
            return Ok(new { message = "roles endpoint - requires role.manage permission" });
        }
    }
}

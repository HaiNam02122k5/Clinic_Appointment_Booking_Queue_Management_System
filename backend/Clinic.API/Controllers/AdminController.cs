using Clinic.Application.Features;
using Clinic.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.API.Controllers
{
    [ApiController]
    [Route("/admin")]
    public class AdminController : ControllerBase
    {
        private readonly ISender _sender;

        public AdminController(ISender sender)
        {
            _sender = sender;
        }

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

        //[HttpPost("test-noti")]
        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> TestNoti([FromBody] TestNotiRequest request)
        //{
        //    var command = new TestNotiCommand(request.Message, request.Channel);
        //    await _sender.Send(command);
        //    return Ok(new { message = "Test notification endpoint - requires Admin role" });
        //}
    }

    //public class TestNotiRequest
    //{
    //    public string Message { get; set; }
    //    public NotificationChannel Channel { get; set; }
    //}
}

using Clinic.API.Models;
using Clinic.Application.Contracts;
using Clinic.Application.Features.Users.Queries;
using MapsterMapper;
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
        private readonly IMapper _mapper;

        public AdminController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
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

        [HttpGet("users")]
        [Authorize(Policy = "Permission:user.manage")]
        [ProducesResponseType(typeof(PaginationResponse<UserSummaryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllUsers([FromBody] UsersQueryRequest request)
        {
            var command = _mapper.Map<UsersQueryRequest, GetUsersQuery>(request);
            var result = await _sender.Send(command);
            return Ok(result);
        }

        [HttpGet("users/{userId}")]
        [Authorize(Policy = "Permission:user.manage")]
        [ProducesResponseType(typeof(UserDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            var command = new GetUserQuery(userId);
            var result = await _sender.Send(command);
            return Ok(result);
        }
    }
}

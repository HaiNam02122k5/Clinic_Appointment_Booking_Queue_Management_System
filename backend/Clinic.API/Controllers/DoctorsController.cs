using Clinic.API.Models;
using Clinic.Application.Contracts;
using Clinic.Application.Features.Doctors.Commands;
using Clinic.Application.Features.Doctors.Queries;
using Clinic.Application.Features.WorkSchedules.Commands;
using Clinic.Application.Features.WorkSchedules.Queries;
using Clinic.Application.Interfaces;
using Clinic.Domain.Enums;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace Clinic.API.Controllers
{
    [ApiController]
    [Route("/doctors")]
    public class DoctorsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ISender _sender;
        private readonly ICurrentUser _currentUser;

        public DoctorsController(IMapper mapper, ISender sender, ICurrentUser currentUser)
        {
            _mapper = mapper;
            _sender = sender;
            _currentUser = currentUser;
        }

        [HttpGet]
        [Authorize(Policy = "Permission:doctor.view.any")]
        [ProducesResponseType(typeof(PaginationResponse<DoctorSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAll([FromQuery] DoctorsQueryRequest request)
        {
            var query = _mapper.Map<GetDoctorsQuery>(request);
            var result = await _sender.Send(query);
            return Ok(result);
        }

        [HttpGet("{doctorId}")]
        [Authorize(Policy = "Permission:doctor.view.any")]
        [ProducesResponseType(typeof(DoctorDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromRoute] Guid doctorId)
        {
            var query = _mapper.Map<GetDoctorQuery>(new { DoctorId = doctorId });
            var result = await _sender.Send(query);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet("me")]
        [Authorize(Policy = "Permission:doctor.view.own")]
        [ProducesResponseType(typeof(DoctorDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetOwnProfile()
        {
            var doctorId = _currentUser.DoctorId;
            var query = _mapper.Map<GetDoctorQuery>(new { DoctorId = doctorId });
            var result = await _sender.Send(query);
            if (result == null)
            {
                return Unauthorized();
            }
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "Permission:doctor.create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] CreateDoctorRequest request)
        {
            var command = _mapper.Map<CreateDoctorCommand>(request);
            var result = await _sender.Send(command);
            return CreatedAtAction(nameof(GetById), new { doctorId = result.Id }, result);
        }

        [HttpPost("from-user")]
        [Authorize(Policy = "Permission:doctor.create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateFromUser([FromBody] CreateDoctorFromUserRequest request)
        {
            var command = _mapper.Map<CreateDoctorFromUserCommand>(request);
            var result = await _sender.Send(command);
            return CreatedAtAction(nameof(GetById), new { doctorId = result.Id }, result);
        }

        [HttpPut("{doctorId}")]
        [Authorize(Policy = "Permission:doctor.edit.any")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Update([FromRoute] Guid doctorId, [FromBody] UpdateDoctorRequest request)
        {
            var userId = _currentUser.UserId;
            var command = new UpdateDoctorCommand(userId, request.FullName, request.PhoneNumber, request.Email, request.DateOfBirth, request.Gender, request.Address, request.LicenseNumber, request.Qualification, request.ExperienceYears, request.Biography, doctorId);
            await _sender.Send(command);    
            return NoContent();
        }

        [HttpPut("me")]
        [Authorize(Policy = "Permission:doctor.edit.own")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateOwnProfile([FromBody] UpdateDoctorRequest request)
        {
            var userId = _currentUser.UserId;
            var command = new UpdateDoctorCommand(userId, request.FullName, request.PhoneNumber, request.Email, request.DateOfBirth, request.Gender, request.Address, request.LicenseNumber, request.Qualification, request.ExperienceYears, request.Biography);
            await _sender.Send(command);
            return NoContent();
        }

        [HttpPatch("{doctorId}/status")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateStatus([FromRoute] Guid doctorId, [FromBody] DoctorStatus status)
        {
            var command = new UpdateDoctorStatusCommand(doctorId, status);
            await _sender.Send(command);
            return NoContent();
        }

        [HttpPatch("{doctorId}/specialty")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> ChangeSpecialty([FromRoute] Guid doctorId, [FromBody] Guid newSpecialtyId)
        {
            var command = new ChangeSpecialtyCommand(doctorId, newSpecialtyId);
            await _sender.Send(command);
            return NoContent();
        }

        [HttpGet("{doctorId}/shifts")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(DoctorScheduleDto<WorkScheduleDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllShifts([FromRoute] Guid doctorId, [FromQuery] GetShiftsQuery query)
        {
            var command = new GetDoctorSchedulesQuery(doctorId, query.StartDate, query.EndDate);
            var result = await _sender.Send(command);
            return Ok(result);
        }

        [HttpPost("{doctorId}/shifts")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(WorkScheduleDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateShift([FromRoute] Guid doctorId, [FromBody] CreateShiftRequest request)
        {
            // In another branch
            var command = new AddDoctorScheduleCommand(doctorId, request.StartTime, request.EndTime, request.PatientLimitPerSlot);
            var result = await _sender.Send(command);
            return Created((string?)null, result);
        }

        [HttpGet("{doctorId}/suggestions")]
        [Authorize(Policy = "Permission:shift.suggestion.manage")]
        public async Task<IActionResult> GetAllSuggestions([FromRoute] Guid doctorId, [FromQuery] GetShiftsQuery query)
        {
            var userId = _currentUser.UserId;
            var command = new GetDoctorRequestedShiftsQuery(userId, query.StartDate, query.EndDate, doctorId);
            var schedules = await _sender.Send(command);
            return Ok(schedules);
        }

        [HttpGet("{doctorId}/queue")]
        [Authorize(Policy = "Permission:doctor.queue.view")]
        public IActionResult GetQueue([FromRoute] string doctorId)
        {
            // Not my job
            return Ok(new { doctorId });
        }

        [HttpPost("{doctorId}/queue/next")]
        [Authorize(Policy = "Permission:queue.call-next")]
        public IActionResult CallNext([FromRoute] string doctorId)
        {
            // Not my job
            return Ok(new { doctorId });
        }
    }
}
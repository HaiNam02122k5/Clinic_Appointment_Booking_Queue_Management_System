using Clinic.API.Models;
using Clinic.Application.Contracts;
using Clinic.Application.Features.Specialties.Commands;
using Clinic.Application.Features.Specialties.Queries;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.API.Controllers
{
    [ApiController]
    [Route("/specialties")]
    public class SpecialtyController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public SpecialtyController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(List<SpecialtyDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSpecialties([FromQuery] GetSpecialtiesRequest request)
        {
            var command = _mapper.Map<GetSpecialtiesQuery>(request);
            var result = await _sender.Send(command);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(SpecialtyDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateSpecialty([FromBody] CreateSpecialtyRequest request)
        {
            var command = _mapper.Map<CreateSpecialtyCommand>(request);
            var result = await _sender.Send(command);
            return CreatedAtAction(nameof(GetSpecialties), result.Id, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(SpecialtyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateSpecialty(Guid id, [FromBody] UpdateSpecialtyRequest request)
        {
            var command = _mapper.Map<UpdateSpecialtyCommand>(new
            {
                Id = id,
                request.Name,
                request.Description,
                request.EstablishedDate
            });
            var result = await _sender.Send(command);
            return Ok(result);
        }

        [HttpPost("{id}/delete")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteSpecialty(Guid id)
        {
            var command = _mapper.Map<DeleteSpecialtyCommand>(new { Id = id });
            var result = await _sender.Send(command);
            return NoContent();
        }
    }
}

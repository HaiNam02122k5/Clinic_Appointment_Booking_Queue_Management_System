using Clinic.Application.Common.Exceptions;
using Clinic.Application.Contracts;
using Clinic.Application.Interfaces;
using Clinic.Domain.Enums;
using MediatR;

namespace Clinic.Application.Features.Doctors.Queries
{
    public record GetDoctorQuery(Guid Id) : IRequest<DoctorDto>;

    public class GetDoctorQueryHandler : IRequestHandler<GetDoctorQuery, DoctorDto>
    {
        private readonly IDoctorRepository _doctorRepository;

        public GetDoctorQueryHandler(IDoctorRepository doctorRepository)
        {
            _doctorRepository = doctorRepository;
        }

        public async Task<DoctorDto> Handle(GetDoctorQuery request, CancellationToken cancellationToken)
        {
            var doctor = await _doctorRepository.GetByIdAsync(request.Id)
                ?? throw new NotFoundException($"Doctor '{request.Id}' not found.");

            var currentWorkHistory = doctor.WorkHistories.FirstOrDefault(wh => wh.Status == WorkHistoryStatus.Active);

            return new DoctorDto
            {
                Id = doctor.Id,
                FullName = doctor.Employee?.Person?.FullName ?? string.Empty,
                Qualification = doctor.Qualification,
                ExperienceYears = doctor.ExperienceYears,
                Biography = doctor.Biography,
                SpecialtyId = currentWorkHistory?.SpecialtyId,
                SpecialtyName = currentWorkHistory?.Specialty?.Name,
                Status = doctor.Status.ToString()
            };
        }
    }
}
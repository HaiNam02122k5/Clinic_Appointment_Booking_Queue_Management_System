using Clinic.Application.Contracts;
using Clinic.Application.Interfaces;
using Clinic.Domain.Enums;
using MediatR;

namespace Clinic.Application.Features.Doctors.Queries
{
    public record GetDoctorsQuery(
        string? Search = null,
        Guid? SpecialtyId = null,
        int Page = 1,
        int PageSize = 10
    ) : IRequest<PaginationResponse<DoctorDto>>;

    public class GetDoctorsQueryHandler : IRequestHandler<GetDoctorsQuery, PaginationResponse<DoctorDto>>
    {
        private readonly IDoctorRepository _doctorRepository;

        public GetDoctorsQueryHandler(IDoctorRepository doctorRepository)
        {
            _doctorRepository = doctorRepository;
        }

        public async Task<PaginationResponse<DoctorDto>> Handle(GetDoctorsQuery request, CancellationToken cancellationToken)
        {
            var doctors = await _doctorRepository.GetPagedAsync(request.Search, request.SpecialtyId, request.Page, request.PageSize);

            var doctorDtos = doctors.Items.Select(d =>
            {
                var currentWorkHistory = d.WorkHistories.FirstOrDefault(wh => wh.Status == WorkHistoryStatus.Active);

                return new DoctorDto
                {
                    Id = d.Id,
                    FullName = d.Employee?.Person?.FullName ?? string.Empty,
                    Qualification = d.Qualification,
                    ExperienceYears = d.ExperienceYears,
                    Biography = d.Biography,
                    SpecialtyId = currentWorkHistory?.SpecialtyId,
                    SpecialtyName = currentWorkHistory?.Specialty?.Name,
                    Status = d.Status.ToString()
                };
            }).ToList();

            return new PaginationResponse<DoctorDto>
            {
                Items = doctorDtos,
                TotalCount = doctors.TotalCount,
                PageNumber = request.Page,
                PageSize = request.PageSize
            };
        }
    }
}
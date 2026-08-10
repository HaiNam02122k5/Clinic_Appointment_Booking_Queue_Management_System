using Clinic.Application.Common.Exceptions;
using Clinic.Application.Interfaces;
using MediatR;

namespace Clinic.Application.Features.Specialties.Commands
{
    // Use-case: Update a specialty
    public record UpdateSpecialtyCommand(Guid Id, string Name, string Description, DateOnly EstablishedDate) : IRequest<Guid>;
    public class UpdateSpecialtyCommandHandler : IRequestHandler<UpdateSpecialtyCommand, Guid>
    {
        private readonly ISpecialtyRepository _specialtyRepository;
        private readonly IUnitOfWork _unitOfWork;
        public UpdateSpecialtyCommandHandler(ISpecialtyRepository specialtyRepository, IUnitOfWork unitOfWork)
        {
            _specialtyRepository = specialtyRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Guid> Handle(UpdateSpecialtyCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new ArgumentException("Specialty name cannot be empty.");
            }
            var specialty = await _specialtyRepository.GetByIdAsync(request.Id);
            if (specialty == null)
            {
                throw new NotFoundException("Specialty not found.");
            }
            if (await _specialtyRepository.GetByNameAsync(request.Name) is not null)
            {
                throw new ArgumentException($"Specialty {request.Name} already exists.");
            }
            specialty.Update(request.Name, request.Description, request.EstablishedDate);
            await _specialtyRepository.UpdateAsync(specialty);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return specialty.Id;
        }
    }
}

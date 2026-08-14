using Clinic.Application.Common.Exceptions;
using Clinic.Application.Interfaces;
using Clinic.Domain.Enums;
using MediatR;

namespace Clinic.Application.Features.Doctors.Commands
{
    // Use-case: Update an existing doctor
    public record UpdateDoctorCommand(
        Guid? UserId,
        string FullName,
        string PhoneNumber,
        string Email,
        DateOnly DateOfBirth,
        Gender Gender,
        string Address,
        string LicenseNumber,
        string Qualification,
        int ExperienceYears,
        string? Biography,
        Guid? DoctorId = null) : IRequest<Guid>;
    public class UpdateDoctorCommandHandler : IRequestHandler<UpdateDoctorCommand, Guid>
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateDoctorCommandHandler(IDoctorRepository doctorRepository, IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _doctorRepository = doctorRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Guid> Handle(UpdateDoctorCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                throw new UnauthorizedAccessException();
            }
            if (user.UserRoles.All(ur => ur.Role.Name != "Admin") && request.DoctorId != null)
            {
                throw new ForbiddenException("You are not authorized to update this information.");
            }
            var doctor = request.DoctorId == null ? user.Person.Employee?.Doctor : await _doctorRepository.GetInfoByIdAsync(request.DoctorId);
            if (doctor == null)
            {
                throw new NotFoundException("Doctor not found.");
            }
            // Update the doctor's properties
            doctor.UpdateInfo(request.LicenseNumber, request.Qualification, request.ExperienceYears, request.Biography);

            doctor.Employee.Person.UpdateAdvancedDetails(request.FullName, request.PhoneNumber, request.Email, request.Gender, request.DateOfBirth, request.Address);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return doctor.Id;
        }
    }
}

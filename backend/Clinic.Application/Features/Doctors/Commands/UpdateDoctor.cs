using Clinic.Application.Common.Exceptions;
using Clinic.Application.Interfaces;
using Clinic.Domain.Enums;
using MediatR;

namespace Clinic.Application.Features.Doctors.Commands
{
    // Use-case: Update an existing doctor
    public record UpdateDoctorCommand(
        Guid DoctorId,
        string FullName,
        string PhoneNumber,
        string Email,
        DateOnly DateOfBirth,
        Gender Gender,
        string Address,
        string LicenseNumber,
        string Qualification,
        DoctorStatus DoctorStatus,
        int ExperienceYears,
        string Biography) : IRequest<Guid>;
    public class UpdateDoctorCommandHandler : IRequestHandler<UpdateDoctorCommand, Guid>
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateDoctorCommandHandler(IDoctorRepository doctorRepository, IUnitOfWork unitOfWork)
        {
            _doctorRepository = doctorRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Guid> Handle(UpdateDoctorCommand request, CancellationToken cancellationToken)
        {
            var doctor = await _doctorRepository.GetInfoByIdAsync(request.DoctorId);
            if (doctor == null)
            {
                throw new NotFoundException("Doctor not found.");
            }

            // Update the doctor's properties
            doctor.UpdateInfo(request.LicenseNumber, request.Qualification, request.ExperienceYears, request.Biography);
            doctor.UpdateStatus(request.DoctorStatus);

            doctor.Employee.Person.UpdateAdvancedDetails(request.FullName, request.PhoneNumber, request.Email, request.Gender, request.DateOfBirth, request.Address);

            await _unitOfWork.SaveChangesAsync();
            return doctor.Id;
        }
    }
}

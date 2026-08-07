using Clinic.Application.Common.Exceptions;
using Clinic.Application.Interfaces;
using Clinic.Application.Services;
using Clinic.Domain.Enums;
using MediatR;

namespace Clinic.Application.Features.Auth.Commands
{
    public record RegisterCommand(
        string Username,
        string Password,
        string FullName,
        string PhoneNumber,
        string Email,
        DateOnly DateOfBirth,
        Gender Gender,
        string Address
    ) : IRequest<Guid>;
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Guid>
    {
        private readonly UserService _userService;
        private readonly PersonService _personService;
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IPersonRepository personRepository, IRoleRepository roleRepository, IUnitOfWork unitOfWork)
        {
            _userService = new UserService(userRepository, passwordHasher);
            _personService = new PersonService(personRepository);
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(RegisterCommand command, CancellationToken cancellationToken)
        {
            // Create a new person entity
            var newPerson = await _personService.CreateOrGetPersonAsync(command.FullName, command.PhoneNumber, command.Email, command.DateOfBirth, command.Gender, command.Address);

            // Create a new user entity and associate it with the person
            var existingUser = await _userService.CreateUserAsync(command.Username, command.Password, newPerson);

            // Assign the "Patient" role to the new user
            existingUser.AssignRole(await _roleRepository.GetByNameAsync("Patient"));

            // TODO: Create a new patient entity and associate it with the person

            // Create a new user entity
            await _unitOfWork.SaveChangesAsync();
            
            return existingUser.Id;
        }
    }
}

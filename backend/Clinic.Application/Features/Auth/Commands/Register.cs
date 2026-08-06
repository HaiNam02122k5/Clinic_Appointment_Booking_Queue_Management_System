using Clinic.Domain.Entities;
using Clinic.Domain.Enums;
using Clinic.Domain.Interfaces;
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
    );
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Guid>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPersonRepository _personRepository;
        private readonly IPasswordHasher _passwordHasher;

        public RegisterCommandHandler(IUserRepository userRepository, IPersonRepository personRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _personRepository = personRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<Guid> Handle(RegisterCommand command, CancellationToken cancellationToken)
        {
            // Check if a user with the same username already exists
            var existingUser = await _userRepository.GetByUsernameAsync(command.Username);
            if (existingUser != null)
            {
                throw new Exception("Username already exists.");
            }

            // Check if a person with the same phone number already exists
            // The clinic uses phone number as a unique identifier for patients.
            // If a patient already exists with the same phone number, link that profile to the new user account. If not, create a new person profile.
            Person outPerson;

            var existingPersonByPhone = await _personRepository.GetByPhoneNumberAsync(command.PhoneNumber);
            if (existingPersonByPhone != null)
            {
                // If the email belongs to another person, don't allow registration.
                if (!string.IsNullOrWhiteSpace(command.Email) && !string.Equals(existingPersonByPhone.Email, command.Email))
                {
                    var existingPersonByEmail = await _personRepository.GetByEmailAsync(command.Email);
                    if (existingPersonByEmail != null)
                    {
                        throw new Exception("Email has been taken");
                    }
                }

                // Override the person's email if not empty
                existingPersonByPhone.Email = command.Email;
                outPerson = await _personRepository.UpdateAsync(existingPersonByPhone);
            }
            else
            {
                var existingPersonByEmail = await _personRepository.GetByEmailAsync(command.Email);
                if (existingPersonByEmail != null)
                {
                    throw new Exception("Email has been taken");
                }
                var person = new Person
                {
                    FullName = command.FullName,
                    PhoneNumber = command.PhoneNumber,
                    Email = command.Email,
                    DateOfBirth = command.DateOfBirth,
                    Gender = command.Gender,
                    Address = command.Address
                };
                // Add the person to the repository
                outPerson = await _personRepository.AddAsync(person);
            }


            // Create a new user entity
            var user = new User
            {
                Username = command.Username,
                PasswordHash = _passwordHasher.HashPassword(command.Password),
                PersonId = outPerson.Id
            };

            // Add the user to the repository
            var addedUser = await _userRepository.AddAsync(user);

            return addedUser.Id;
        }
    }
}

using Clinic.Application.Interfaces;
using Clinic.Domain.Common.Exceptions;
using MediatR;

namespace Clinic.Application.Features.Users.Commands
{
    public class UpdateUserStatusCommandHandler
        : IRequestHandler<UpdateUserStatusCommand>
    {
        private readonly IUserRepository _userRepository;

        public UpdateUserStatusCommandHandler(
            IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task Handle(
            UpdateUserStatusCommand request,
            CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);

            if (user == null)
            {
                throw new KeyNotFoundException(
                    $"User with ID '{request.UserId}' was not found.");
            }

            user.ChangeStatus(request.IsActive);

            await _userRepository.UpdateAsync(user);
        }
    }
}
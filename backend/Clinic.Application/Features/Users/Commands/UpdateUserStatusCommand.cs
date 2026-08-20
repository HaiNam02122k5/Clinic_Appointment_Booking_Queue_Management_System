using MediatR;

namespace Clinic.Application.Features.Users.Commands
{
    public record UpdateUserStatusCommand(
        Guid UserId,
        bool IsActive
    ) : IRequest;
}
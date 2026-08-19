using Clinic.Application.Common.Exceptions;
using Clinic.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Clinic.Application.Features.Auth.Commands
{
    public record ForgotPasswordCommand(string PhoneNumberOrEmail) : IRequest<int>;
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, int>
    {
        private readonly IPersonRepository _personRepository;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;
        //private readonly INotificationQueue _notificationQueue;
        public ForgotPasswordCommandHandler(IPersonRepository personRepository, IUserService userService, IUnitOfWork unitOfWork)
        {
            _personRepository = personRepository;
            _userService = userService;
            _unitOfWork = unitOfWork;
            //_notificationQueue = notificationQueue;
        }

        public async Task<int> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _personRepository.GetByPhoneNumberAsync(request.PhoneNumberOrEmail);
            if (user == null)
            {
                user = await _personRepository.GetByEmailAsync(request.PhoneNumberOrEmail);
                if (user == null)
                {
                    return -1; // No spoil user info
                }
            }

            if (user.User == null)
            {
                return -1; // No spoil user info
            }

            // Temporary way to reset password.
            const string chars =
                "abcdefghijklmnopqrstuvwxyz" +
                "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
                "0123456789" +
                "!@#$%^&*";

            var randomPassword = new char[15];
            for (int i = 0; i < randomPassword.Length; i++)
            {
                randomPassword[i] = chars[RandomNumberGenerator.GetInt32(chars.Length)];
            }

            // Send a notification to user via email below
            // _notificationQueue.Enqueue(new NotificationJob<Person>(user, NotificationType.PasswordReset, new { NewPassword = new string(randomPassword) }, false, true, false));
            Console.WriteLine($"Temporary password for {user.FullName}: {new string(randomPassword)}");

            await _userService.UpdatePassword(user.User, new string(randomPassword));
            await _unitOfWork.SaveChangesAsync();

            return 0;
        }
    }
}

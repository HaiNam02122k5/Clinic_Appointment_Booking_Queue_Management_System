using Clinic.Application.Contracts;
using Clinic.Application.Interfaces;
using Clinic.Domain.Entities;
using Clinic.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.Application.Features.Employees.Commands
{
    // Use-case: Create a new employee from an existing user
    public record CreateEmployeeFromUserCommand(
        Guid UserId,
        DateOnly HireDate,
        List<string> Roles,
        string? Email = null,
        string? Address = null
    ) : IRequest<EmployeeSummaryDto>;
    public class CreateEmployeeFromUserCommandHandler : IRequestHandler<CreateEmployeeFromUserCommand, EmployeeSummaryDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateEmployeeFromUserCommandHandler(
            IUserRepository userRepository,
            IEmployeeRepository employeeRepository,
            IRoleRepository roleRepository,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _employeeRepository = employeeRepository;
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<EmployeeSummaryDto> Handle(CreateEmployeeFromUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                throw new ArgumentException($"User with ID '{request.UserId}' does not exist.");
            }
            if (string.IsNullOrWhiteSpace(user.Person.Email) && string.IsNullOrWhiteSpace(request.Email))
            {
                throw new ArgumentException("Employee must have an email address.");
            }
            if (string.IsNullOrWhiteSpace(user.Person.Address) && string.IsNullOrWhiteSpace(request.Address))
            {
                throw new ArgumentException("Employee must have an address.");
            }
            if (user.Person.Employee != null)
            {
                throw new InvalidOperationException($"User with ID '{request.UserId}' is already an employee.");
            }
            if (request.Roles == null || request.Roles.Count == 0)
            {
                throw new ArgumentException("At least one role must be assigned to the employee.");
            }
            user.Person.UpdateDetails(
                email: request.Email,
                gender: user.Person.Gender,
                address: request.Address
            );
            var employee = new Employee(user.Person, request.HireDate);
            // Add new roles
            foreach (var roleName in request.Roles)
            {
                Role role = await _roleRepository.GetByNameAsync(roleName.Trim());
                if (role == null)
                {
                    throw new ArgumentException($"Role '{roleName}' does not exist.");
                }
                user.AssignRole(role);
            }
            await _employeeRepository.AddAsync(employee);
            await _unitOfWork.SaveChangesAsync();
            return new EmployeeSummaryDto
            {
                Id = employee.Id,
                FullName = employee.Person.FullName,
                PhoneNumber = employee.Person.PhoneNumber,
                Email = employee.Person.Email,
                Gender = employee.Person.Gender,
                DateOfBirth = employee.Person.DateOfBirth,
                Status = employee.Status,
                Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList(),
            };
        }
    }
}

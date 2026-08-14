using Clinic.Application.Common.Exceptions;
using Clinic.Application.Interfaces;
using Clinic.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.Application.Features.Employees.Commands
{
    public record UpdateEmployeeCommand(
        Guid EmployeeId,
        string FullName,
        string PhoneNumber,
        string Email,
        DateOnly DateOfBirth,
        Gender Gender,
        string Address,
        EmployeeStatus Status,
        List<string> Roles
    ) : IRequest<Guid>;
    public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, Guid>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;
        public UpdateEmployeeCommandHandler(
            IEmployeeRepository employeeRepository,
            IRoleRepository roleRepository,
            IUnitOfWork unitOfWork)
        {
            _employeeRepository = employeeRepository;
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Guid> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
        {
            var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId);
            if (employee == null)
            {
                throw new NotFoundException("Employee not found.");
            }
            // Update the employee's properties
            employee.UpdateStatus(request.Status);
            employee.Person.UpdateAdvancedDetails(request.FullName, request.PhoneNumber, request.Email, request.Gender, request.DateOfBirth, request.Address);
            foreach (var roleName in request.Roles)
            {
                var role = await _roleRepository.GetByNameAsync(roleName);
                if (role == null)
                {
                    throw new ArgumentException($"Role '{roleName}' not found.");
                }
                if (!employee.Person.User.UserRoles.Any(r => r.Role.Name == roleName))
                {
                    employee.Person.User.AssignRole(role);
                }
            }
            var existingRoles = employee.Person.User!.UserRoles.Select(ur => ur.Role).ToList();
            foreach (var role in existingRoles)
            {
                if (!request.Roles.Contains(role.Name))
                {
                    employee.Person.User.RemoveRole(role);
                }
            }
            await _unitOfWork.SaveChangesAsync();
            return employee.Id;
        }
    }
}

using Clinic.Application.Common.Exceptions;
using Clinic.Application.Features.Employees.Commands;
using Clinic.Application.Interfaces;
using Clinic.Application.UnitTests.Common;
using Clinic.Domain.Entities;
using Clinic.Domain.Enums;
using System.Net;
using System.Net.NetworkInformation;

namespace Clinic.Application.UnitTests.Features.Employees.Commands
{
    public class UpdateEmployeeTests
    {
        [Fact]
        public async Task TestValidUpdateEmployeeCommand()
        {
            var employeeRepository = new FakeEmployeeRepository();
            var roleRepository = new FakeRoleRepository();
            var unitOfWork = new FakeUnitOfWork();
            var handler = new UpdateEmployeeCommandHandler(employeeRepository, roleRepository, unitOfWork);
            var person = TestDataFactory.CreatePerson();
            var user = TestDataFactory.CreateUser(person: person);
            var employee = TestDataFactory.CreateEmployee(person: person);
            
            await employeeRepository.AddAsync(employee);
            var command = new UpdateEmployeeCommand(
                EmployeeId: employee.Id,
                FullName: "New FName",
                PhoneNumber: "0111222333",
                Email: "newemail@example.com",
                DateOfBirth: DateOnly.FromDateTime(DateTime.Now.AddYears(-30)),
                Gender: Gender.Male,
                Address: "123 New Address",
                Status: EmployeeStatus.Active,
                Roles: new List<string> { "Receptionist" }
            );
            var result = await handler.Handle(command, CancellationToken.None);

            Assert.Equal("New FName", employee.Person.FullName);
            Assert.NotNull(employee.Person.User.UserRoles.FirstOrDefault(r => r.Role.Name == "Receptionist"));

            var command2 = new UpdateEmployeeCommand(
                EmployeeId: employee.Id,
                FullName: "New FName2",
                PhoneNumber: "0111222334",
                Email: "newemail2@example.com",
                DateOfBirth: DateOnly.FromDateTime(DateTime.Now.AddYears(-30)),
                Gender: Gender.Male,
                Address: "123 New Address",
                Status: EmployeeStatus.Active,
                Roles: new List<string> { "Receptionist", "Admin" }
            );
            var result2 = await handler.Handle(command2, CancellationToken.None);
            Assert.Equal("New FName2", employee.Person.FullName);
            Assert.NotNull(employee.Person.User.UserRoles.FirstOrDefault(r => r.Role.Name == "Receptionist"));
            Assert.NotNull(employee.Person.User.UserRoles.FirstOrDefault(r => r.Role.Name == "Admin"));
        }

        [Fact]
        public async Task TestUpdateNonExistentEmployeeCommand()
        {
            var employeeRepository = new FakeEmployeeRepository();
            var roleRepository = new FakeRoleRepository();
            var unitOfWork = new FakeUnitOfWork();
            var handler = new UpdateEmployeeCommandHandler(employeeRepository, roleRepository, unitOfWork);
            var command = new UpdateEmployeeCommand(
                EmployeeId: Guid.NewGuid(),
                FullName: "New FName",
                PhoneNumber: "0111222333",
                Email: "newemail@example.com",
                DateOfBirth: DateOnly.FromDateTime(DateTime.Now.AddYears(-30)),
                Gender: Gender.Male,
                Address: "123 New Address",
                Status: EmployeeStatus.Active,
                Roles: new List<string> { "Receptionist" }
            );
             await Assert.ThrowsAsync<NotFoundException>(async () => await handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task TestInvalidRoleUpdateEmployeeCommand()
        {
            var employeeRepository = new FakeEmployeeRepository();
            var roleRepository = new FakeRoleRepository();
            var unitOfWork = new FakeUnitOfWork();
            var handler = new UpdateEmployeeCommandHandler(employeeRepository, roleRepository, unitOfWork);
            var employee = TestDataFactory.CreateEmployee();
            await employeeRepository.AddAsync(employee);
            var command = new UpdateEmployeeCommand(
                EmployeeId: employee.Id,
                FullName: "New FName",
                PhoneNumber: "0111222333",
                Email: "newemail@example.com",
                DateOfBirth: DateOnly.FromDateTime(DateTime.Now.AddYears(-30)),
                Gender: Gender.Male,
                Address: "123 New Address",
                Status: EmployeeStatus.Active,
                Roles: new List<string> { "InvalidRole" }
            );
            await Assert.ThrowsAsync<ArgumentException>(async () => await handler.Handle(command, CancellationToken.None));
        }
    }
}

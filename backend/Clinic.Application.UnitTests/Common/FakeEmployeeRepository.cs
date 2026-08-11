using Clinic.Application.Interfaces;
using Clinic.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.Application.UnitTests.Common
{
    public class FakeEmployeeRepository : IEmployeeRepository
    {
        private readonly List<Employee> _employees = [];
        public async Task AddAsync(Employee employee)
        {
            _employees.Add(employee);
        }
    }
}

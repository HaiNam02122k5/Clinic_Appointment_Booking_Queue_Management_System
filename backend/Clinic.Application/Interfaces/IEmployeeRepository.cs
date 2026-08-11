using Clinic.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.Application.Interfaces
{
    public interface IEmployeeRepository
    {
        Task AddAsync(Employee employee);
    }
}

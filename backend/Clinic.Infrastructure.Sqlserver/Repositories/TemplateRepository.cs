using Clinic.Domain.Interfaces;
using Clinic.Infrastructure.Sqlserver.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.Infrastructure.Sqlserver.Repositories
{
    public class TemplateRepository : IUserRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public TemplateRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //... Implement repository methods here
    }
}

using Clinic.Infrastructure.Sqlserver.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.Infrastructure.Sqlserver.Configurations
{
    public class TemplateConfiguration : IEntityTypeConfiguration<TemplateDataModel>
    {
        public void Configure(EntityTypeBuilder<TemplateDataModel> builder)
        {
            // ... Configure the entity properties and relationships here
        }
    }
}

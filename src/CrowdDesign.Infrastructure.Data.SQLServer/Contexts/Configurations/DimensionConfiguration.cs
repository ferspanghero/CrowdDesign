using CrowdDesign.Core.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrowdDesign.Infrastructure.SQLServer.Contexts.Configurations
{
    public class DimensionConfiguration : EntityTypeConfiguration<Dimension>
    {
        public DimensionConfiguration()
        {
            Property(e => e.Name).IsRequired();

            Property(e => e.Description).IsRequired();

            HasRequired(e => e.Project).WithMany(e => e.Dimensions);
        }
    }
}

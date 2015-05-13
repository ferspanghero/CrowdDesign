using System.Data.Entity.ModelConfiguration;
using CrowdDesign.Core.Entities;

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

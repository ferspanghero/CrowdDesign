using System.Data.Entity.ModelConfiguration;
using CrowdDesign.Core.Entities;

namespace CrowdDesign.Infrastructure.SQLServer.Contexts.Configurations
{
    public class ProjectConfiguration : EntityTypeConfiguration<Project>
    {
        public ProjectConfiguration()
        {
            Property(e => e.Name).IsRequired();
            Property(e => e.Name).HasMaxLength(50);
        }
    }
}

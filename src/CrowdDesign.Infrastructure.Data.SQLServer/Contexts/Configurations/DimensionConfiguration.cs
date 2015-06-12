using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using CrowdDesign.Core.Entities;

namespace CrowdDesign.Infrastructure.SQLServer.Contexts.Configurations
{
    public class DimensionConfiguration : EntityTypeConfiguration<Dimension>
    {
        public DimensionConfiguration()
        {
            Property(e => e.Name).IsRequired();
            Property(e => e.Name).HasMaxLength(50);
            Property(e => e.Name).HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("IX_Name") { IsUnique = true }));

            Property(e => e.Description).IsRequired();
            Property(e => e.Description).HasMaxLength(200);

            HasRequired(e => e.Project).WithMany(e => e.Dimensions);
        }
    }
}

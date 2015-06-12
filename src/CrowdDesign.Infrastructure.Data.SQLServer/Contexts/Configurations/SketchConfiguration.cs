using System.Data.Entity.ModelConfiguration;
using CrowdDesign.Core.Entities;

namespace CrowdDesign.Infrastructure.SQLServer.Contexts.Configurations
{
    public class SketchConfiguration : EntityTypeConfiguration<Sketch>
    {
        public SketchConfiguration()
        {
            HasRequired(e => e.Dimension).WithMany(e => e.Sketches);

            HasRequired(e => e.User).WithMany(e => e.Sketches);

            Property(e => e.Title).IsRequired();
            Property(e => e.Title).HasMaxLength(25);
        }
    }
}

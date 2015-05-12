using CrowdDesign.Core.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrowdDesign.Infrastructure.SQLServer.Contexts.Configurations
{
    public class SketchConfiguration : EntityTypeConfiguration<Sketch>
    {
        public SketchConfiguration()
        {
            HasRequired(e => e.Dimension).WithMany(e => e.Sketches);

            HasRequired(e => e.User).WithMany(e => e.Sketches);
        }
    }
}

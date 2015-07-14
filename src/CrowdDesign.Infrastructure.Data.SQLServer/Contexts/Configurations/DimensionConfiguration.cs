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

            // IMPORTANT: the definition of the composite unique key that determines two dimensions from the same project cannot have the same name
            //            was moved to "201506280552052_UpdateDimensionUniqueKey.cs". The reason is that one of the columns in this unique key
            //            is the project Id. Since the Dimension class only has the navigation property "Project", EF requires that it had a direct Id
            //            property (something like "int Project_ID") to be able to include it in the unique key definition. Since we don't want to change
            //            the domain model because of a particular repository layer solution limitation, we are moving the creation of the key as an 
            //            explicit SQL instruction in the migration script.

            Property(e => e.Description).IsRequired();
            Property(e => e.Description).HasMaxLength(200);

            HasRequired(e => e.Project).WithMany(e => e.Dimensions);
        }
    }
}

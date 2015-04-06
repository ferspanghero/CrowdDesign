using CrowdDesign.Core.Entities;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace CrowdDesign.Infrastructure.SQLServer
{
    public class Database : DbContext
    {
        #region Properties
        public DbSet<Project> Projects { get; set; } 
        #endregion

        #region Methods
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();  

            base.OnModelCreating(modelBuilder);
        }
        #endregion
    }
}

using CrowdDesign.Core.Entities;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace CrowdDesign.Infrastructure.SQLServer
{
    public class Database : DbContext
    {
        #region Properties
        public DbSet<Project> Projects { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Sketch> Sketches { get; set; }
        #endregion

        #region Methods
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Sketch>()
                .HasRequired(e => e.Category)
                .WithMany()
                .WillCascadeOnDelete();

            base.OnModelCreating(modelBuilder);
        }
        #endregion
    }
}

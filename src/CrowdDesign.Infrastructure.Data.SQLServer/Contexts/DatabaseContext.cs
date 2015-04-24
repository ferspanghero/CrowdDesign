using System.Data.Entity;
using CrowdDesign.Core.Entities;

namespace CrowdDesign.Infrastructure.SQLServer.Contexts
{
    public class DatabaseContext : DbContext
    {
        #region Properties
        public DbSet<Project> Projects { get; set; }
        public DbSet<Dimension> Dimensions { get; set; }
        public DbSet<Sketch> Sketches { get; set; }
        public DbSet<User> Users { get; set; }
        #endregion

        #region Methods
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            #region Project configuration
            modelBuilder.Entity<Project>()
                .Property(e => e.Name)
                .IsRequired();
            #endregion

            #region Dimension configuration
            modelBuilder.Entity<Dimension>()
                .Property(e => e.Name)
                .IsRequired();

            modelBuilder.Entity<Dimension>()
                .Property(e => e.Description)
                .IsRequired();
            #endregion

            #region Sketch configuration
            modelBuilder.Entity<Sketch>()
                .HasRequired(e => e.Dimension)
                .WithMany()
                .WillCascadeOnDelete();
            #endregion

            base.OnModelCreating(modelBuilder);
        }
        #endregion
    }
}

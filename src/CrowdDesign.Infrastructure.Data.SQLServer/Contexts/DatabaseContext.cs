using System.Data.Entity;
using CrowdDesign.Core.Entities;

namespace CrowdDesign.Infrastructure.SQLServer.Contexts
{
    /// <summary>
    /// Emulates an in-memory representation of the database used by Entity Framework to manipulate data
    /// </summary>
    public class DatabaseContext : DbContext
    {
        #region Constructors

        public DatabaseContext()
        {
            Database.SetInitializer(new DatabaseInitializer());
        }
        #endregion

        #region Properties
        public DbSet<Project> Projects { get; set; }
        public DbSet<Dimension> Dimensions { get; set; }
        public DbSet<Sketch> Sketches { get; set; }
        public DbSet<User> Users { get; set; }
        #endregion

        #region Methods
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            #region Relationships and constraints configuration
            #region Projects
            modelBuilder.Entity<Project>()
                .Property(e => e.Name)
                .IsRequired();
            #endregion

            #region Dimensions
            modelBuilder.Entity<Dimension>()
                .Property(e => e.Name)
                .IsRequired();

            modelBuilder.Entity<Dimension>()
                .Property(e => e.Description)
                .IsRequired();
            #endregion

            #region Sketches
            modelBuilder.Entity<Sketch>()
                .HasRequired(e => e.Dimension)
                .WithMany()
                .WillCascadeOnDelete();
            #endregion
            #endregion

            base.OnModelCreating(modelBuilder);
        }
        #endregion
    }
}

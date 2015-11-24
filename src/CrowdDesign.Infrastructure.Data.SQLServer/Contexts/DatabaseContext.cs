using System.Data.Entity;
using System.Reflection;
using CrowdDesign.Core.Entities;
using CrowdDesign.Infrastructure.SQLServer.Migrations;

namespace CrowdDesign.Infrastructure.SQLServer.Contexts
{
    /// <summary>
    /// Emulates an in-memory representation of the database used by Entity Framework to manipulate data
    /// </summary>
    public class DatabaseContext : DbContext
    {
        #region Constructors

        public DatabaseContext()
            : base("DefaultConnection")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<DatabaseContext, Configuration>());
        }
        #endregion

        #region Properties
        public DbSet<Project> Projects { get; set; }
        public DbSet<Dimension> Dimensions { get; set; }
        public DbSet<Sketch> Sketches { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<DimensionVotingEntry> DimensionVotingEntries { get; set; }
        public DbSet<ProjectVotingEntry> ProjectVotingEntries { get; set; }
        #endregion

        #region Methods
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.AddFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }
        #endregion
    }
}

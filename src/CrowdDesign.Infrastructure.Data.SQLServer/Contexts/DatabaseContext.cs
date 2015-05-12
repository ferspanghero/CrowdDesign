using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using CrowdDesign.Core.Entities;
using System.Reflection;
using CrowdDesign.Infrastructure.SQLServer.Contexts.Configurations;

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
            modelBuilder.Configurations.AddFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }
        #endregion
    }
}

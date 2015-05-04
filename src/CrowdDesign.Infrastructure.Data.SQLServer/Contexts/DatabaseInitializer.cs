using System.Data.Entity;
using CrowdDesign.Core.Entities;

namespace CrowdDesign.Infrastructure.SQLServer.Contexts
{
    /// <summary>
    /// Represents a configuration that executes custom initialization code when the database is created for the first time.
    /// </summary>
    public class DatabaseInitializer : CreateDatabaseIfNotExists<DatabaseContext>
    {
        protected override void Seed(DatabaseContext context)
        {
            // Adds default users to the database
            // TODO: The password should be encrypted
            context.Users.Add
            (
                new User
                {
                    Username = "admin",
                    Password = "password",
                    IsAdmin = true
                }
            );

            context.Users.Add
            (
                new User
                {
                    Username = "user",
                    Password = "password",
                    IsAdmin = false
                }
            );

            base.Seed(context);
        }
    }
}

using CrowdDesign.Core.Entities;

namespace CrowdDesign.Infrastructure.SQLServer.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<CrowdDesign.Infrastructure.SQLServer.Contexts.DatabaseContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(CrowdDesign.Infrastructure.SQLServer.Contexts.DatabaseContext context)
        {
            // Adds default users to the database
            // TODO: The password should be encrypted
            context.Users.AddOrUpdate
                (
                    e => e.Username,
                    new User
                    {
                        Username = "admin",
                        Password = "password",
                        IsAdmin = true
                    }
                );

            context.Users.AddOrUpdate
                (
                    e => e.Username,
                    new User
                    {
                        Username = "user1",
                        Password = "password",
                        IsAdmin = false
                    }
                );

            context.Users.AddOrUpdate
                (
                    e => e.Username,
                    new User
                    {
                        Username = "user2",
                        Password = "password",
                        IsAdmin = false
                    }
                );

            context.SaveChanges();
        }
    }
}

using System.Data.Entity.Migrations;
using CrowdDesign.Core.Entities;
using CrowdDesign.Infrastructure.SQLServer.Contexts;

namespace CrowdDesign.Infrastructure.SQLServer.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<DatabaseContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(DatabaseContext context)
        {
            base.Seed(context);
        }
    }
}

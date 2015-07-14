namespace CrowdDesign.Infrastructure.SQLServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class UpdateDimensionUniqueKey : DbMigration
    {
        public override void Up()
        {
            // IMPORTANT: the definition of the composite unique key that determines two dimensions from the same project cannot have the same name
            //            was moved to this class. The reason is that one of the columns in this unique key is the project Id. Since the Dimension class 
            //            only has the navigation property "Project", EF requires that it had a direct Id property (something like "int Project_ID") 
            //            to be able to include it in the unique key definition. Since we don't want to change the domain model because of a particular 
            //            repository layer solution limitation, we are moving the creation of the key as an explicit SQL instruction in the migration script.

            DropIndex("dbo.Dimensions", new[] { "Name" });

            CreateIndex("dbo.Dimensions", new[] { "Name", "Project_Id" }, true, "IX_Name");
        }

        public override void Down()
        {
            DropIndex("dbo.Dimensions", new[] { "Name", "Project_Id" });
        }
    }
}

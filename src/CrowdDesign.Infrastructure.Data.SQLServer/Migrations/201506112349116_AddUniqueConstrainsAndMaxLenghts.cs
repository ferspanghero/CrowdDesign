namespace CrowdDesign.Infrastructure.SQLServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUniqueConstrainsAndMaxLenghts : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Dimensions", "Name", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Dimensions", "Description", c => c.String(nullable: false, maxLength: 200));
            AlterColumn("dbo.Projects", "Name", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Sketches", "Title", c => c.String(nullable: false, maxLength: 25));
            CreateIndex("dbo.Dimensions", "Name", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.Dimensions", new[] { "Name" });
            AlterColumn("dbo.Sketches", "Title", c => c.String(nullable: false));
            AlterColumn("dbo.Projects", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.Dimensions", "Description", c => c.String(nullable: false));
            AlterColumn("dbo.Dimensions", "Name", c => c.String(nullable: false));
        }
    }
}

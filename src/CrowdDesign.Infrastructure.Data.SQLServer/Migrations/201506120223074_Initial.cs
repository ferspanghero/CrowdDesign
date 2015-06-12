namespace CrowdDesign.Infrastructure.SQLServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Dimensions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Description = c.String(nullable: false, maxLength: 200),
                        SortCriteria = c.String(),
                        Project_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Projects", t => t.Project_Id, cascadeDelete: true)
                .Index(t => t.Name, unique: true)
                .Index(t => t.Project_Id);
            
            CreateTable(
                "dbo.Projects",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Sketches",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Data = c.String(),
                        ImageUri = c.String(),
                        Position = c.Int(nullable: false),
                        Title = c.String(nullable: false, maxLength: 25),
                        Dimension_Id = c.Int(nullable: false),
                        User_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Dimensions", t => t.Dimension_Id, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.User_Id, cascadeDelete: true)
                .Index(t => t.Dimension_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Username = c.String(),
                        Password = c.String(),
                        IsAdmin = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Sketches", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Sketches", "Dimension_Id", "dbo.Dimensions");
            DropForeignKey("dbo.Dimensions", "Project_Id", "dbo.Projects");
            DropIndex("dbo.Sketches", new[] { "User_Id" });
            DropIndex("dbo.Sketches", new[] { "Dimension_Id" });
            DropIndex("dbo.Dimensions", new[] { "Project_Id" });
            DropIndex("dbo.Dimensions", new[] { "Name" });
            DropTable("dbo.Users");
            DropTable("dbo.Sketches");
            DropTable("dbo.Projects");
            DropTable("dbo.Dimensions");
        }
    }
}

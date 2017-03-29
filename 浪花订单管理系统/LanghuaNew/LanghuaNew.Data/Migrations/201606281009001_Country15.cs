namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Country15 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RoleRights",
                c => new
                    {
                        RoleRightID = c.Int(nullable: false, identity: true),
                        ControllerName = c.String(),
                        ActionName = c.String(),
                    })
                .PrimaryKey(t => t.RoleRightID);
            
            CreateTable(
                "dbo.SendMessages",
                c => new
                    {
                        SendMessageID = c.Int(nullable: false, identity: true),
                        CustomerID = c.Int(nullable: false),
                        IsSend = c.Boolean(nullable: false),
                        UserID = c.Int(nullable: false),
                        UserName = c.String(),
                    })
                .PrimaryKey(t => t.SendMessageID)
                .ForeignKey("dbo.Customers", t => t.CustomerID, cascadeDelete: true)
                .Index(t => t.CustomerID);
            
            CreateTable(
                "dbo.TBOrderStates",
                c => new
                    {
                        TBOrderStateID = c.Int(nullable: false, identity: true),
                        CustomerID = c.Int(nullable: false),
                        TBNum = c.String(),
                        IsSend = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.TBOrderStateID)
                .ForeignKey("dbo.Customers", t => t.CustomerID, cascadeDelete: true)
                .Index(t => t.CustomerID);
            
            AddColumn("dbo.Roles", "RoleRight_RoleRightID", c => c.Int());
            CreateIndex("dbo.Roles", "RoleRight_RoleRightID");
            AddForeignKey("dbo.Roles", "RoleRight_RoleRightID", "dbo.RoleRights", "RoleRightID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TBOrderStates", "CustomerID", "dbo.Customers");
            DropForeignKey("dbo.SendMessages", "CustomerID", "dbo.Customers");
            DropForeignKey("dbo.Roles", "RoleRight_RoleRightID", "dbo.RoleRights");
            DropIndex("dbo.TBOrderStates", new[] { "CustomerID" });
            DropIndex("dbo.SendMessages", new[] { "CustomerID" });
            DropIndex("dbo.Roles", new[] { "RoleRight_RoleRightID" });
            DropColumn("dbo.Roles", "RoleRight_RoleRightID");
            DropTable("dbo.TBOrderStates");
            DropTable("dbo.SendMessages");
            DropTable("dbo.RoleRights");
        }
    }
}

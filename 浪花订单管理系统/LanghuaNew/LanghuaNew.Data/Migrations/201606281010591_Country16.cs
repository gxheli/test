namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Country16 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Roles", "RoleRight_RoleRightID", "dbo.RoleRights");
            DropIndex("dbo.Roles", new[] { "RoleRight_RoleRightID" });
            CreateTable(
                "dbo.RoleRoleRights",
                c => new
                    {
                        Role_RoleID = c.Int(nullable: false),
                        RoleRight_RoleRightID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Role_RoleID, t.RoleRight_RoleRightID })
                .ForeignKey("dbo.Roles", t => t.Role_RoleID, cascadeDelete: true)
                .ForeignKey("dbo.RoleRights", t => t.RoleRight_RoleRightID, cascadeDelete: true)
                .Index(t => t.Role_RoleID)
                .Index(t => t.RoleRight_RoleRightID);
            
            DropColumn("dbo.Roles", "RoleRight_RoleRightID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Roles", "RoleRight_RoleRightID", c => c.Int());
            DropForeignKey("dbo.RoleRoleRights", "RoleRight_RoleRightID", "dbo.RoleRights");
            DropForeignKey("dbo.RoleRoleRights", "Role_RoleID", "dbo.Roles");
            DropIndex("dbo.RoleRoleRights", new[] { "RoleRight_RoleRightID" });
            DropIndex("dbo.RoleRoleRights", new[] { "Role_RoleID" });
            DropTable("dbo.RoleRoleRights");
            CreateIndex("dbo.Roles", "RoleRight_RoleRightID");
            AddForeignKey("dbo.Roles", "RoleRight_RoleRightID", "dbo.RoleRights", "RoleRightID");
        }
    }
}

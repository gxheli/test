namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Customer20 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RoleRoleRights", "Role_RoleID", "dbo.Roles");
            DropForeignKey("dbo.RoleRoleRights", "RoleRight_RoleRightID", "dbo.RoleRights");
            DropIndex("dbo.RoleRoleRights", new[] { "Role_RoleID" });
            DropIndex("dbo.RoleRoleRights", new[] { "RoleRight_RoleRightID" });
            CreateTable(
                "dbo.MenuRights",
                c => new
                    {
                        MenuRightID = c.Int(nullable: false, identity: true),
                        MenuRightName = c.String(),
                        Remark = c.String(),
                        isDefault = c.Boolean(nullable: false),
                        modular = c.Int(),
                    })
                .PrimaryKey(t => t.MenuRightID);
            
            CreateTable(
                "dbo.SalesStatistics",
                c => new
                    {
                        SalesStatisticID = c.Int(nullable: false, identity: true),
                        SupplierID = c.Int(nullable: false),
                        ServiceItemID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SalesStatisticID)
                .ForeignKey("dbo.ServiceItems", t => t.ServiceItemID, cascadeDelete: true)
                .ForeignKey("dbo.Suppliers", t => t.SupplierID, cascadeDelete: true)
                .Index(t => t.SupplierID)
                .Index(t => t.ServiceItemID);
            
            CreateTable(
                "dbo.RoleMenuRights",
                c => new
                    {
                        Role_RoleID = c.Int(nullable: false),
                        MenuRight_MenuRightID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Role_RoleID, t.MenuRight_MenuRightID })
                .ForeignKey("dbo.Roles", t => t.Role_RoleID, cascadeDelete: true)
                .ForeignKey("dbo.MenuRights", t => t.MenuRight_MenuRightID, cascadeDelete: true)
                .Index(t => t.Role_RoleID)
                .Index(t => t.MenuRight_MenuRightID);
            
            AddColumn("dbo.RoleRights", "Remark", c => c.String());
            AddColumn("dbo.RoleRights", "MenuRightID", c => c.Int());
            AlterColumn("dbo.Roles", "RoleEnName", c => c.String());
            CreateIndex("dbo.RoleRights", "MenuRightID");
            AddForeignKey("dbo.RoleRights", "MenuRightID", "dbo.MenuRights", "MenuRightID");
            DropTable("dbo.RoleRoleRights");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.RoleRoleRights",
                c => new
                    {
                        Role_RoleID = c.Int(nullable: false),
                        RoleRight_RoleRightID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Role_RoleID, t.RoleRight_RoleRightID });
            
            DropForeignKey("dbo.SalesStatistics", "SupplierID", "dbo.Suppliers");
            DropForeignKey("dbo.SalesStatistics", "ServiceItemID", "dbo.ServiceItems");
            DropForeignKey("dbo.RoleMenuRights", "MenuRight_MenuRightID", "dbo.MenuRights");
            DropForeignKey("dbo.RoleMenuRights", "Role_RoleID", "dbo.Roles");
            DropForeignKey("dbo.RoleRights", "MenuRightID", "dbo.MenuRights");
            DropIndex("dbo.RoleMenuRights", new[] { "MenuRight_MenuRightID" });
            DropIndex("dbo.RoleMenuRights", new[] { "Role_RoleID" });
            DropIndex("dbo.SalesStatistics", new[] { "ServiceItemID" });
            DropIndex("dbo.SalesStatistics", new[] { "SupplierID" });
            DropIndex("dbo.RoleRights", new[] { "MenuRightID" });
            AlterColumn("dbo.Roles", "RoleEnName", c => c.String(nullable: false));
            DropColumn("dbo.RoleRights", "MenuRightID");
            DropColumn("dbo.RoleRights", "Remark");
            DropTable("dbo.RoleMenuRights");
            DropTable("dbo.SalesStatistics");
            DropTable("dbo.MenuRights");
            CreateIndex("dbo.RoleRoleRights", "RoleRight_RoleRightID");
            CreateIndex("dbo.RoleRoleRights", "Role_RoleID");
            AddForeignKey("dbo.RoleRoleRights", "RoleRight_RoleRightID", "dbo.RoleRights", "RoleRightID", cascadeDelete: true);
            AddForeignKey("dbo.RoleRoleRights", "Role_RoleID", "dbo.Roles", "RoleID", cascadeDelete: true);
        }
    }
}

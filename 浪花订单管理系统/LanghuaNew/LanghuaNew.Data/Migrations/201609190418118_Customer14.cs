namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Customer14 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SupplierRoles",
                c => new
                    {
                        SupplierRoleID = c.Int(nullable: false, identity: true),
                        SupplierID = c.Int(),
                        SupplierRoleName = c.String(nullable: false),
                        SupplierRoleEnName = c.String(nullable: false),
                        Remark = c.String(),
                        CreateTime = c.DateTimeOffset(nullable: false, precision: 7),
                        LastEditTime = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.SupplierRoleID)
                .ForeignKey("dbo.Suppliers", t => t.SupplierID)
                .Index(t => t.SupplierID);
            
            CreateTable(
                "dbo.SupplierRoleRights",
                c => new
                    {
                        SupplierRoleRightID = c.Int(nullable: false, identity: true),
                        ControllerName = c.String(),
                        ActionName = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.SupplierRoleRightID);
            
            CreateTable(
                "dbo.SupplierUsers",
                c => new
                    {
                        SupplierUserID = c.Int(nullable: false, identity: true),
                        SupplierID = c.Int(nullable: false),
                        SupplierUserName = c.String(nullable: false),
                        SupplierNickName = c.String(nullable: false),
                        PassWord = c.String(nullable: false),
                        CreateTime = c.DateTimeOffset(nullable: false, precision: 7),
                        LastLoginTime = c.DateTimeOffset(nullable: false, precision: 7),
                        IP = c.String(),
                        OpenID = c.String(),
                        SupplierUserEnableState = c.Int(nullable: false),
                        IsMaster = c.Boolean(nullable: false),
                        RealTimeMessage = c.Boolean(nullable: false),
                        SummaryMessage = c.Boolean(nullable: false),
                        Disturb = c.Boolean(nullable: false),
                        BeginTime = c.String(),
                        EndTime = c.String(),
                    })
                .PrimaryKey(t => t.SupplierUserID)
                .ForeignKey("dbo.Suppliers", t => t.SupplierID, cascadeDelete: true)
                .Index(t => t.SupplierID);
            
            CreateTable(
                "dbo.SupplierUserLogs",
                c => new
                    {
                        SupplierUserLogID = c.Int(nullable: false, identity: true),
                        OperSupplierUserID = c.Int(nullable: false),
                        OperSupplierNickName = c.String(),
                        OperTime = c.DateTimeOffset(nullable: false, precision: 7),
                        Remark = c.String(),
                        Operate = c.Int(nullable: false),
                        SupplierUserID = c.Int(),
                        SupplierUserName = c.String(),
                        SupplierNickName = c.String(),
                    })
                .PrimaryKey(t => t.SupplierUserLogID);
            
            CreateTable(
                "dbo.SupplierRoleRightSupplierRoles",
                c => new
                    {
                        SupplierRoleRight_SupplierRoleRightID = c.Int(nullable: false),
                        SupplierRole_SupplierRoleID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.SupplierRoleRight_SupplierRoleRightID, t.SupplierRole_SupplierRoleID })
                .ForeignKey("dbo.SupplierRoleRights", t => t.SupplierRoleRight_SupplierRoleRightID, cascadeDelete: true)
                .ForeignKey("dbo.SupplierRoles", t => t.SupplierRole_SupplierRoleID, cascadeDelete: true)
                .Index(t => t.SupplierRoleRight_SupplierRoleRightID)
                .Index(t => t.SupplierRole_SupplierRoleID);
            
            CreateTable(
                "dbo.SupplierUserSupplierRoles",
                c => new
                    {
                        SupplierUser_SupplierUserID = c.Int(nullable: false),
                        SupplierRole_SupplierRoleID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.SupplierUser_SupplierUserID, t.SupplierRole_SupplierRoleID })
                .ForeignKey("dbo.SupplierUsers", t => t.SupplierUser_SupplierUserID, cascadeDelete: true)
                .ForeignKey("dbo.SupplierRoles", t => t.SupplierRole_SupplierRoleID, cascadeDelete: true)
                .Index(t => t.SupplierUser_SupplierUserID)
                .Index(t => t.SupplierRole_SupplierRoleID);
            
            DropColumn("dbo.Suppliers", "SupplierSysName");
            DropColumn("dbo.Suppliers", "SupplierPWD");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Suppliers", "SupplierPWD", c => c.String());
            AddColumn("dbo.Suppliers", "SupplierSysName", c => c.String());
            DropForeignKey("dbo.SupplierUserSupplierRoles", "SupplierRole_SupplierRoleID", "dbo.SupplierRoles");
            DropForeignKey("dbo.SupplierUserSupplierRoles", "SupplierUser_SupplierUserID", "dbo.SupplierUsers");
            DropForeignKey("dbo.SupplierUsers", "SupplierID", "dbo.Suppliers");
            DropForeignKey("dbo.SupplierRoleRightSupplierRoles", "SupplierRole_SupplierRoleID", "dbo.SupplierRoles");
            DropForeignKey("dbo.SupplierRoleRightSupplierRoles", "SupplierRoleRight_SupplierRoleRightID", "dbo.SupplierRoleRights");
            DropForeignKey("dbo.SupplierRoles", "SupplierID", "dbo.Suppliers");
            DropIndex("dbo.SupplierUserSupplierRoles", new[] { "SupplierRole_SupplierRoleID" });
            DropIndex("dbo.SupplierUserSupplierRoles", new[] { "SupplierUser_SupplierUserID" });
            DropIndex("dbo.SupplierRoleRightSupplierRoles", new[] { "SupplierRole_SupplierRoleID" });
            DropIndex("dbo.SupplierRoleRightSupplierRoles", new[] { "SupplierRoleRight_SupplierRoleRightID" });
            DropIndex("dbo.SupplierUsers", new[] { "SupplierID" });
            DropIndex("dbo.SupplierRoles", new[] { "SupplierID" });
            DropTable("dbo.SupplierUserSupplierRoles");
            DropTable("dbo.SupplierRoleRightSupplierRoles");
            DropTable("dbo.SupplierUserLogs");
            DropTable("dbo.SupplierUsers");
            DropTable("dbo.SupplierRoleRights");
            DropTable("dbo.SupplierRoles");
        }
    }
}

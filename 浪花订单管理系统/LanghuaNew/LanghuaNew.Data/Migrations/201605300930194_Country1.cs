namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Country1 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.RoleUsers", newName: "UserRoles");
            DropForeignKey("dbo.Orders", "UserID", "dbo.Users");
            DropForeignKey("dbo.CustomerServices", "OrderID", "dbo.Orders");
            DropForeignKey("dbo.OrderHistories", "OperUser_UserID", "dbo.Users");
            DropIndex("dbo.Orders", new[] { "UserID" });
            DropIndex("dbo.CustomerServices", new[] { "OrderID" });
            DropIndex("dbo.OrderHistories", new[] { "OperUser_UserID" });
            DropPrimaryKey("dbo.UserRoles");
            AddColumn("dbo.Orders", "CreateUserID", c => c.String());
            AddColumn("dbo.Orders", "CreateUserNikeName", c => c.String());
            AddColumn("dbo.Orders", "IsNeedCustomerService", c => c.Boolean(nullable: false));
            AddColumn("dbo.Orders", "Remark", c => c.String());
            AddColumn("dbo.Users", "IsDelete", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrderHistories", "OperUserID", c => c.String());
            AddColumn("dbo.OrderHistories", "OperUserNickName", c => c.String());
            AddColumn("dbo.ServiceItemHistories", "SupplierCode", c => c.String());
            AlterColumn("dbo.ServiceItemHistories", "SupplierID", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.UserRoles", new[] { "User_UserID", "Role_RoleID" });
            DropColumn("dbo.OrderHistories", "OperUser_UserID");
            DropTable("dbo.CustomerServices");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.CustomerServices",
                c => new
                    {
                        CustomerServiceID = c.Int(nullable: false, identity: true),
                        OrderID = c.Int(nullable: false),
                        IsHandle = c.Boolean(nullable: false),
                        Remark = c.String(),
                        HandleRemark = c.String(),
                        CreateUserID = c.Int(nullable: false),
                        CreateUserName = c.String(),
                        HandleUserID = c.Int(nullable: false),
                        HandleUserName = c.String(),
                    })
                .PrimaryKey(t => t.CustomerServiceID);
            
            AddColumn("dbo.OrderHistories", "OperUser_UserID", c => c.Int());
            DropPrimaryKey("dbo.UserRoles");
            AlterColumn("dbo.ServiceItemHistories", "SupplierID", c => c.String());
            DropColumn("dbo.ServiceItemHistories", "SupplierCode");
            DropColumn("dbo.OrderHistories", "OperUserNickName");
            DropColumn("dbo.OrderHistories", "OperUserID");
            DropColumn("dbo.Users", "IsDelete");
            DropColumn("dbo.Orders", "Remark");
            DropColumn("dbo.Orders", "IsNeedCustomerService");
            DropColumn("dbo.Orders", "CreateUserNikeName");
            DropColumn("dbo.Orders", "CreateUserID");
            AddPrimaryKey("dbo.UserRoles", new[] { "Role_RoleID", "User_UserID" });
            CreateIndex("dbo.OrderHistories", "OperUser_UserID");
            CreateIndex("dbo.CustomerServices", "OrderID");
            CreateIndex("dbo.Orders", "UserID");
            AddForeignKey("dbo.OrderHistories", "OperUser_UserID", "dbo.Users", "UserID");
            AddForeignKey("dbo.CustomerServices", "OrderID", "dbo.Orders", "OrderID", cascadeDelete: true);
            AddForeignKey("dbo.Orders", "UserID", "dbo.Users", "UserID", cascadeDelete: true);
            RenameTable(name: "dbo.UserRoles", newName: "RoleUsers");
        }
    }
}

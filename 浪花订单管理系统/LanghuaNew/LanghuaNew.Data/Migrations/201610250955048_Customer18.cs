namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Customer18 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.OrderHistories", "Order_OrderID", "dbo.Orders");
            DropIndex("dbo.OrderHistories", new[] { "Order_OrderID" });
            RenameColumn(table: "dbo.OrderHistories", name: "Order_OrderID", newName: "OrderID");
            CreateTable(
                "dbo.CancelRegisters",
                c => new
                    {
                        CancelRegisterID = c.Int(nullable: false, identity: true),
                        ServiceItemID = c.Int(nullable: false),
                        SupplierID = c.Int(nullable: false),
                        StartDate = c.DateTimeOffset(nullable: false, precision: 7),
                        EndDate = c.DateTimeOffset(nullable: false, precision: 7),
                        Remark = c.String(),
                        CreateUserID = c.Int(nullable: false),
                        CreateUserNikeName = c.String(),
                        CreateTime = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.CancelRegisterID)
                .ForeignKey("dbo.ServiceItems", t => t.ServiceItemID, cascadeDelete: true)
                .ForeignKey("dbo.Suppliers", t => t.SupplierID, cascadeDelete: true)
                .Index(t => t.ServiceItemID)
                .Index(t => t.SupplierID);
            
            CreateTable(
                "dbo.DistributionTallies",
                c => new
                    {
                        DistributionTallyID = c.Int(nullable: false, identity: true),
                        ServiceItemID = c.Int(nullable: false),
                        SupplierID = c.Int(nullable: false),
                        GroupNo = c.String(),
                        TravelDate = c.DateTimeOffset(nullable: false, precision: 7),
                        ReturnDate = c.DateTimeOffset(nullable: false, precision: 7),
                        AdultNum = c.Int(nullable: false),
                        ChildNum = c.Int(nullable: false),
                        INFNum = c.Int(nullable: false),
                        RoomNum = c.Int(nullable: false),
                        RightNum = c.Int(nullable: false),
                        Remark = c.String(),
                        IsCancel = c.Boolean(nullable: false),
                        CreateUserID = c.Int(nullable: false),
                        CreateUserNikeName = c.String(),
                        CreateTime = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.DistributionTallyID)
                .ForeignKey("dbo.ServiceItems", t => t.ServiceItemID, cascadeDelete: true)
                .ForeignKey("dbo.Suppliers", t => t.SupplierID, cascadeDelete: true)
                .Index(t => t.ServiceItemID)
                .Index(t => t.SupplierID);
            
            AddColumn("dbo.SellControls", "IsDistribution", c => c.Boolean(nullable: false));
            AddColumn("dbo.BillReports", "RealReceive", c => c.Single(nullable: false));
            AddColumn("dbo.BillReports", "Currency", c => c.String());
            AlterColumn("dbo.OrderHistories", "OrderID", c => c.Int(nullable: false));
            AlterColumn("dbo.BillReports", "TotalReceive", c => c.Single(nullable: false));
            CreateIndex("dbo.OrderHistories", "OrderID");
            AddForeignKey("dbo.OrderHistories", "OrderID", "dbo.Orders", "OrderID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderHistories", "OrderID", "dbo.Orders");
            DropForeignKey("dbo.DistributionTallies", "SupplierID", "dbo.Suppliers");
            DropForeignKey("dbo.DistributionTallies", "ServiceItemID", "dbo.ServiceItems");
            DropForeignKey("dbo.CancelRegisters", "SupplierID", "dbo.Suppliers");
            DropForeignKey("dbo.CancelRegisters", "ServiceItemID", "dbo.ServiceItems");
            DropIndex("dbo.DistributionTallies", new[] { "SupplierID" });
            DropIndex("dbo.DistributionTallies", new[] { "ServiceItemID" });
            DropIndex("dbo.CancelRegisters", new[] { "SupplierID" });
            DropIndex("dbo.CancelRegisters", new[] { "ServiceItemID" });
            DropIndex("dbo.OrderHistories", new[] { "OrderID" });
            AlterColumn("dbo.BillReports", "TotalReceive", c => c.String());
            AlterColumn("dbo.OrderHistories", "OrderID", c => c.Int());
            DropColumn("dbo.BillReports", "Currency");
            DropColumn("dbo.BillReports", "RealReceive");
            DropColumn("dbo.SellControls", "IsDistribution");
            DropTable("dbo.DistributionTallies");
            DropTable("dbo.CancelRegisters");
            RenameColumn(table: "dbo.OrderHistories", name: "OrderID", newName: "Order_OrderID");
            CreateIndex("dbo.OrderHistories", "Order_OrderID");
            AddForeignKey("dbo.OrderHistories", "Order_OrderID", "dbo.Orders", "OrderID");
        }
    }
}

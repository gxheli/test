namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Country23 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.TravellerServiceItemHistories", newName: "ServiceItemHistoryTravellers");
            DropPrimaryKey("dbo.ServiceItemHistoryTravellers");
            CreateTable(
                "dbo.AlipayTransferLogs",
                c => new
                    {
                        AlipayTransferLogID = c.Int(nullable: false, identity: true),
                        AlipayTransferID = c.Int(nullable: false),
                        UserID = c.Int(nullable: false),
                        UserName = c.Int(nullable: false),
                        Operate = c.Int(nullable: false),
                        OperateTime = c.DateTimeOffset(nullable: false, precision: 7),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.AlipayTransferLogID)
                .ForeignKey("dbo.AlipayTransfers", t => t.AlipayTransferID, cascadeDelete: true)
                .Index(t => t.AlipayTransferID);
            
            CreateTable(
                "dbo.AlipayTransfers",
                c => new
                    {
                        AlipayTransferID = c.Int(nullable: false, identity: true),
                        OrderSourseID = c.Int(nullable: false),
                        TBID = c.String(),
                        TBNum = c.String(),
                        ReceiveAddress = c.String(),
                        TransferTypeValue = c.Int(nullable: false),
                        TransferNum = c.Double(nullable: false),
                        Remark = c.String(),
                        TransferStateValue = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AlipayTransferID)
                .ForeignKey("dbo.OrderSourses", t => t.OrderSourseID, cascadeDelete: true)
                .Index(t => t.OrderSourseID);
            
            AddPrimaryKey("dbo.ServiceItemHistoryTravellers", new[] { "ServiceItemHistory_OrderID", "Traveller_TravellerID" });
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AlipayTransfers", "OrderSourseID", "dbo.OrderSourses");
            DropForeignKey("dbo.AlipayTransferLogs", "AlipayTransferID", "dbo.AlipayTransfers");
            DropIndex("dbo.AlipayTransfers", new[] { "OrderSourseID" });
            DropIndex("dbo.AlipayTransferLogs", new[] { "AlipayTransferID" });
            DropPrimaryKey("dbo.ServiceItemHistoryTravellers");
            DropTable("dbo.AlipayTransfers");
            DropTable("dbo.AlipayTransferLogs");
            AddPrimaryKey("dbo.ServiceItemHistoryTravellers", new[] { "Traveller_TravellerID", "ServiceItemHistory_OrderID" });
            RenameTable(name: "dbo.ServiceItemHistoryTravellers", newName: "TravellerServiceItemHistories");
        }
    }
}

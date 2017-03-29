namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Customer7 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ServiceItemHistoryTravellers", "ServiceItemHistory_OrderID", "dbo.ServiceItemHistories");
            DropForeignKey("dbo.ServiceItemHistoryTravellers", "Traveller_TravellerID", "dbo.Travellers");
            DropIndex("dbo.ServiceItemHistoryTravellers", new[] { "ServiceItemHistory_OrderID" });
            DropIndex("dbo.ServiceItemHistoryTravellers", new[] { "Traveller_TravellerID" });
            CreateTable(
                "dbo.OrderTravellers",
                c => new
                    {
                        OrderTravellerID = c.Int(nullable: false, identity: true),
                        TravellerName = c.String(),
                        TravellerEnname = c.String(),
                        PassportNo = c.String(),
                        Birthday = c.DateTimeOffset(nullable: false, precision: 7),
                        TravellerSex = c.Int(nullable: false),
                        CreateTime = c.DateTimeOffset(nullable: false, precision: 7),
                        TravellerDetail_Height = c.String(),
                        TravellerDetail_Weight = c.String(),
                        TravellerDetail_ShoesSize = c.String(),
                        TravellerDetail_ClothesSize = c.String(),
                        TravellerDetail_GlassesNum = c.String(),
                        OrderID = c.Int(nullable: false),
                        CustomerID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.OrderTravellerID)
                .ForeignKey("dbo.Customers", t => t.CustomerID, cascadeDelete: true)
                .ForeignKey("dbo.ServiceItemHistories", t => t.OrderID, cascadeDelete: true)
                .Index(t => t.OrderID)
                .Index(t => t.CustomerID);
            
            AddColumn("dbo.ServiceItems", "IsAutomaticDeliver", c => c.Boolean(nullable: false));
            DropTable("dbo.ServiceItemHistoryTravellers");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ServiceItemHistoryTravellers",
                c => new
                    {
                        ServiceItemHistory_OrderID = c.Int(nullable: false),
                        Traveller_TravellerID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ServiceItemHistory_OrderID, t.Traveller_TravellerID });
            
            DropForeignKey("dbo.OrderTravellers", "OrderID", "dbo.ServiceItemHistories");
            DropForeignKey("dbo.OrderTravellers", "CustomerID", "dbo.Customers");
            DropIndex("dbo.OrderTravellers", new[] { "CustomerID" });
            DropIndex("dbo.OrderTravellers", new[] { "OrderID" });
            DropColumn("dbo.ServiceItems", "IsAutomaticDeliver");
            DropTable("dbo.OrderTravellers");
            CreateIndex("dbo.ServiceItemHistoryTravellers", "Traveller_TravellerID");
            CreateIndex("dbo.ServiceItemHistoryTravellers", "ServiceItemHistory_OrderID");
            AddForeignKey("dbo.ServiceItemHistoryTravellers", "Traveller_TravellerID", "dbo.Travellers", "TravellerID", cascadeDelete: true);
            AddForeignKey("dbo.ServiceItemHistoryTravellers", "ServiceItemHistory_OrderID", "dbo.ServiceItemHistories", "OrderID", cascadeDelete: true);
        }
    }
}

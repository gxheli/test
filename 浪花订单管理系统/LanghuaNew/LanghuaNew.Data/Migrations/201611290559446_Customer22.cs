namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Customer22 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ItemPriceLogs",
                c => new
                    {
                        ItemPriceLogID = c.Int(nullable: false, identity: true),
                        Operate = c.String(),
                        OperateTime = c.DateTimeOffset(nullable: false, precision: 7),
                        UserID = c.Int(nullable: false),
                        UserName = c.String(),
                        Operator = c.Int(nullable: false),
                        Remark = c.String(),
                        SupplierServiceItemID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ItemPriceLogID)
                .ForeignKey("dbo.SupplierServiceItems", t => t.SupplierServiceItemID, cascadeDelete: true)
                .Index(t => t.SupplierServiceItemID);
            
            CreateTable(
                "dbo.ExtraServicePriceChanges",
                c => new
                    {
                        ExtraServicePriceChangeID = c.Int(nullable: false, identity: true),
                        ExtraServicePriceID = c.Int(nullable: false),
                        ServicePrice = c.Single(nullable: false),
                        SupplierServiceItemChangeID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ExtraServicePriceChangeID)
                .ForeignKey("dbo.SupplierServiceItemChanges", t => t.SupplierServiceItemChangeID, cascadeDelete: true)
                .Index(t => t.SupplierServiceItemChangeID);
            
            CreateTable(
                "dbo.SupplierServiceItemChanges",
                c => new
                    {
                        SupplierServiceItemChangeID = c.Int(nullable: false, identity: true),
                        SupplierServiceItemID = c.Int(nullable: false),
                        CurrencyID = c.Int(nullable: false),
                        PayType = c.Int(nullable: false),
                        SelectEffectiveWay = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SupplierServiceItemChangeID);
            
            CreateTable(
                "dbo.ItemPriceBySupplierChanges",
                c => new
                    {
                        ItemPriceBySupplierChangeID = c.Int(nullable: false, identity: true),
                        ItemPriceBySupplierID = c.Int(nullable: false),
                        startTime = c.DateTimeOffset(nullable: false, precision: 7),
                        EndTime = c.DateTimeOffset(nullable: false, precision: 7),
                        AdultNetPrice = c.Single(nullable: false),
                        ChildNetPrice = c.Single(nullable: false),
                        BobyNetPrice = c.Single(nullable: false),
                        Price = c.Single(nullable: false),
                        SupplierServiceItemChangeID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ItemPriceBySupplierChangeID)
                .ForeignKey("dbo.SupplierServiceItemChanges", t => t.SupplierServiceItemChangeID, cascadeDelete: true)
                .Index(t => t.SupplierServiceItemChangeID);
            
            AddColumn("dbo.SupplierServiceItems", "IsChange", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemPriceBySupplierChanges", "SupplierServiceItemChangeID", "dbo.SupplierServiceItemChanges");
            DropForeignKey("dbo.ExtraServicePriceChanges", "SupplierServiceItemChangeID", "dbo.SupplierServiceItemChanges");
            DropForeignKey("dbo.ItemPriceLogs", "SupplierServiceItemID", "dbo.SupplierServiceItems");
            DropIndex("dbo.ItemPriceBySupplierChanges", new[] { "SupplierServiceItemChangeID" });
            DropIndex("dbo.ExtraServicePriceChanges", new[] { "SupplierServiceItemChangeID" });
            DropIndex("dbo.ItemPriceLogs", new[] { "SupplierServiceItemID" });
            DropColumn("dbo.SupplierServiceItems", "IsChange");
            DropTable("dbo.ItemPriceBySupplierChanges");
            DropTable("dbo.SupplierServiceItemChanges");
            DropTable("dbo.ExtraServicePriceChanges");
            DropTable("dbo.ItemPriceLogs");
        }
    }
}

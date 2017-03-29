namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Customer23 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ExtraServicePriceChanges", "ExtraServiceID", c => c.Int(nullable: false));
            AddColumn("dbo.SupplierServiceItemChanges", "ServiceItemID", c => c.Int(nullable: false));
            AddColumn("dbo.SupplierServiceItemChanges", "SupplierID", c => c.Int(nullable: false));
            CreateIndex("dbo.ExtraServicePriceChanges", "ExtraServiceID");
            CreateIndex("dbo.SupplierServiceItemChanges", "ServiceItemID");
            CreateIndex("dbo.SupplierServiceItemChanges", "SupplierID");
            CreateIndex("dbo.SupplierServiceItemChanges", "CurrencyID");
            AddForeignKey("dbo.ExtraServicePriceChanges", "ExtraServiceID", "dbo.ExtraServices", "ExtraServiceID", cascadeDelete: true);
            AddForeignKey("dbo.SupplierServiceItemChanges", "CurrencyID", "dbo.Currencies", "CurrencyID", cascadeDelete: true);
            AddForeignKey("dbo.SupplierServiceItemChanges", "SupplierID", "dbo.Suppliers", "SupplierID", cascadeDelete: true);
            AddForeignKey("dbo.SupplierServiceItemChanges", "ServiceItemID", "dbo.ServiceItems", "ServiceItemID", cascadeDelete: true);
            DropColumn("dbo.ExtraServicePrices", "MinNum");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ExtraServicePrices", "MinNum", c => c.Int(nullable: false));
            DropForeignKey("dbo.SupplierServiceItemChanges", "ServiceItemID", "dbo.ServiceItems");
            DropForeignKey("dbo.SupplierServiceItemChanges", "SupplierID", "dbo.Suppliers");
            DropForeignKey("dbo.SupplierServiceItemChanges", "CurrencyID", "dbo.Currencies");
            DropForeignKey("dbo.ExtraServicePriceChanges", "ExtraServiceID", "dbo.ExtraServices");
            DropIndex("dbo.SupplierServiceItemChanges", new[] { "CurrencyID" });
            DropIndex("dbo.SupplierServiceItemChanges", new[] { "SupplierID" });
            DropIndex("dbo.SupplierServiceItemChanges", new[] { "ServiceItemID" });
            DropIndex("dbo.ExtraServicePriceChanges", new[] { "ExtraServiceID" });
            DropColumn("dbo.SupplierServiceItemChanges", "SupplierID");
            DropColumn("dbo.SupplierServiceItemChanges", "ServiceItemID");
            DropColumn("dbo.ExtraServicePriceChanges", "ExtraServiceID");
        }
    }
}

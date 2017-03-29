namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Country13 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ExtraServicePrices", "SupplierServiceItem_SupplierServiceItemID", "dbo.SupplierServiceItems");
            DropForeignKey("dbo.ItemPriceBySuppliers", "SupplierServiceItem_SupplierServiceItemID", "dbo.SupplierServiceItems");
            DropIndex("dbo.ExtraServicePrices", new[] { "SupplierServiceItem_SupplierServiceItemID" });
            DropIndex("dbo.ItemPriceBySuppliers", new[] { "SupplierServiceItem_SupplierServiceItemID" });
            RenameColumn(table: "dbo.ExtraServicePrices", name: "SupplierServiceItem_SupplierServiceItemID", newName: "SupplierServiceItemID");
            RenameColumn(table: "dbo.ItemPriceBySuppliers", name: "SupplierServiceItem_SupplierServiceItemID", newName: "SupplierServiceItemID");
            AddColumn("dbo.Orders", "CustomerState", c => c.Int(nullable: false));
            AlterColumn("dbo.ExtraServicePrices", "SupplierServiceItemID", c => c.Int(nullable: false));
            AlterColumn("dbo.ItemPriceBySuppliers", "SupplierServiceItemID", c => c.Int(nullable: false));
            CreateIndex("dbo.ExtraServicePrices", "SupplierServiceItemID");
            CreateIndex("dbo.ItemPriceBySuppliers", "SupplierServiceItemID");
            AddForeignKey("dbo.ExtraServicePrices", "SupplierServiceItemID", "dbo.SupplierServiceItems", "SupplierServiceItemID", cascadeDelete: true);
            AddForeignKey("dbo.ItemPriceBySuppliers", "SupplierServiceItemID", "dbo.SupplierServiceItems", "SupplierServiceItemID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ItemPriceBySuppliers", "SupplierServiceItemID", "dbo.SupplierServiceItems");
            DropForeignKey("dbo.ExtraServicePrices", "SupplierServiceItemID", "dbo.SupplierServiceItems");
            DropIndex("dbo.ItemPriceBySuppliers", new[] { "SupplierServiceItemID" });
            DropIndex("dbo.ExtraServicePrices", new[] { "SupplierServiceItemID" });
            AlterColumn("dbo.ItemPriceBySuppliers", "SupplierServiceItemID", c => c.Int());
            AlterColumn("dbo.ExtraServicePrices", "SupplierServiceItemID", c => c.Int());
            DropColumn("dbo.Orders", "CustomerState");
            RenameColumn(table: "dbo.ItemPriceBySuppliers", name: "SupplierServiceItemID", newName: "SupplierServiceItem_SupplierServiceItemID");
            RenameColumn(table: "dbo.ExtraServicePrices", name: "SupplierServiceItemID", newName: "SupplierServiceItem_SupplierServiceItemID");
            CreateIndex("dbo.ItemPriceBySuppliers", "SupplierServiceItem_SupplierServiceItemID");
            CreateIndex("dbo.ExtraServicePrices", "SupplierServiceItem_SupplierServiceItemID");
            AddForeignKey("dbo.ItemPriceBySuppliers", "SupplierServiceItem_SupplierServiceItemID", "dbo.SupplierServiceItems", "SupplierServiceItemID");
            AddForeignKey("dbo.ExtraServicePrices", "SupplierServiceItem_SupplierServiceItemID", "dbo.SupplierServiceItems", "SupplierServiceItemID");
        }
    }
}

namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Customer26 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SupplierServiceItems", "SellPrice", c => c.Single(nullable: false));
            AddColumn("dbo.SupplierServiceItems", "ChildSellPrice", c => c.Single(nullable: false));
            AddColumn("dbo.ExtraServicePrices", "ServiceSellPrice", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ExtraServicePrices", "ServiceSellPrice");
            DropColumn("dbo.SupplierServiceItems", "ChildSellPrice");
            DropColumn("dbo.SupplierServiceItems", "SellPrice");
        }
    }
}

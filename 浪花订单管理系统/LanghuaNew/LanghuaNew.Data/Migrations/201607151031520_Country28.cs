namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Country28 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SellControls", "FirstServiceItem_ServiceItemID", "dbo.ServiceItems");
            DropForeignKey("dbo.SellControls", "SecondServiceItem_ServiceItemID", "dbo.ServiceItems");
            DropIndex("dbo.SellControls", new[] { "FirstServiceItem_ServiceItemID" });
            DropIndex("dbo.SellControls", new[] { "SecondServiceItem_ServiceItemID" });
            AddColumn("dbo.SellControls", "FirstServiceItem_ServiceItemID1", c => c.Int());
            AddColumn("dbo.SellControls", "SecondServiceItem_ServiceItemID1", c => c.Int());
            AlterColumn("dbo.SellControls", "FirstServiceItem_ServiceItemID", c => c.Int(nullable: false));
            AlterColumn("dbo.SellControls", "SecondServiceItem_ServiceItemID", c => c.Int(nullable: false));
            CreateIndex("dbo.SellControls", "FirstServiceItem_ServiceItemID1");
            CreateIndex("dbo.SellControls", "SecondServiceItem_ServiceItemID1");
            AddForeignKey("dbo.SellControls", "FirstServiceItem_ServiceItemID1", "dbo.ServiceItems", "ServiceItemID");
            AddForeignKey("dbo.SellControls", "SecondServiceItem_ServiceItemID1", "dbo.ServiceItems", "ServiceItemID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SellControls", "SecondServiceItem_ServiceItemID1", "dbo.ServiceItems");
            DropForeignKey("dbo.SellControls", "FirstServiceItem_ServiceItemID1", "dbo.ServiceItems");
            DropIndex("dbo.SellControls", new[] { "SecondServiceItem_ServiceItemID1" });
            DropIndex("dbo.SellControls", new[] { "FirstServiceItem_ServiceItemID1" });
            AlterColumn("dbo.SellControls", "SecondServiceItem_ServiceItemID", c => c.Int());
            AlterColumn("dbo.SellControls", "FirstServiceItem_ServiceItemID", c => c.Int());
            DropColumn("dbo.SellControls", "SecondServiceItem_ServiceItemID1");
            DropColumn("dbo.SellControls", "FirstServiceItem_ServiceItemID1");
            CreateIndex("dbo.SellControls", "SecondServiceItem_ServiceItemID");
            CreateIndex("dbo.SellControls", "FirstServiceItem_ServiceItemID");
            AddForeignKey("dbo.SellControls", "SecondServiceItem_ServiceItemID", "dbo.ServiceItems", "ServiceItemID");
            AddForeignKey("dbo.SellControls", "FirstServiceItem_ServiceItemID", "dbo.ServiceItems", "ServiceItemID");
        }
    }
}

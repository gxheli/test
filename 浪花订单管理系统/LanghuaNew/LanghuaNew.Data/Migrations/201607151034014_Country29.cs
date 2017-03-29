namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Country29 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.SellControls", new[] { "FirstServiceItem_ServiceItemID1" });
            DropIndex("dbo.SellControls", new[] { "SecondServiceItem_ServiceItemID1" });
            DropColumn("dbo.SellControls", "FirstServiceItem_ServiceItemID");
            DropColumn("dbo.SellControls", "SecondServiceItem_ServiceItemID");
            RenameColumn(table: "dbo.SellControls", name: "FirstServiceItem_ServiceItemID1", newName: "FirstServiceItem_ServiceItemID");
            RenameColumn(table: "dbo.SellControls", name: "SecondServiceItem_ServiceItemID1", newName: "SecondServiceItem_ServiceItemID");
            AlterColumn("dbo.SellControls", "FirstServiceItem_ServiceItemID", c => c.Int());
            AlterColumn("dbo.SellControls", "SecondServiceItem_ServiceItemID", c => c.Int());
            CreateIndex("dbo.SellControls", "FirstServiceItem_ServiceItemID");
            CreateIndex("dbo.SellControls", "SecondServiceItem_ServiceItemID");
        }
        
        public override void Down()
        {
            DropIndex("dbo.SellControls", new[] { "SecondServiceItem_ServiceItemID" });
            DropIndex("dbo.SellControls", new[] { "FirstServiceItem_ServiceItemID" });
            AlterColumn("dbo.SellControls", "SecondServiceItem_ServiceItemID", c => c.Int(nullable: false));
            AlterColumn("dbo.SellControls", "FirstServiceItem_ServiceItemID", c => c.Int(nullable: false));
            RenameColumn(table: "dbo.SellControls", name: "SecondServiceItem_ServiceItemID", newName: "SecondServiceItem_ServiceItemID1");
            RenameColumn(table: "dbo.SellControls", name: "FirstServiceItem_ServiceItemID", newName: "FirstServiceItem_ServiceItemID1");
            AddColumn("dbo.SellControls", "SecondServiceItem_ServiceItemID", c => c.Int(nullable: false));
            AddColumn("dbo.SellControls", "FirstServiceItem_ServiceItemID", c => c.Int(nullable: false));
            CreateIndex("dbo.SellControls", "SecondServiceItem_ServiceItemID1");
            CreateIndex("dbo.SellControls", "FirstServiceItem_ServiceItemID1");
        }
    }
}

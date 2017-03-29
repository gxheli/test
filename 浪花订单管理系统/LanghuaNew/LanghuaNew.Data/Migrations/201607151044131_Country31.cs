namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Country31 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SellControls", "FirstServiceItemID", c => c.Int(nullable: false));
            AddColumn("dbo.SellControls", "SecondServiceItemID", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SellControls", "SecondServiceItemID");
            DropColumn("dbo.SellControls", "FirstServiceItemID");
        }
    }
}

namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Country32 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.SellControls", "FirstServiceItemID");
            DropColumn("dbo.SellControls", "SecondServiceItemID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SellControls", "SecondServiceItemID", c => c.Int(nullable: false));
            AddColumn("dbo.SellControls", "FirstServiceItemID", c => c.Int(nullable: false));
        }
    }
}

namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Customer8 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderTravellers", "TravellerID", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderTravellers", "TravellerID");
        }
    }
}

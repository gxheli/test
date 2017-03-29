namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Country4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServiceItemHistories", "TrafficCurrencyID", c => c.Int(nullable: false));
            AddColumn("dbo.ServiceItemHistories", "TrafficCurrencyName", c => c.String());
            AddColumn("dbo.ServiceItemHistories", "TrafficCurrencyChangeType", c => c.Int(nullable: false));
            AddColumn("dbo.ServiceItemHistories", "TrafficExchangeRate", c => c.Single(nullable: false));
            AddColumn("dbo.ServiceItemHistories", "TotalCost", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ServiceItemHistories", "TotalCost");
            DropColumn("dbo.ServiceItemHistories", "TrafficExchangeRate");
            DropColumn("dbo.ServiceItemHistories", "TrafficCurrencyChangeType");
            DropColumn("dbo.ServiceItemHistories", "TrafficCurrencyName");
            DropColumn("dbo.ServiceItemHistories", "TrafficCurrencyID");
        }
    }
}

namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Country5 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServiceItems", "IsFixedDay", c => c.Boolean(nullable: false));
            AddColumn("dbo.ServiceItems", "FixedDays", c => c.Int(nullable: false));
            AddColumn("dbo.ServiceItemHistories", "ReturnDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.ServiceItemHistories", "TravelCompany", c => c.String());
            AddColumn("dbo.ServiceItemHistories", "InsuranceDays", c => c.Int(nullable: false));
            AlterColumn("dbo.ServiceItemHistories", "ReceiveManTime", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ServiceItemHistories", "ReceiveManTime", c => c.DateTime(nullable: false));
            DropColumn("dbo.ServiceItemHistories", "InsuranceDays");
            DropColumn("dbo.ServiceItemHistories", "TravelCompany");
            DropColumn("dbo.ServiceItemHistories", "ReturnDate");
            DropColumn("dbo.ServiceItems", "FixedDays");
            DropColumn("dbo.ServiceItems", "IsFixedDay");
        }
    }
}

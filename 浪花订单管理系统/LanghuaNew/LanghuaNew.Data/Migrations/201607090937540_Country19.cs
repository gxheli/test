namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Country19 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServiceItemHistories", "FixedDays", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ServiceItemHistories", "FixedDays");
        }
    }
}

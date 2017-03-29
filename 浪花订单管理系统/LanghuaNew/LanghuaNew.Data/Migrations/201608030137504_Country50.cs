namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Country50 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderSourses", "OrderSourseEnableState", c => c.Int(nullable: false));
            AddColumn("dbo.Currencies", "CurrencyEnableState", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Currencies", "CurrencyEnableState");
            DropColumn("dbo.OrderSourses", "OrderSourseEnableState");
        }
    }
}

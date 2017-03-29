namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Customer37 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServiceItems", "CityID", c => c.Int());
            CreateIndex("dbo.ServiceItems", "CityID");
            AddForeignKey("dbo.ServiceItems", "CityID", "dbo.Cities", "CityID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ServiceItems", "CityID", "dbo.Cities");
            DropIndex("dbo.ServiceItems", new[] { "CityID" });
            DropColumn("dbo.ServiceItems", "CityID");
        }
    }
}

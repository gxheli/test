namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Customer19 : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.ServiceItemHistories", "ServiceItemID");
        }
        
        public override void Down()
        {
            DropIndex("dbo.ServiceItemHistories", new[] { "ServiceItemID" });
        }
    }
}

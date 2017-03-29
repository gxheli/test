namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Country8 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Orders", "OrderNo", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Orders", "OrderNo", c => c.String());
        }
    }
}

namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Country9 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Customers", "CreateTime", c => c.DateTimeOffset(nullable: false, precision: 7));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Customers", "CreateTime", c => c.DateTime(nullable: false));
        }
    }
}

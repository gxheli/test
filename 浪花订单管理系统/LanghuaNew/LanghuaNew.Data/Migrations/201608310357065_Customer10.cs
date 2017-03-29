namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Customer10 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Customers", "CustomerTBCode", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Customers", "CustomerTBCode", c => c.String());
        }
    }
}

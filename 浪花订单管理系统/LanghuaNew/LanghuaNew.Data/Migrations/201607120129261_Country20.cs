namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Country20 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Customers", "OpenID", c => c.String());
            AddColumn("dbo.ServiceItemHistories", "ChangeValue", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ServiceItemHistories", "ChangeValue");
            DropColumn("dbo.Customers", "OpenID");
        }
    }
}

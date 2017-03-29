namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Customer27 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SupplierUsers", "UpdatePassWordTime", c => c.DateTimeOffset(nullable: false, precision: 7));
            AddColumn("dbo.Users", "UpdatePassWordTime", c => c.DateTimeOffset(nullable: false, precision: 7));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "UpdatePassWordTime");
            DropColumn("dbo.SupplierUsers", "UpdatePassWordTime");
        }
    }
}

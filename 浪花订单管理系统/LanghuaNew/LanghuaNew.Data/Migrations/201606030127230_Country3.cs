namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Country3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "IsConfirm", c => c.Boolean(nullable: false));
            AddColumn("dbo.ServiceItemHistories", "ServiceItemID", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ServiceItemHistories", "ServiceItemID");
            DropColumn("dbo.Orders", "IsConfirm");
        }
    }
}

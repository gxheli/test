namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Country14 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Customers", "IsNeedCustomerService", c => c.Boolean(nullable: false));
            AddColumn("dbo.Customers", "IsUsePassWord", c => c.Boolean(nullable: false));
            AddColumn("dbo.Orders", "TBNum", c => c.String());
            DropColumn("dbo.TBOrders", "IsSend");
            DropColumn("dbo.TBOrders", "TBNum");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TBOrders", "TBNum", c => c.String());
            AddColumn("dbo.TBOrders", "IsSend", c => c.Boolean(nullable: false));
            DropColumn("dbo.Orders", "TBNum");
            DropColumn("dbo.Customers", "IsUsePassWord");
            DropColumn("dbo.Customers", "IsNeedCustomerService");
        }
    }
}

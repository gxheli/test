namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Country7 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Elements", "ServiceItem_ServiceItemID", "dbo.ServiceItems");
            DropIndex("dbo.Elements", new[] { "ServiceItem_ServiceItemID" });
            AlterColumn("dbo.Customers", "Tel", c => c.String());
            AlterColumn("dbo.Customers", "BakTel", c => c.String());
            DropColumn("dbo.Elements", "ServiceItem_ServiceItemID");
            DropColumn("dbo.ExtraServices", "IsNeedDetail");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ExtraServices", "IsNeedDetail", c => c.Boolean(nullable: false));
            AddColumn("dbo.Elements", "ServiceItem_ServiceItemID", c => c.Int());
            AlterColumn("dbo.Customers", "BakTel", c => c.Int(nullable: false));
            AlterColumn("dbo.Customers", "Tel", c => c.Int(nullable: false));
            CreateIndex("dbo.Elements", "ServiceItem_ServiceItemID");
            AddForeignKey("dbo.Elements", "ServiceItem_ServiceItemID", "dbo.ServiceItems", "ServiceItemID");
        }
    }
}

namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Customer3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "CustomerName", c => c.String());
            AddColumn("dbo.Orders", "CustomerEnname", c => c.String());
            AddColumn("dbo.Orders", "Tel", c => c.String());
            AddColumn("dbo.Orders", "BakTel", c => c.String());
            AddColumn("dbo.Orders", "Email", c => c.String());
            AddColumn("dbo.Orders", "Wechat", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "Wechat");
            DropColumn("dbo.Orders", "Email");
            DropColumn("dbo.Orders", "BakTel");
            DropColumn("dbo.Orders", "Tel");
            DropColumn("dbo.Orders", "CustomerEnname");
            DropColumn("dbo.Orders", "CustomerName");
        }
    }
}

namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Customer36 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AlipayTransfers", "OrderNo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AlipayTransfers", "OrderNo");
        }
    }
}

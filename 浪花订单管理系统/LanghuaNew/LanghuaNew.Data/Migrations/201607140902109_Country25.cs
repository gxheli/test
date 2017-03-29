namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Country25 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AlipayTransfers", "TransferReason", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AlipayTransfers", "TransferReason");
        }
    }
}

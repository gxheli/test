namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Country26 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AlipayTransfers", "TransferTime", c => c.DateTimeOffset(nullable: false, precision: 7));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AlipayTransfers", "TransferTime");
        }
    }
}

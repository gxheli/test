namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Country44 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AlipayTransfers", "TransferReason");
            AddColumn("dbo.AlipayTransfers", "TransferReason", c => c.String());
            //AlterColumn("dbo.AlipayTransfers", "TransferReason", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AlipayTransfers", "TransferReason", c => c.Double(nullable: false));
        }
    }
}

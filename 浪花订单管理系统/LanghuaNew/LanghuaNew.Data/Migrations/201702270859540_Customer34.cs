namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Customer34 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TBOrderNoes", "Payment", c => c.Single(nullable: false));
            AddColumn("dbo.TBOrderNoes", "RefundFee", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TBOrderNoes", "RefundFee");
            DropColumn("dbo.TBOrderNoes", "Payment");
        }
    }
}

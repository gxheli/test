namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Customer35 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TBOrderNoes", "PaymentSplit", c => c.Single(nullable: false));
            AddColumn("dbo.TBOrderNoes", "RefundFeeSplit", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TBOrderNoes", "RefundFeeSplit");
            DropColumn("dbo.TBOrderNoes", "PaymentSplit");
        }
    }
}

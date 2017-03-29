namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Customer31 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SellControls", "LastUpdateTime", c => c.DateTimeOffset(nullable: false, precision: 7));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SellControls", "LastUpdateTime");
        }
    }
}

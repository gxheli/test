namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Customer17 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SellControls", "IsSurplusNum", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SellControls", "IsSurplusNum");
        }
    }
}
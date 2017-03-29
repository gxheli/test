namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Country42 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SellControls", "RowNum", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SellControls", "RowNum");
        }
    }
}

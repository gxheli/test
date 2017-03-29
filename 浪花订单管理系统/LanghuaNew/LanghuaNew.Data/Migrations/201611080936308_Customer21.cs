namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Customer21 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Roles", "RoleEnableState", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Roles", "RoleEnableState");
        }
    }
}

namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Country2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Orders", "CreateUserID", c => c.Int(nullable:true));
            DropColumn("dbo.Orders", "NetPrice");
            DropColumn("dbo.Orders", "UserID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "UserID", c => c.Int(nullable: false));
            AddColumn("dbo.Orders", "NetPrice", c => c.Single(nullable: false));
            AlterColumn("dbo.Orders", "CreateUserID", c => c.String());
        }
    }
}
 
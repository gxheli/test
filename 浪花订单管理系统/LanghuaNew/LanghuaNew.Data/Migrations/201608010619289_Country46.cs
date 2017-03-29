namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Country46 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderSourses", "ShowNo", c => c.Int(nullable: false));
            AddColumn("dbo.Users", "UserEnableState", c => c.Int(nullable: false));
            AddColumn("dbo.Users", "CreateTime", c => c.DateTimeOffset(nullable: false, precision: 7));
            DropColumn("dbo.Users", "IsDelete");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "IsDelete", c => c.Boolean(nullable: false));
            DropColumn("dbo.Users", "CreateTime");
            DropColumn("dbo.Users", "UserEnableState");
            DropColumn("dbo.OrderSourses", "ShowNo");
        }
    }
}

namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Country51 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserLoginLogs",
                c => new
                    {
                        UserLoginLogID = c.Int(nullable: false, identity: true),
                        UserID = c.Int(nullable: false),
                        UserName = c.String(),
                        LoginTime = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.UserLoginLogID);
            
            AddColumn("dbo.Customers", "IsBack", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Customers", "IsBack");
            DropTable("dbo.UserLoginLogs");
        }
    }
}

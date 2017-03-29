namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Country47 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserLogs",
                c => new
                    {
                        UserLogID = c.Int(nullable: false, identity: true),
                        OperUserID = c.String(),
                        OperUserNickName = c.String(),
                        OperTime = c.DateTimeOffset(nullable: false, precision: 7),
                        Remark = c.String(),
                        Operate = c.Int(nullable: false),
                        UserID = c.Int(nullable: false),
                        UserName = c.String(),
                    })
                .PrimaryKey(t => t.UserLogID)
                .ForeignKey("dbo.Users", t => t.UserID, cascadeDelete: true)
                .Index(t => t.UserID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserLogs", "UserID", "dbo.Users");
            DropIndex("dbo.UserLogs", new[] { "UserID" });
            DropTable("dbo.UserLogs");
        }
    }
}

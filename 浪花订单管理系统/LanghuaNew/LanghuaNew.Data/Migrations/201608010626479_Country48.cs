namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Country48 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserLogs", "UserID", "dbo.Users");
            DropIndex("dbo.UserLogs", new[] { "UserID" });
            DropTable("dbo.UserLogs");
        }
        
        public override void Down()
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
                .PrimaryKey(t => t.UserLogID);
            
            CreateIndex("dbo.UserLogs", "UserID");
            AddForeignKey("dbo.UserLogs", "UserID", "dbo.Users", "UserID", cascadeDelete: true);
        }
    }
}

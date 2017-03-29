namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Country49 : DbMigration
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
                .PrimaryKey(t => t.UserLogID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UserLogs");
        }
    }
}

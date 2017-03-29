namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Customer28 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.QRcodeStatistics",
                c => new
                    {
                        QRcodeStatisticID = c.Int(nullable: false, identity: true),
                        Code = c.Int(nullable: false),
                        IP = c.String(),
                        UserAgent = c.String(),
                        CreateTime = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.QRcodeStatisticID);
            
            CreateTable(
                "dbo.TB_Access_Token",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        access_token = c.String(maxLength: 100),
                        refresh_token_valid_time = c.Double(nullable: false),
                        refresh_token = c.String(maxLength: 100),
                        token_type = c.String(maxLength: 30),
                        r2_expires_in = c.Int(nullable: false),
                        w1_valid = c.Double(nullable: false),
                        r2_valid = c.Double(nullable: false),
                        r1_valid = c.Double(nullable: false),
                        w1_expires_in = c.Int(nullable: false),
                        w2_expires_in = c.Int(nullable: false),
                        w2_valid = c.Double(nullable: false),
                        r1_expires_in = c.Int(nullable: false),
                        expire_time = c.Double(nullable: false),
                        expires_in = c.Int(nullable: false),
                        re_expires_in = c.Int(nullable: false),
                        taobao_user_nick = c.String(maxLength: 50),
                        taobao_user_id = c.String(maxLength: 30),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TB_Access_Token");
            DropTable("dbo.QRcodeStatistics");
        }
    }
}

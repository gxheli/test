namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Customer2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WeixinMessages",
                c => new
                    {
                        WeixinMessageID = c.Int(nullable: false, identity: true),
                        StartTime = c.DateTimeOffset(nullable: false, precision: 7),
                        EndTime = c.DateTimeOffset(nullable: false, precision: 7),
                        WeixinMessageState = c.Int(nullable: false),
                        CountryID = c.Int(nullable: false),
                        Message = c.String(),
                        Url = c.String(),
                        LastEditDate = c.DateTimeOffset(nullable: false, precision: 7),
                        OperUserID = c.Int(nullable: false),
                        OperUserNickName = c.String(),
                    })
                .PrimaryKey(t => t.WeixinMessageID)
                .ForeignKey("dbo.Countries", t => t.CountryID, cascadeDelete: true)
                .Index(t => t.CountryID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WeixinMessages", "CountryID", "dbo.Countries");
            DropIndex("dbo.WeixinMessages", new[] { "CountryID" });
            DropTable("dbo.WeixinMessages");
        }
    }
}

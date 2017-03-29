namespace WXData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class First : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LHArticles",
                c => new
                    {
                        LHArticleID = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Description = c.String(),
                        PicUrl = c.String(),
                        Url = c.String(),
                        LHNew_LHNewID = c.Int(),
                    })
                .PrimaryKey(t => t.LHArticleID)
                .ForeignKey("dbo.LHNews", t => t.LHNew_LHNewID)
                .Index(t => t.LHNew_LHNewID);
            
            CreateTable(
                "dbo.LHNews",
                c => new
                    {
                        LHNewID = c.Int(nullable: false, identity: true),
                        media_id = c.String(),
                        update_time = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.LHNewID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LHArticles", "LHNew_LHNewID", "dbo.LHNews");
            DropIndex("dbo.LHArticles", new[] { "LHNew_LHNewID" });
            DropTable("dbo.LHNews");
            DropTable("dbo.LHArticles");
        }
    }
}

namespace WXData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Country2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.LHArticles", "LHNew_LHNewID", "dbo.LHNews");
            DropIndex("dbo.LHArticles", new[] { "LHNew_LHNewID" });
            RenameColumn(table: "dbo.LHArticles", name: "LHNew_LHNewID", newName: "LHNewID");
            AlterColumn("dbo.LHArticles", "LHNewID", c => c.Int(nullable: false));
            CreateIndex("dbo.LHArticles", "LHNewID");
            AddForeignKey("dbo.LHArticles", "LHNewID", "dbo.LHNews", "LHNewID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LHArticles", "LHNewID", "dbo.LHNews");
            DropIndex("dbo.LHArticles", new[] { "LHNewID" });
            AlterColumn("dbo.LHArticles", "LHNewID", c => c.Int());
            RenameColumn(table: "dbo.LHArticles", name: "LHNewID", newName: "LHNew_LHNewID");
            CreateIndex("dbo.LHArticles", "LHNew_LHNewID");
            AddForeignKey("dbo.LHArticles", "LHNew_LHNewID", "dbo.LHNews", "LHNewID");
        }
    }
}

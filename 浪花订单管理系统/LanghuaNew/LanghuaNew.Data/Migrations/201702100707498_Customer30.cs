namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Customer30 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SellControlClassifies",
                c => new
                    {
                        SellControlClassifyID = c.Int(nullable: false, identity: true),
                        ClassName = c.String(),
                        OrderBy = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SellControlClassifyID);
            
            CreateTable(
                "dbo.SellControlShows",
                c => new
                    {
                        SellControlShowID = c.Int(nullable: false, identity: true),
                        SellControlID = c.Int(nullable: false),
                        thisdate = c.String(),
                        date = c.String(),
                        TravelNum = c.Int(nullable: false),
                        PreTravelNum = c.Int(nullable: false),
                        ReturnNum = c.Int(nullable: false),
                        PreReturnNum = c.Int(nullable: false),
                        DistributionNum = c.Int(nullable: false),
                        state = c.Int(nullable: false),
                        ControlNum = c.Int(nullable: false),
                        ReMark = c.String(),
                    })
                .PrimaryKey(t => t.SellControlShowID)
                .ForeignKey("dbo.SellControls", t => t.SellControlID, cascadeDelete: true)
                .Index(t => t.SellControlID);
            
            AddColumn("dbo.SellControls", "SellControlClassifyID", c => c.Int());
            CreateIndex("dbo.SellControls", "SellControlClassifyID");
            AddForeignKey("dbo.SellControls", "SellControlClassifyID", "dbo.SellControlClassifies", "SellControlClassifyID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SellControlShows", "SellControlID", "dbo.SellControls");
            DropForeignKey("dbo.SellControls", "SellControlClassifyID", "dbo.SellControlClassifies");
            DropIndex("dbo.SellControlShows", new[] { "SellControlID" });
            DropIndex("dbo.SellControls", new[] { "SellControlClassifyID" });
            DropColumn("dbo.SellControls", "SellControlClassifyID");
            DropTable("dbo.SellControlShows");
            DropTable("dbo.SellControlClassifies");
        }
    }
}

namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Customer25 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FliterInfos",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        FliterNum = c.String(nullable: false, maxLength: 30),
                        DepartureCity = c.String(nullable: false),
                        FilterDeparture = c.String(nullable: false),
                        ArrivalCity = c.String(nullable: false),
                        FilterArrival = c.String(nullable: false),
                        SpliderTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.FliterNum);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.FliterInfos", new[] { "FliterNum" });
            DropTable("dbo.FliterInfos");
        }
    }
}

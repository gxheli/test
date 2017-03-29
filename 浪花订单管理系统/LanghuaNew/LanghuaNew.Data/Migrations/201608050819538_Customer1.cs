namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Customer1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CustomerBacks",
                c => new
                    {
                        CustomerBackID = c.Int(nullable: false, identity: true),
                        CustomerID = c.Int(nullable: false),
                        CustomerBackType = c.Int(nullable: false),
                        Remark = c.String(),
                        CreateData = c.DateTimeOffset(nullable: false, precision: 7),
                        OperateUserID = c.Int(nullable: false),
                        OperateUserNickName = c.String(),
                    })
                .PrimaryKey(t => t.CustomerBackID)
                .ForeignKey("dbo.Customers", t => t.CustomerID, cascadeDelete: true)
                .Index(t => t.CustomerID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CustomerBacks", "CustomerID", "dbo.Customers");
            DropIndex("dbo.CustomerBacks", new[] { "CustomerID" });
            DropTable("dbo.CustomerBacks");
        }
    }
}

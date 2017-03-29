namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Customer29 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CancelRegisterLogs",
                c => new
                    {
                        CancelRegisterLogID = c.Int(nullable: false, identity: true),
                        CancelRegisterID = c.Int(nullable: false),
                        OperUserID = c.Int(nullable: false),
                        OperUserNickName = c.String(),
                        OperTime = c.DateTimeOffset(nullable: false, precision: 7),
                        Operate = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.CancelRegisterLogID)
                .ForeignKey("dbo.CancelRegisters", t => t.CancelRegisterID, cascadeDelete: true)
                .Index(t => t.CancelRegisterID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CancelRegisterLogs", "CancelRegisterID", "dbo.CancelRegisters");
            DropIndex("dbo.CancelRegisterLogs", new[] { "CancelRegisterID" });
            DropTable("dbo.CancelRegisterLogs");
        }
    }
}

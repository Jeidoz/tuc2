namespace tuc2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addedteststableandmodifiedrelationships : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Tests",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IsSelected = c.Boolean(nullable: false),
                        InputData = c.String(maxLength: 4000),
                        OutputData = c.String(maxLength: 4000),
                        TestTask_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TestTasks", t => t.TestTask_Id)
                .Index(t => t.TestTask_Id);
            
            DropColumn("dbo.TestTasks", "InputFile");
            DropColumn("dbo.TestTasks", "OutputFile");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TestTasks", "OutputFile", c => c.String(maxLength: 4000));
            AddColumn("dbo.TestTasks", "InputFile", c => c.String(maxLength: 4000));
            DropForeignKey("dbo.Tests", "TestTask_Id", "dbo.TestTasks");
            DropIndex("dbo.Tests", new[] { "TestTask_Id" });
            DropTable("dbo.Tests");
        }
    }
}

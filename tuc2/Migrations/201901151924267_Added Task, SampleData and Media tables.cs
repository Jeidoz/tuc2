namespace tuc2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedTaskSampleDataandMediatables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Media",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ImagePath = c.String(maxLength: 4000),
                        TestTask_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TestTasks", t => t.TestTask_Id)
                .Index(t => t.TestTask_Id);
            
            CreateTable(
                "dbo.SampleDatas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IsInput = c.Boolean(nullable: false),
                        Content = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TestTasks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 4000),
                        Description = c.String(maxLength: 4000),
                        InputFile = c.String(maxLength: 4000),
                        OutputFile = c.String(maxLength: 4000),
                        InputExample_Id = c.Int(),
                        OutputExample_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SampleDatas", t => t.InputExample_Id)
                .ForeignKey("dbo.SampleDatas", t => t.OutputExample_Id)
                .Index(t => t.InputExample_Id)
                .Index(t => t.OutputExample_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TestTasks", "OutputExample_Id", "dbo.SampleDatas");
            DropForeignKey("dbo.Media", "TestTask_Id", "dbo.TestTasks");
            DropForeignKey("dbo.TestTasks", "InputExample_Id", "dbo.SampleDatas");
            DropIndex("dbo.TestTasks", new[] { "OutputExample_Id" });
            DropIndex("dbo.TestTasks", new[] { "InputExample_Id" });
            DropIndex("dbo.Media", new[] { "TestTask_Id" });
            DropTable("dbo.TestTasks");
            DropTable("dbo.SampleDatas");
            DropTable("dbo.Media");
        }
    }
}

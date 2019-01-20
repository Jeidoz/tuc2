namespace tuc2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SimplifiedinputandoutputexamplesinTestTaskstable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TestTasks", "InputExample_Id", "dbo.SampleDatas");
            DropForeignKey("dbo.TestTasks", "OutputExample_Id", "dbo.SampleDatas");
            DropIndex("dbo.TestTasks", new[] { "InputExample_Id" });
            DropIndex("dbo.TestTasks", new[] { "OutputExample_Id" });
            AddColumn("dbo.TestTasks", "InputExample", c => c.String(maxLength: 4000));
            AddColumn("dbo.TestTasks", "OutputExample", c => c.String(maxLength: 4000));
            DropColumn("dbo.TestTasks", "InputExample_Id");
            DropColumn("dbo.TestTasks", "OutputExample_Id");
            DropTable("dbo.SampleDatas");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.SampleDatas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IsInput = c.Boolean(nullable: false),
                        Content = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.TestTasks", "OutputExample_Id", c => c.Int());
            AddColumn("dbo.TestTasks", "InputExample_Id", c => c.Int());
            DropColumn("dbo.TestTasks", "OutputExample");
            DropColumn("dbo.TestTasks", "InputExample");
            CreateIndex("dbo.TestTasks", "OutputExample_Id");
            CreateIndex("dbo.TestTasks", "InputExample_Id");
            AddForeignKey("dbo.TestTasks", "OutputExample_Id", "dbo.SampleDatas", "Id");
            AddForeignKey("dbo.TestTasks", "InputExample_Id", "dbo.SampleDatas", "Id");
        }
    }
}

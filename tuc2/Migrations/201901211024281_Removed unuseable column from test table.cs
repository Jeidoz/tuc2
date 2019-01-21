namespace tuc2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Removedunuseablecolumnfromtesttable : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Tests", "IsSelected");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Tests", "IsSelected", c => c.Boolean(nullable: false));
        }
    }
}

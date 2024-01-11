namespace DBA5.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update1 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Actors", newName: "Actor");
            RenameTable(name: "dbo.Shows", newName: "Show");
            RenameTable(name: "dbo.Episodes", newName: "Episode");
            RenameTable(name: "dbo.Genres", newName: "Genre");
            RenameColumn(table: "dbo.Episode", name: "ShowId", newName: "Show_Id");
            RenameIndex(table: "dbo.Episode", name: "IX_ShowId", newName: "IX_Show_Id");
            CreateTable(
                "dbo.ActorMediaItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Content = c.Binary(),
                        ContentType = c.String(maxLength: 200),
                        Caption = c.String(nullable: false, maxLength: 100),
                        Actor_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Actor", t => t.Actor_Id)
                .Index(t => t.Actor_Id);
            
            AddColumn("dbo.Actor", "Biography", c => c.String());
            AddColumn("dbo.Show", "Premise", c => c.String());
            AddColumn("dbo.Episode", "Premise", c => c.String());
            AddColumn("dbo.Episode", "VideoContentType", c => c.String(maxLength: 200));
            AddColumn("dbo.Episode", "Video", c => c.Binary());
            AlterColumn("dbo.Genre", "Name", c => c.String(nullable: false, maxLength: 200));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ActorMediaItems", "Actor_Id", "dbo.Actor");
            DropIndex("dbo.ActorMediaItems", new[] { "Actor_Id" });
            AlterColumn("dbo.Genre", "Name", c => c.String(nullable: false, maxLength: 50));
            DropColumn("dbo.Episode", "Video");
            DropColumn("dbo.Episode", "VideoContentType");
            DropColumn("dbo.Episode", "Premise");
            DropColumn("dbo.Show", "Premise");
            DropColumn("dbo.Actor", "Biography");
            DropTable("dbo.ActorMediaItems");
            RenameIndex(table: "dbo.Episode", name: "IX_Show_Id", newName: "IX_ShowId");
            RenameColumn(table: "dbo.Episode", name: "Show_Id", newName: "ShowId");
            RenameTable(name: "dbo.Genre", newName: "Genres");
            RenameTable(name: "dbo.Episode", newName: "Episodes");
            RenameTable(name: "dbo.Show", newName: "Shows");
            RenameTable(name: "dbo.Actor", newName: "Actors");
        }
    }
}

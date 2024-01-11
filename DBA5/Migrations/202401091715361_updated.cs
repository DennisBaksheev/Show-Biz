namespace DBA5.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updated : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ActorMediaItems", "ActorId", "dbo.Actors");
            DropIndex("dbo.ActorMediaItems", new[] { "ActorId" });
            DropColumn("dbo.Actors", "Biography");
            DropColumn("dbo.Shows", "Premise");
            DropColumn("dbo.Episodes", "Premise");
            DropColumn("dbo.Episodes", "MediaItemId");
            DropColumn("dbo.Episodes", "Video");
            DropColumn("dbo.Episodes", "VideoContentType");
            DropColumn("dbo.Episodes", "VideoPath");
            DropTable("dbo.ActorMediaItems");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ActorMediaItems",
                c => new
                    {
                        MediaItemId = c.Int(nullable: false, identity: true),
                        Content = c.Binary(),
                        ContentType = c.String(maxLength: 256),
                        FileName = c.String(maxLength: 256),
                        Caption = c.String(),
                        ActorId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.MediaItemId);
            
            AddColumn("dbo.Episodes", "VideoPath", c => c.String());
            AddColumn("dbo.Episodes", "VideoContentType", c => c.String());
            AddColumn("dbo.Episodes", "Video", c => c.Binary());
            AddColumn("dbo.Episodes", "MediaItemId", c => c.Int());
            AddColumn("dbo.Episodes", "Premise", c => c.String());
            AddColumn("dbo.Shows", "Premise", c => c.String());
            AddColumn("dbo.Actors", "Biography", c => c.String());
            CreateIndex("dbo.ActorMediaItems", "ActorId");
            AddForeignKey("dbo.ActorMediaItems", "ActorId", "dbo.Actors", "Id");
        }
    }
}

namespace Instant.Server.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedChatDeletionEntity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ChatDeletions",
                c => new
                    {
                        ChatId = c.Int(nullable: false),
                        DateDeleted = c.DateTime(nullable: false),
                        Initiator_Login = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ChatId)
                .ForeignKey("dbo.Chats", t => t.ChatId)
                .ForeignKey("dbo.Users", t => t.Initiator_Login)
                .Index(t => t.ChatId)
                .Index(t => t.Initiator_Login);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ChatDeletions", "Initiator_Login", "dbo.Users");
            DropForeignKey("dbo.ChatDeletions", "ChatId", "dbo.Chats");
            DropIndex("dbo.ChatDeletions", new[] { "Initiator_Login" });
            DropIndex("dbo.ChatDeletions", new[] { "ChatId" });
            DropTable("dbo.ChatDeletions");
        }
    }
}

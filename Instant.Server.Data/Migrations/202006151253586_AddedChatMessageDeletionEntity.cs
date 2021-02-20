namespace Instant.Server.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedChatMessageDeletionEntity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ChatMessageDeletions",
                c => new
                    {
                        ChatMessageId = c.Int(nullable: false),
                        DateDeleted = c.DateTime(nullable: false),
                        Initiator_Login = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ChatMessageId)
                .ForeignKey("dbo.ChatMessages", t => t.ChatMessageId)
                .ForeignKey("dbo.Users", t => t.Initiator_Login)
                .Index(t => t.ChatMessageId)
                .Index(t => t.Initiator_Login);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ChatMessageDeletions", "Initiator_Login", "dbo.Users");
            DropForeignKey("dbo.ChatMessageDeletions", "ChatMessageId", "dbo.ChatMessages");
            DropIndex("dbo.ChatMessageDeletions", new[] { "Initiator_Login" });
            DropIndex("dbo.ChatMessageDeletions", new[] { "ChatMessageId" });
            DropTable("dbo.ChatMessageDeletions");
        }
    }
}

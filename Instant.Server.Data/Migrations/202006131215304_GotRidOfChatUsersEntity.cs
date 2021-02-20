namespace Instant.Server.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GotRidOfChatUsersEntity : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ChatUsers", "ChatId", "dbo.Chats");
            DropForeignKey("dbo.ChatUsers", "UserLogin", "dbo.Users");
            DropIndex("dbo.ChatUsers", new[] { "ChatId" });
            DropIndex("dbo.ChatUsers", new[] { "UserLogin" });
            DropTable("dbo.ChatUsers");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ChatUsers",
                c => new
                    {
                        ChatId = c.Int(nullable: false),
                        UserLogin = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.ChatId, t.UserLogin });
            
            CreateIndex("dbo.ChatUsers", "UserLogin");
            CreateIndex("dbo.ChatUsers", "ChatId");
            AddForeignKey("dbo.ChatUsers", "UserLogin", "dbo.Users", "Login", cascadeDelete: true);
            AddForeignKey("dbo.ChatUsers", "ChatId", "dbo.Chats", "ChatId", cascadeDelete: true);
        }
    }
}

namespace Instant.Server.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialEntities : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ChatMessages",
                c => new
                    {
                        ChatMessageId = c.Int(nullable: false, identity: true),
                        DateSent = c.DateTime(nullable: false),
                        Sender_Login = c.String(maxLength: 128),
                        Chat_ChatId = c.Int(),
                    })
                .PrimaryKey(t => t.ChatMessageId)
                .ForeignKey("dbo.Users", t => t.Sender_Login)
                .ForeignKey("dbo.Chats", t => t.Chat_ChatId)
                .Index(t => t.Sender_Login)
                .Index(t => t.Chat_ChatId);
            
            CreateTable(
                "dbo.Chats",
                c => new
                    {
                        ChatId = c.Int(nullable: false, identity: true),
                        ChatType = c.Int(nullable: false),
                        Title = c.String(),
                        LastUpdate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ChatId);
            
            CreateTable(
                "dbo.ChatUsers",
                c => new
                    {
                        ChatId = c.Int(nullable: false),
                        UserLogin = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.ChatId, t.UserLogin })
                .ForeignKey("dbo.Chats", t => t.ChatId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserLogin, cascadeDelete: true)
                .Index(t => t.ChatId)
                .Index(t => t.UserLogin);
            
            CreateTable(
                "dbo.ChatPermissions",
                c => new
                    {
                        ChatPermissionId = c.Int(nullable: false, identity: true),
                        PermissionType = c.Int(nullable: false),
                        LastUpdate = c.DateTime(nullable: false),
                        Chat_ChatId = c.Int(),
                        ChatUser_Login = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ChatPermissionId)
                .ForeignKey("dbo.Chats", t => t.Chat_ChatId)
                .ForeignKey("dbo.Users", t => t.ChatUser_Login)
                .Index(t => t.Chat_ChatId)
                .Index(t => t.ChatUser_Login);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Login = c.String(nullable: false, maxLength: 128),
                        Password = c.String(),
                        LastUpdate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Login);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ChatMessages", "Chat_ChatId", "dbo.Chats");
            DropForeignKey("dbo.ChatPermissions", "ChatUser_Login", "dbo.Users");
            DropForeignKey("dbo.ChatUsers", "UserLogin", "dbo.Users");
            DropForeignKey("dbo.ChatMessages", "Sender_Login", "dbo.Users");
            DropForeignKey("dbo.ChatPermissions", "Chat_ChatId", "dbo.Chats");
            DropForeignKey("dbo.ChatUsers", "ChatId", "dbo.Chats");
            DropIndex("dbo.ChatPermissions", new[] { "ChatUser_Login" });
            DropIndex("dbo.ChatPermissions", new[] { "Chat_ChatId" });
            DropIndex("dbo.ChatUsers", new[] { "UserLogin" });
            DropIndex("dbo.ChatUsers", new[] { "ChatId" });
            DropIndex("dbo.ChatMessages", new[] { "Chat_ChatId" });
            DropIndex("dbo.ChatMessages", new[] { "Sender_Login" });
            DropTable("dbo.Users");
            DropTable("dbo.ChatPermissions");
            DropTable("dbo.ChatUsers");
            DropTable("dbo.Chats");
            DropTable("dbo.ChatMessages");
        }
    }
}

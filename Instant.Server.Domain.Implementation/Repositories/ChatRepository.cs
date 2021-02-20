using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using Instant.Server.Data;
using Instant.Server.Data.Entities;
using Instant.Server.Domain.Enums;
using Instant.Server.Domain.Repositories;

namespace Instant.Server.Domain.Implementation.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly InstantServerDbContext dbContext;
        
        public ChatRepository(InstantServerDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Chat GetChat(int chatId)
        {
            var chat = this.dbContext.Chats.Find(chatId);
            return chat;
        }

        public IList<Chat> GetChats()
        {
            var chats = this.dbContext.Chats.ToList();
            return chats;
        }

        public IList<User> GetMembersOfChat(int chatId)
        {
            var chat = GetChat(chatId);
            _ = chat ?? throw new ArgumentException($"Can not get members of chat #{chatId.ToString()}, because it is not present");

            var userIds = this.dbContext.ChatPermissions
                .Where(chu => chu.Chat.ChatId == chat.ChatId
                    && (chu.PermissionType != (int) ChatPermissionTypes.None 
                    || chu.PermissionType != (int) ChatPermissionTypes.Banned))
                .Select(chu => chu.ChatUser.Login)
                .ToList();
            
            var users = this.dbContext.Users
                .Where(u => userIds
                    .Any(uId => uId == u.Login))
                .ToList();
            
            return users;
        }

        public IList<Chat> GetChatsOfUser(string userLogin)
        {
            var chatIds = this.dbContext.ChatPermissions
                .Where(chu => chu.ChatUser.Login == userLogin)
                .Select(chu => chu.Chat.ChatId)
                .ToList();

            var chats = chatIds
                .Select(GetChat)
                .ToList();

            return chats;
        }

        public void SaveChat(Chat chat)
        {
            _ = chat ?? throw new ArgumentNullException("Can not save chat, because it is null");
            dbContext.Chats.AddOrUpdate(chat);
            dbContext.SaveChanges();
        }

        public IList<ChatDeletion> GetChatDeletions(string userLogin)
        {
            var chatDeletions = this.dbContext.ChatDeletions
                .Where(chd => chd.Chat.ChatPermissions
                    .Any(chp => chp.ChatUser.Login == userLogin))
                .ToList();
            return chatDeletions;
        }

        public void DeleteChat(Chat chat, string initiatorLogin)
        {
            var initiator = this.dbContext.Users.Find(initiatorLogin);
            var chatDeletion = new ChatDeletion();
            chatDeletion.ChatId = chat.ChatId;
            chatDeletion.Chat = chat;
            chatDeletion.Initiator = initiator;
            chatDeletion.DateDeleted = DateTime.Now;
            this.dbContext.ChatDeletions.AddOrUpdate(chatDeletion);
            this.dbContext.SaveChanges();
        }

        public bool IsChatDeleted(int chatId)
        {
            return this.dbContext.ChatDeletions
                .FirstOrDefault(chd => chd.ChatId == chatId) == null;
        }
    }
}
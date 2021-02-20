using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using Instant.Server.Data;
using Instant.Server.Data.Entities;
using Instant.Server.Domain.Repositories;

namespace Instant.Server.Domain.Implementation.Repositories
{
    public class ChatPermissionRepository : IChatPermissionRepository
    {
        private readonly InstantServerDbContext dbContext;
        
        public ChatPermissionRepository(InstantServerDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public ChatPermission GetChatPermission(int chatPermissionId)
        {
            var chatPermission = this.dbContext.ChatPermissions.Find(chatPermissionId);
            return chatPermission;
        }

        public ChatPermission GetChatPermission(string userLogin, int chatId)
        {
            var chatPermission = this.dbContext.ChatPermissions
                .FirstOrDefault(cp => cp.ChatUser.Login == userLogin
                                      && cp.Chat.ChatId == chatId);

            return chatPermission;
        }

        public IList<ChatPermission> GetChatPermissions(int chatId)
        {
            var chatPermissions = this.dbContext.ChatPermissions
                .Where(chp => chp.Chat.ChatId == chatId)
                .ToList();

            return chatPermissions;
        }

        public IList<ChatPermission> GetChatPermissions(string userLogin)
        {
            var chatPermissions = this.dbContext.ChatPermissions
                .Where(chp => chp.ChatUser.Login == userLogin)
                .ToList();

            return chatPermissions;
        }

        public void SaveChatPermission(ChatPermission chatPermission)
        {
            _ = chatPermission ?? throw new ArgumentNullException("Can not save chat permission, because it is null");
            this.dbContext.ChatPermissions.AddOrUpdate(chatPermission);
            this.dbContext.SaveChanges();
        }

        public void DeleteChatPermission(ChatPermission chatPermission)
        {
            _ = chatPermission ?? throw new ArgumentNullException("Can not delete chat permission," +
                                                                  "because it is null");

            this.dbContext.ChatPermissions.Remove(chatPermission);
            this.dbContext.SaveChanges();
        }

        public void DeleteChatPermission(int chatPermissionId)
        {
            var chatPermission = GetChatPermission(chatPermissionId);
            _ = chatPermission ?? throw new ArgumentException($"Can not delete chat " +
                                                              $"#{chatPermissionId.ToString()}, because it is null");
            
            DeleteChatPermission(chatPermission);
        }
    }
}
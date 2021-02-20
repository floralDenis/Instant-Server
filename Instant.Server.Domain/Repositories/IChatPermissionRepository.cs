using System.Collections.Generic;
using Instant.Server.Data.Entities;

namespace Instant.Server.Domain.Repositories
{
    public interface IChatPermissionRepository
    {
        ChatPermission GetChatPermission(int chatPermissionId);
        ChatPermission GetChatPermission(string userLogin, int chatId);
        IList<ChatPermission> GetChatPermissions(int chatId);
        IList<ChatPermission> GetChatPermissions(string userLogin);
        void SaveChatPermission(ChatPermission chatPermission);
        void DeleteChatPermission(ChatPermission chatPermission);
        void DeleteChatPermission(int chatPermissionId);
    }
}
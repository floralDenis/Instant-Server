using System;
using System.Collections.Generic;
using Instant.Server.Domain.Enums;
using Instant.Server.Domain.Models;
using Instant.Server.Domain.OperationOptions;

namespace Instant.Server.Domain.Services
{
    public interface IChatPermissionService
    {
        ChatPermission GetChatPermission(string userLogin, int chatId);
        IList<ChatPermission> GetMissedChatPermissions(string userLogin, DateTime lastOnline);
        void AddOrUpdateChatPermission(AddOrUpdateChatPermissionOptions options);
        void DeleteChatPermissions(int chatId);
        bool DoesUserHavePermission(string userLogin, int chatId, ChatPermissionTypes permissionType);
    }
}
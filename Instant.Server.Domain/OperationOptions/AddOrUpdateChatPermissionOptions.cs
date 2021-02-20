using System;
using Instant.Server.Domain.Enums;

namespace Instant.Server.Domain.OperationOptions
{
    public class AddOrUpdateChatPermissionOptions
    {
        public int PermissionId { get; }
        public string UserLogin { get; }
        public int ChatId { get; }
        public ChatPermissionTypes PermissionType { get; }
        public string InitiatorLogin { get; }
        public DateTime LastUpdate { get; }

        public AddOrUpdateChatPermissionOptions(
            int permissionId = 0,
            string userLogin = "",
            int chatId = 0,
            ChatPermissionTypes permissionType = ChatPermissionTypes.None,
            string initiatorLogin = "",
            DateTime lastUpdate = default)
        {
            this.PermissionId = permissionId;
            this.UserLogin = userLogin;
            this.ChatId = chatId;
            this.PermissionType = permissionType;
            this.InitiatorLogin = initiatorLogin;
            this.LastUpdate = lastUpdate;
        }
    }
}
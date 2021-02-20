using System;

namespace Instant.Server.Domain.Models
{
    public class ChatPermission
    {
        public int ChatPermissionId { get; set; }
        public string ChatMemberLogin { get; set; }
        public int ChatId { get; set; }
        public int PermissionType { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
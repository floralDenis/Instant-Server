using System;

namespace Instant.Server.Data.Entities
{
    public class ChatPermission
    {
        public int ChatPermissionId { get; set; }
        public User ChatUser { get; set; }
        public Chat Chat { get; set; }
        public int PermissionType { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
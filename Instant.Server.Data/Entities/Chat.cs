using System;
using System.Collections.Generic;

namespace Instant.Server.Data.Entities
{
    public class Chat
    {
        public int ChatId { get; set; }
        public int ChatType { get; set; }
        public string Title { get; set; }
        public DateTime LastUpdate { get; set; }
        public ICollection<ChatMessage> Messages { get; set; }
        public ICollection<ChatPermission> ChatPermissions { get; set; }
    }
}
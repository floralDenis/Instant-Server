using System;

namespace Instant.Server.Domain.Models
{
    public class Chat
    {
        public int ChatId { get; set; }
        public int ChatType { get; set; }
        public string Title { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
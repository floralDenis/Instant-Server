using System;

namespace Instant.Server.Data.Entities
{
    public class ChatMessage
    {
        public int ChatMessageId { get; set; }
        public User Sender { get; set; }
        public Chat Chat { get; set; }
        public string Text { get; set; }
        public DateTime DateSent { get; set; }
    }
}
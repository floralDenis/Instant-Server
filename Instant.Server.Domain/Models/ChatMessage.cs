using System;

namespace Instant.Server.Domain.Models
{
    public class ChatMessage
    {
        public int MessageId { get; }
        public int ChatId { get; }
        public string SenderLogin { get; }
        public string Text { get; }
        public DateTime DateSent { get; set; }

        public ChatMessage(
            int messageId,
            int chatId,
            string senderLogin,
            string text,
            DateTime dateSent)
        {
            this.MessageId = messageId;
            this.ChatId = chatId;
            this.SenderLogin = senderLogin;
            this.Text = text;
            this.DateSent = dateSent;
        }
    }
}
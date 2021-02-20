using Instant.Server.Domain.Models;

namespace Instant.Server.BLL
{
    public static class Messages
    {
        public static bool IsMessageValid(ChatMessage chatMessage)
        {
            return !string.IsNullOrEmpty(chatMessage.Text)
                   && !string.IsNullOrWhiteSpace(chatMessage.Text);
        }
    }
}
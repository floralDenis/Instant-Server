using System.Collections.Generic;
using Instant.Server.Data.Entities;

namespace Instant.Server.Domain.Repositories
{
    public interface IChatMessageRepository
    {
        IList<ChatMessage> GetMessages(int chatId);
        ChatMessage GetMessage(int chatMessageId);
        void SaveMessage(ChatMessage chatMessage);
        IList<ChatMessageDeletion> GetChatMessageDeletions(int chatId);
        void DeleteMessage(ChatMessage chatMessage, string initiatorLogin);
    }
}
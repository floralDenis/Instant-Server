using System;
using System.Collections.Generic;
using Instant.Server.Domain.Models;

namespace Instant.Server.Domain.Services
{
    public interface IChatMessageService
    {
        IList<ChatMessage> GetMissedMessages(int chatId, DateTime lastOnline);
        void SaveMessage(ChatMessage chatMessage);
        IList<ChatMessage> GetMissedDeletedMessages(int chatId, DateTime lastOnline);
        void DeleteMessage(int chatMessageId, string initiatorLogin);
        void DeleteMessages(int chatId, string initiatorLogin);
        int GetChatIdByMessageId(int chatMessageId);
    }
}
using System;
using System.Collections.Generic;
using Instant.Server.Domain.Models;
using Instant.Server.Domain.OperationOptions;

namespace Instant.Server.Domain.Services
{
    public interface IChatService
    {
        bool DoesChatExist(int chatId);
        Chat CreateOrUpdateChat(CreateOrUpdateChatOptions options);
        void DeleteChat(int chatId, string initiatorLogin);
        void SendMessage(ChatMessage chatMessage);
        IList<User> GetAffectedUsers(string userLogin);
        IList<User> GetMembersOfChat(int chatId);
        IList<Chat> GetChatsOfUser(string userLogin);
        IList<ChatMessage> GetMissedMessages(string userLogin, DateTime lastOnline);
        IList<Chat> GetMissedChatUpdates(string userLogin, DateTime lastOnline);
        IList<ChatMessage> GetMissedDeletedMessages(string userLogin, DateTime lastOnline);
        IList<Chat> GetMissedDeletedChats(string userLogin, DateTime lastOnline);
    }
}
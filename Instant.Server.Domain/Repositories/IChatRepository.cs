using System.Collections.Generic;
using Instant.Server.Data.Entities;

namespace Instant.Server.Domain.Repositories
{
    public interface IChatRepository
    {
        Chat GetChat(int chatId);
        IList<Chat> GetChats();
        IList<User> GetMembersOfChat(int chatId);
        IList<Chat> GetChatsOfUser(string userLogin);
        void SaveChat(Chat chat);
        IList<ChatDeletion> GetChatDeletions(string userLogin);
        void DeleteChat(Chat chat, string initiatorLogin);
        bool IsChatDeleted(int chatId);
    }
}
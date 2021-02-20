using System.Collections.Generic;
using Instant.Server.Data.Entities;

namespace Instant.Server.Domain.Repositories
{
    public interface IUserRepository
    {
        User GetUser(string userLogin);
        IList<User> GetAllUsers();
        IList<Chat> GetChatsOfUser(string userLogin);
        void SaveUser(User user);
        void DeleteUser(User user);
        void DeleteUser(string userLogin);
    }
}
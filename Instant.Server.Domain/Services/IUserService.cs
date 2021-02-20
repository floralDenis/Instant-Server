using System.Collections.Generic;
using Instant.Server.Domain.Models;
using Instant.Server.Domain.OperationOptions;

namespace Instant.Server.Domain.Services
{
    public interface IUserService
    {
        bool DoesUserExist(string userLogin);
        User GetUser(string login);
        IList<User> GetUsers(IList<string> logins);
        User SignIn(AuthorizeUserOptions options);
        void SignUp(AuthorizeUserOptions options);
        void DeleteAccount(AuthorizeUserOptions options);
        IList<Chat> GetChatsOfUser(string userLogin);
    }
}
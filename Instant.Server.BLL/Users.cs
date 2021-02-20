using System.Linq;
using System.Collections.Generic;
using Instant.Server.Domain.Models;

namespace Instant.Server.BLL
{
    public static class Users
    {
        public static bool IsUserDataUnique(User user, IList<User> users)
        {
            bool isUnique = users.All(u => u.Login != user.Login);
            return isUnique;
        }

        public static bool AreUserCredentialsValid(string login, string password)
        {
            return !string.IsNullOrEmpty(login) && !string.IsNullOrWhiteSpace(login)
                   && !string.IsNullOrEmpty(password) && !string.IsNullOrWhiteSpace(password);
        }
    }
}
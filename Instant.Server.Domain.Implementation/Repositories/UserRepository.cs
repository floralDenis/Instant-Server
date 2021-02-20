using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using Instant.Server.Data;
using Instant.Server.Data.Entities;
using Instant.Server.Domain.Repositories;

namespace Instant.Server.Domain.Implementation.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly InstantServerDbContext dbContext;

        public UserRepository(InstantServerDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public User GetUser(string userLogin)
        {
            var user = dbContext.Users.FirstOrDefault(u => u.Login == userLogin);
            return user;
        }

        public IList<User> GetAllUsers()
        {
            var users = dbContext.Users.ToList();
            return users;
        }

        public IList<Chat> GetChatsOfUser(string userLogin)
        {
            var user = GetUser(userLogin);
            _ = user ?? throw new ArgumentException($"Can not get chats of user #{userLogin.ToString()}, no such user");

            var chatIds = dbContext.ChatPermissions
                .Where(chu => chu.ChatUser.Login == user.Login)
                .Select(chu => chu.Chat.ChatId)
                .ToList();
            
            var chats = dbContext.Chats
                .Where(ch => chatIds
                    .Any(chId => chId == ch.ChatId))
                .ToList();

            return chats;
        }

        public void SaveUser(User user)
        {
            _ = user ?? throw new ArgumentNullException("Can not save user, because it is null");
            this.dbContext.Users.AddOrUpdate(user);
            
            this.dbContext.SaveChanges();
        }

        public void DeleteUser(User user)
        {
            _ = user ?? throw new ArgumentNullException("Can not delete user, because it is null");
            this.dbContext.Users.Remove(user);
            
            this.dbContext.SaveChanges();
        }

        public void DeleteUser(string userLogin)
        {
            var existingUser = GetUser(userLogin);
            
            _ = existingUser ?? throw new ArgumentNullException($"Can not delete user, because user #{userLogin.ToString()} it is null");
            DeleteUser(existingUser);
            
            this.dbContext.SaveChanges();
        }
    }
}
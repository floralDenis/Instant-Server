using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Instant.Server.BLL;
using Instant.Server.Domain.Models;
using Instant.Server.Domain.OperationOptions;
using Instant.Server.Domain.Repositories;
using Instant.Server.Domain.Services;
using DataEntities = Instant.Server.Data.Entities;

namespace Instant.Server.Domain.Implementation.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;
        
        public UserService(
            IUserRepository userRepository,
            IMapper mapper)
        {
            this.userRepository = userRepository;
            this.mapper = mapper;
        }

        public bool DoesUserExist(string login)
        {
            var user = GetUser(login);
            return user != null;
        }

        public User GetUser(string login)
        {
            var user = userRepository.GetUser(login);
            var domainUser = this.mapper.Map<User>(user);
            return domainUser;
        }

        public IList<User> GetUsers(IList<string> logins)
        {
            var users = logins.Select(GetUser).ToList();
            return users;
        }

        public User SignIn(AuthorizeUserOptions options)
        {
            var user = userRepository.GetUser(options.Login);
            _ = user ?? throw new ArgumentException($"Can not sign in. No user with login '{options.Login}'");

            if (user.Password != options.Password)
            {
                throw new ArgumentException($"Can not sign in. Wrong password provided for login '{options.Login}'");
            }

            var domainUser = mapper.Map<User>(user);
            return domainUser;
        }

        public void SignUp(AuthorizeUserOptions options)
        {
            var domainUser = mapper.Map<User>(options);
            var usersInDb = userRepository.GetAllUsers();
            var users = mapper.Map<IList<User>>(usersInDb);

            if (!Users.IsUserDataUnique(domainUser, users))
            {
                throw new ArgumentException($"User with such login ('{options.Login}') already exists");
            }

            if (!Users.AreUserCredentialsValid(options.Login, options.Password))
            {
                throw new ArgumentException($"Provided user credentials for login '{options.Login}' are not valid");
            }

            var dataUser = mapper.Map<DataEntities.User>(options);
            dataUser.LastUpdate = DateTime.Now;
            
            userRepository.SaveUser(dataUser);
        }

        public void DeleteAccount(AuthorizeUserOptions options)
        {
            var user = SignIn(options);
            userRepository.DeleteUser(user.Login);
        }

        public IList<Chat> GetChatsOfUser(string userLogin)
        {
            var chats = userRepository.GetChatsOfUser(userLogin);
            var domainChats = mapper.Map<IList<Chat>>(chats);
            return domainChats;
        }
    }
}
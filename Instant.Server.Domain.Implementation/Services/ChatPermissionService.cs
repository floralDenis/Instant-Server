using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Instant.Server.BLL;
using Instant.Server.Domain.Enums;
using Instant.Server.Domain.Models;
using Instant.Server.Domain.OperationOptions;
using Instant.Server.Domain.Repositories;
using Instant.Server.Domain.Services;
using DataEntities = Instant.Server.Data.Entities;

namespace Instant.Server.Domain.Implementation.Services
{
    public class ChatPermissionService : IChatPermissionService
    {
        private readonly IChatPermissionRepository chatPermissionRepository;
        private readonly IUserRepository userRepository;
        private readonly IChatRepository chatRepository;
        private readonly IUserService userService;
        private readonly IMapper mapper;

        public const string SYSTEM_PERMISSION = "INSTANT_PERMISSION";
        
        public ChatPermissionService(
            IChatPermissionRepository chatPermissionRepository,
            IUserRepository userRepository,
            IChatRepository chatRepository,
            IUserService userService,
            IMapper mapper)
        {
            this.chatPermissionRepository = chatPermissionRepository;
            this.userRepository = userRepository;
            this.chatRepository = chatRepository;
            this.userService = userService;
            this.mapper = mapper;
        }

        public ChatPermission GetChatPermission(string userLogin, int chatId)
        {
            if (!this.userService.DoesUserExist(userLogin))
            {
                throw new ArgumentException($"Chat #{chatId.ToString()} or user #{userLogin.ToString()} does not exist");
            }
            
            var chatPermission = chatPermissionRepository.GetChatPermission(userLogin, chatId);
            var domainChatPermission = mapper.Map<ChatPermission>(chatPermission);
            return domainChatPermission;
        }

        public IList<ChatPermission> GetMissedChatPermissions(string userLogin, DateTime lastOnline)
        {
            var chatPermissions = this.chatPermissionRepository.GetChatPermissions(userLogin);
            var missedChatPermissions = chatPermissions
                .Where(chp => chp.LastUpdate > lastOnline)
                .ToList();

            var domainMissedChatPermissions = this.mapper.Map<IList<ChatPermission>>(missedChatPermissions);
            return domainMissedChatPermissions;
        }

        public void AddOrUpdateChatPermission(AddOrUpdateChatPermissionOptions options)
        {
            if (string.IsNullOrEmpty(options.UserLogin)
                || string.IsNullOrEmpty(options.InitiatorLogin)
                || !this.userService.DoesUserExist(options.UserLogin)
                || (options.InitiatorLogin != SYSTEM_PERMISSION
                    && !this.userService.DoesUserExist(options.InitiatorLogin)))
            {
                throw new ArgumentException($"Chat #{options.ChatId.ToString()} or " +
                                            $"user #{options.UserLogin.ToString()} or" +
                                            $"initiator user #{options.InitiatorLogin.ToString()} does not exist");
            }

            if (options.InitiatorLogin != SYSTEM_PERMISSION
                && options.InitiatorLogin != options.UserLogin
                && !DoesUserHavePermission(options.InitiatorLogin, options.ChatId, ChatPermissionTypes.Moderate))
            {
                throw new InvalidOperationException($"User {options.InitiatorLogin} does not have permissions" +
                                                    $"to assign to {options.UserLogin} new chat permission");
            }

            var entityChatPermission = this.chatPermissionRepository.GetChatPermission(options.UserLogin, options.ChatId);
            entityChatPermission = entityChatPermission ?? mapper.Map<DataEntities.ChatPermission>(options);
            entityChatPermission.PermissionType = (int) options.PermissionType;
            entityChatPermission.ChatUser = this.userRepository.GetUser(options.UserLogin);
            entityChatPermission.Chat = this.chatRepository.GetChat(options.ChatId);
            entityChatPermission.LastUpdate = DateTime.Now;

            this.chatPermissionRepository.SaveChatPermission(entityChatPermission);
        }

        public void DeleteChatPermissions(int chatId)
        {
            var existingChatPermissions = this.chatPermissionRepository.GetChatPermissions(chatId);
            foreach (var chatPermission in existingChatPermissions)
            {
                this.chatPermissionRepository.DeleteChatPermission(chatPermission);
            }
        }

        public bool DoesUserHavePermission(string userLogin, int chatId, ChatPermissionTypes permissionType)
        {
            var chatPermission = GetChatPermission(userLogin, chatId);
            var existingChatPermissionType = (ChatPermissionTypes?) chatPermission?.PermissionType
                                             ?? ChatPermissionTypes.None;
            return Chats.DoesUserHaveChatPermission(existingChatPermissionType, permissionType);
        }
    }
}
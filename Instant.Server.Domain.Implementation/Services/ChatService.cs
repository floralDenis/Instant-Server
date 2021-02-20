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
    public class ChatService : IChatService
    {
        private readonly IChatRepository chatRepository;
        private readonly IChatPermissionService chatPermissionService;
        private readonly IChatMessageService chatMessageService;
        private readonly IUserService userService;
        private readonly IMapper mapper;

        public ChatService(
            IChatRepository chatRepository,
            IChatPermissionService chatPermissionService,
            IChatMessageService chatMessageService,
            IUserService userService,
            IMapper mapper)
        {
            this.chatRepository = chatRepository;
            this.chatPermissionService = chatPermissionService;
            this.chatMessageService = chatMessageService;
            this.userService = userService;
            this.mapper = mapper;
        }

        public bool DoesChatExist(int chatId)
        {
            var chat = chatRepository.GetChat(chatId);
            return chat != null;
        }

        public Chat CreateOrUpdateChat(CreateOrUpdateChatOptions options)
        {
            _ = options ?? throw new ArgumentNullException("Can not create or update chat, because options are null");
            
            bool isNewChat = chatRepository.GetChat(options.ChatId) == null;
            if (!chatPermissionService.DoesUserHavePermission(
                    options.InitiatorLogin,
                    options.ChatId,
                    ChatPermissionTypes.Moderate)
                && !isNewChat)
            {
                throw new ArgumentException($"Can not save chat, because initiator " +
                                            $"(user #{options.InitiatorLogin.ToString()}) does not have permissions");
            }

            var chat = isNewChat
                ? CreateChat(options)
                : UpdateChat(options);

            var domainChat = this.mapper.Map<Chat>(chat);
            return domainChat;
        }

        private DataEntities.Chat CreateChat(CreateOrUpdateChatOptions options)
        {
            var chatMemberLogins = (IList<string>) options.ExtraData;

            var chat = mapper.Map<DataEntities.Chat>(options);
            chat.LastUpdate = DateTime.Now;
            chatRepository.SaveChat(chat);
            
            var initiator = this.userService.GetUser(options.InitiatorLogin);
            var adminChatPermission = new AddOrUpdateChatPermissionOptions(
                userLogin: initiator.Login,
                chatId: chat.ChatId, 
                permissionType: ChatPermissionTypes.Administrate,
                initiatorLogin: ChatPermissionService.SYSTEM_PERMISSION,
                lastUpdate: DateTime.Now);
            this.chatPermissionService.AddOrUpdateChatPermission(adminChatPermission);
            
            foreach (var userLogin in chatMemberLogins)
            {
                var userChatPermission = new AddOrUpdateChatPermissionOptions(
                    userLogin: userLogin,
                    chatId: chat.ChatId,
                    permissionType: chat.ChatType == (int) ChatTypes.PrivateChat
                        ? ChatPermissionTypes.Administrate
                        : ChatPermissionTypes.ReadWrite,
                    initiatorLogin: initiator.Login,
                    lastUpdate: DateTime.Now);
                this.chatPermissionService.AddOrUpdateChatPermission(userChatPermission);
            }
            
            return chat;
        }

        private DataEntities.Chat UpdateChat(CreateOrUpdateChatOptions options)
        {
            var chat = this.chatRepository.GetChat(options.ChatId);
            chat.Title = options.Title;
            chat.LastUpdate = DateTime.Now;
            this.chatRepository.SaveChat(chat);
            
            var chatMemberLogins = (IList<string>) options.ExtraData;
            var existingChatMembers = chat.ChatPermissions
                .Where(chp => chp.Chat.ChatId == chat.ChatId)
                .Select(chp => chp.ChatUser.Login)
                .ToList();
            var newChatMembers = chatMemberLogins
                .Where(nchm => !existingChatMembers.Contains(nchm));

            foreach (var userLogin in newChatMembers)
            {
                var userChatPermission = new AddOrUpdateChatPermissionOptions(
                    userLogin: userLogin,
                    chatId: chat.ChatId,
                    permissionType: chat.ChatType == (int) ChatTypes.PrivateChat
                        ? ChatPermissionTypes.Administrate
                        : ChatPermissionTypes.ReadWrite,
                    initiatorLogin: options.InitiatorLogin,
                    lastUpdate: DateTime.Now);
                this.chatPermissionService.AddOrUpdateChatPermission(userChatPermission);
            }
            
            return chat;
        }
        
        public void DeleteChat(int chatId, string initiatorLogin)
        {
            if (!this.chatPermissionService.DoesUserHavePermission(
                initiatorLogin,
                chatId,
                ChatPermissionTypes.Administrate))
            {
                throw new ArgumentException($"Can not delete chat {chatId.ToString()}, because initiator " +
                                            $"(user #{initiatorLogin}) does not have permissions");
            }
            
            var chat = this.chatRepository.GetChat(chatId);
            _ = chat ?? throw new NullReferenceException("Can not delete chat, because it is null");
            this.chatMessageService.DeleteMessages(chatId, initiatorLogin);
            this.chatRepository.DeleteChat(chat, initiatorLogin);
        }

        public bool IsChatDeleted(int chatId)
        {
            return this.chatRepository.IsChatDeleted(chatId);
        }

        public void SendMessage(ChatMessage chatMessage)
        {
            _ = chatMessage ?? throw new ArgumentNullException("Can not send message, because it is null");
            if (!this.chatPermissionService.DoesUserHavePermission(
                chatMessage.SenderLogin,
                chatMessage.ChatId,
                ChatPermissionTypes.ReadWrite))
            {
                throw new ArgumentException("Can not send message, user does not have permission for that");
            }

            chatMessage.DateSent = DateTime.Now;
            this.chatMessageService.SaveMessage(chatMessage);
        }

        public IList<User> GetAffectedUsers(string userLogin)
        {
            var affectedUsers = new List<User>();
            
            var chatsOfUser = userService.GetChatsOfUser(userLogin);
            foreach (var chat in chatsOfUser)
            {
                var membersOfChat = GetMembersOfChat(chat.ChatId);
                affectedUsers.AddRange(membersOfChat);
            }

            return affectedUsers;
        }
        
        public IList<User> GetMembersOfChat(int chatId)
        {
            var users = chatRepository.GetMembersOfChat(chatId);
            var domainUsers = mapper.Map<IList<User>>(users);
            return domainUsers;
        }

        public IList<Chat> GetChatsOfUser(string userLogin)
        {
            var chats = this.chatRepository.GetChatsOfUser(userLogin);
            var domainChats = this.mapper.Map<IList<Chat>>(chats);
            return domainChats;
        }

        public IList<ChatMessage> GetMissedMessages(string userLogin, DateTime lastOnline)
        {
            var chatsOfUser = GetChatsOfUser(userLogin);
            
            var missedMessages = new List<ChatMessage>();
            foreach (var chat in chatsOfUser)
            {
                var missedMessagesFromChat = this.chatMessageService.GetMissedMessages(chat.ChatId, lastOnline);
                missedMessages.AddRange(missedMessagesFromChat);
            }

            return missedMessages;
        }

        public IList<Chat> GetMissedChatUpdates(string userLogin, DateTime lastOnline)
        {
            var chatsOfUser = GetChatsOfUser(userLogin);

            var missedChatUpdates = chatsOfUser
                .Where(ch => ch.LastUpdate > lastOnline)
                .ToList();

            return missedChatUpdates;
        }

        public IList<ChatMessage> GetMissedDeletedMessages(string userLogin, DateTime lastOnline)
        {
            var chatsOfUser = GetChatsOfUser(userLogin);

            var missedDeletedMessages = new List<ChatMessage>();
            foreach (var chat in chatsOfUser)
            {
                if (!IsChatDeleted(chat.ChatId))
                {
                    var missedDeletedMessagesFromChat = this.chatMessageService.GetMissedDeletedMessages(chat.ChatId, lastOnline);
                    missedDeletedMessages.AddRange(missedDeletedMessages);
                }
            }

            return missedDeletedMessages;
        }

        public IList<Chat> GetMissedDeletedChats(string userLogin, DateTime lastOnline)
        {
            var chatsOfUser = GetChatsOfUser(userLogin);
            var chatDeletionsOfUser = this.chatRepository.GetChatDeletions(userLogin)
                .Where(chd => chd.DateDeleted > lastOnline);
            var missedDeletedChats = chatsOfUser
                .Where(ch => chatDeletionsOfUser
                    .Any(chd => chd.Chat.ChatId == ch.ChatId))
                .ToList();

            var domainMissedDeletedChats = this.mapper.Map<IList<Chat>>(missedDeletedChats);
            return domainMissedDeletedChats;
        }
    }
}
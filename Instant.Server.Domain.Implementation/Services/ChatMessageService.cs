using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Instant.Server.Domain.Enums;
using Instant.Server.Domain.Repositories;
using Instant.Server.Domain.Services;
using ChatMessage = Instant.Server.Domain.Models.ChatMessage;

namespace Instant.Server.Domain.Implementation.Services
{
    public class ChatMessageService : IChatMessageService
    {
        private readonly IChatMessageRepository chatMessageRepository;
        private readonly IChatRepository chatRepository;
        private readonly IUserRepository userRepository;
        private readonly IChatPermissionService chatPermissionService;
        private readonly IMapper mapper;

        public ChatMessageService(
            IChatMessageRepository chatMessageRepository,
            IChatRepository chatRepository,
            IUserRepository userRepository,
            IChatPermissionService chatPermissionService,
            IMapper mapper)
        {
            this.chatMessageRepository = chatMessageRepository;
            this.chatRepository = chatRepository;
            this.userRepository = userRepository;
            this.chatPermissionService = chatPermissionService;
            this.mapper = mapper;
        }

        public IList<ChatMessage> GetMissedMessages(int chatId, DateTime lastOnline)
        {
            var messages = this.chatMessageRepository.GetMessages(chatId);
            var missedMessages = messages
                .Where(m => m.DateSent >= lastOnline)
                .ToList();

            var domainMissedMessages = mapper.Map<IList<ChatMessage>>(missedMessages);
            return domainMissedMessages;
        }

        public void SaveMessage(ChatMessage chatMessage)
        {
            var entityMessage = this.mapper.Map<Data.Entities.ChatMessage>(chatMessage);
            entityMessage.Chat = this.chatRepository.GetChat(chatMessage.ChatId);
            entityMessage.Sender = this.userRepository.GetUser(chatMessage.SenderLogin);
            this.chatMessageRepository.SaveMessage(entityMessage);
        }

        public IList<ChatMessage> GetMissedDeletedMessages(int chatId, DateTime lastOnline)
        {
            var chatMessageDeletionsInChat = this.chatMessageRepository.GetChatMessageDeletions(chatId);
            var missedChatMessageDeletions = chatMessageDeletionsInChat
                .Where(chmd => chmd.DateDeleted > lastOnline)
                .ToList();
            var messages = this.chatMessageRepository.GetMessages(chatId);
            var missedDeletedMessages = messages
                .Where(chm => missedChatMessageDeletions
                    .Any(mchmd => chm.ChatMessageId == mchmd.ChatMessageId))
                .ToList();

            var domainMissedDeletedMessages = this.mapper.Map<IList<ChatMessage>>(missedDeletedMessages);
            return domainMissedDeletedMessages;
        }

        public void DeleteMessage(int chatMessageId, string initiatorLogin)
        {
            int chatId = GetChatIdByMessageId(chatMessageId);
            if (!this.chatPermissionService.DoesUserHavePermission(
                initiatorLogin,
                chatId,
                ChatPermissionTypes.Administrate))
            {
                throw new ArgumentException($"Can not delete message {chatMessageId.ToString()}, because initiator " +
                                            $"(user #{initiatorLogin}) does not have permissions");
            }

            var chatMessage = this.chatMessageRepository.GetMessage(chatMessageId);
            this.chatMessageRepository.DeleteMessage(chatMessage, initiatorLogin);
        }

        public void DeleteMessages(int chatId, string initiatorLogin)
        {
            var existingChatMessages = this.chatMessageRepository.GetMessages(chatId);
            foreach (var chatMessage in existingChatMessages)
            {
                this.chatMessageRepository.DeleteMessage(chatMessage, initiatorLogin);
            }
        }

        public int GetChatIdByMessageId(int chatMessageId)
        {
            var chatMessage = this.chatMessageRepository.GetMessage(chatMessageId);
            return chatMessage.Chat.ChatId;
        }
    }
}
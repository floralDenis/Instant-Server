using System;
using System.Collections.Generic;
using System.Linq;
using Instant.Server.Data;
using Instant.Server.Data.Entities;
using Instant.Server.Domain.Repositories;

namespace Instant.Server.Domain.Implementation.Repositories
{
    public class ChatMessageRepository : IChatMessageRepository
    {
        private readonly InstantServerDbContext dbContext;

        public ChatMessageRepository(InstantServerDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IList<ChatMessage> GetMessages(int chatId)
        {
            var messagesInChat = this.dbContext.ChatMessages
                .Where(m => m.Chat.ChatId == chatId)
                .ToList();

            return messagesInChat;
        }

        public ChatMessage GetMessage(int chatMessageId)
        {
            var chatMessage = this.dbContext.ChatMessages.Find(chatMessageId);
            return chatMessage;
        }

        public void SaveMessage(ChatMessage chatMessage)
        {
            this.dbContext.ChatMessages.Add(chatMessage);
            this.dbContext.SaveChanges();
        }

        public IList<ChatMessageDeletion> GetChatMessageDeletions(int chatId)
        {
            var chatMessageDeletions = this.dbContext.ChatMessageDeletions
                .Where(chd => chd.ChatMessage.Chat.ChatId == chatId)
                .ToList();
            return chatMessageDeletions;
        }

        public void DeleteMessage(ChatMessage chatMessage, string initiatorLogin)
        {
            var initiator = this.dbContext.Users.Find(initiatorLogin);
            
            var chatMessageDeletion = new ChatMessageDeletion();
            chatMessageDeletion.ChatMessageId = chatMessage.ChatMessageId;
            chatMessageDeletion.ChatMessage = chatMessage;
            chatMessageDeletion.Initiator = initiator;
            chatMessageDeletion.DateDeleted = DateTime.Now;
            this.dbContext.ChatMessageDeletions.Add(chatMessageDeletion);
            this.dbContext.SaveChanges();
        }
    }
}
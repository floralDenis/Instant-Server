using System;

namespace Instant.Server.Domain.OperationOptions
{
    public class CreateOrUpdateChatOptions
    {
        public int ChatId { get; }
        public int ChatType { get; set; }
        public string Title { get; }
        public string InitiatorLogin { get; }
        public DateTime LastUpdate{ get; }
        public object ExtraData { get; }

        public CreateOrUpdateChatOptions(
            int chatId,
            int chatType,
            string title,
            string initiatorLogin,
            DateTime lastUpdate,
            object extraData = null)
        {
            this.ChatId = chatId;
            this.ChatType = chatType;
            this.Title = title;
            this.InitiatorLogin = initiatorLogin;
            this.LastUpdate = lastUpdate;
            this.ExtraData = extraData;
        }
    }
}
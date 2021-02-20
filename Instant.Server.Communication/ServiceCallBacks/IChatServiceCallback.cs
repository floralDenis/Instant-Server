using System.ServiceModel;
using Instant.Server.Communication.DataContracts;

namespace Instant.Server.Communication.ServiceCallBacks
{
    public interface IChatServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void ReceiveMessage(SendMessageOptions sendMessageOptions);

        [OperationContract(IsOneWay = true)]
        void RemoveMessage(int chatMessageId);

        [OperationContract(IsOneWay = true)]
        void AddOrUpdateChat(CreateOrUpdateChatOptions options);
        
        [OperationContract(IsOneWay = true)]
        void RemoveChat(int chatId);

        [OperationContract(IsOneWay = true)]
        void UpdateChatPermission(AddOrUpdateChatPermissionOptions options);
    }
}
using System.ServiceModel;
using Instant.Server.Communication.DataContracts;
using Instant.Server.Communication.ServiceCallBacks;

namespace Instant.Server.Communication.ServiceContracts
{
    [ServiceContract(SessionMode = SessionMode.Required,
        CallbackContract = typeof(IChatServiceCallback))]
    public interface IChatService
    {
        [OperationContract]
        OperationResult SignUp(AuthorizeUserOptions options);

        [OperationContract]
        OperationResult DeleteAccount(AuthorizeUserOptions options);

        [OperationContract(IsInitiating = true)]
        OperationResult SignIn(AuthorizeUserOptions options);

        [OperationContract(IsTerminating = true, IsOneWay = true)]
        void Disconnect(AuthorizeUserOptions options);

        [OperationContract(IsOneWay = false)]
        OperationResult GetUserData(string userLogin);
        
        [OperationContract(IsOneWay = false)]
        OperationResult CreateOrUpdateChat(CreateOrUpdateChatOptions options);

        [OperationContract(IsOneWay = false)]
        OperationResult DeleteChat(DeleteChatOptions options);
        
        [OperationContract(IsOneWay = false)]
        OperationResult SendMessage(SendMessageOptions options);

        [OperationContract(IsOneWay = false)]
        OperationResult DeleteMessage(DeleteMessageOptions options);
        
        [OperationContract(IsOneWay = false)]
        OperationResult UpdateUserPermissionInChat(AddOrUpdateChatPermissionOptions options);
    }
}
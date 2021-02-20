using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using AutoMapper;
using Instant.Server.Communication.DataContracts;
using Instant.Server.Communication.DataContracts.Enums;
using Instant.Server.Communication.ServiceCallBacks;
using Instant.Server.Communication.ServiceContracts;
using DomainOptions = Instant.Server.Domain.OperationOptions;
using DomainModels = Instant.Server.Domain.Models;
using DomainServices = Instant.Server.Domain.Services;

namespace Instant.Server.Communication.ServiceContractsImplementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession,
        ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ChatService : IChatService
    {
        private readonly Dictionary<string, IChatServiceCallback> chatServiceCallbacks;
        private IChatServiceCallback CurrentSessionChatServiceCallback
            => OperationContext.Current
                ?.GetCallbackChannel<IChatServiceCallback>();

        private readonly DomainServices.IChatService chatService;
        private readonly DomainServices.IChatPermissionService chatPermissionService;
        private readonly DomainServices.IUserService userService;
        private readonly DomainServices.IChatMessageService chatMessageService;
        private readonly IMapper mapper;

        private readonly object syncObject;
        
        public ChatService(
            DomainServices.IChatService chatService,
            DomainServices.IChatPermissionService chatPermissionService,
            DomainServices.IUserService userService,
            DomainServices.IChatMessageService chatMessageService,
            IMapper mapper)
        {
            this.chatService = chatService;
            this.chatPermissionService = chatPermissionService;
            this.userService = userService;
            this.chatMessageService = chatMessageService;
            this.mapper = mapper;

            this.chatServiceCallbacks = new Dictionary<string, IChatServiceCallback>();
            this.syncObject = new object();
        }

        public OperationResult SignUp(AuthorizeUserOptions options)
        {
            OperationResult operationResult;

            try
            {
                var domainOptions = mapper.Map<DomainOptions.AuthorizeUserOptions>(options);
                this.userService.SignUp(domainOptions);

                operationResult = new OperationResult(OperationResultTypes.Success);
            }
            catch (Exception exception)
            {
                operationResult = new OperationResult(
                    operationResultType: OperationResultTypes.Failure,
                    message: exception.ToString());
            }

            return operationResult;
        }

        public OperationResult DeleteAccount(AuthorizeUserOptions options)
        {
            OperationResult operationResult;

            try
            {
                var domainOptions = mapper.Map<DomainOptions.AuthorizeUserOptions>(options);
                userService.DeleteAccount(domainOptions);

                operationResult = new OperationResult(OperationResultTypes.Success);
            }
            catch (Exception exception)
            {
                operationResult = new OperationResult(
                    operationResultType: OperationResultTypes.Failure,
                    message: exception.ToString());
            }

            return operationResult;
        }

        public OperationResult SignIn(AuthorizeUserOptions options)
        {
            OperationResult operationResult;
            
            try
            {
                var domainOptions = mapper.Map<DomainOptions.AuthorizeUserOptions>(options);
                var domainUser = this.userService.SignIn(domainOptions);

                var callback = this.CurrentSessionChatServiceCallback;
                if (this.chatServiceCallbacks.ContainsValue(callback))
                {
                    throw new ArgumentException($"Can not connect client (user #{domainUser.Login}). Client is already connected");
                }

                lock (syncObject)
                {
                    this.chatServiceCallbacks.Add(domainUser.Login, callback);
                }

                var domainMissedChatUpdates = this.chatService.GetMissedChatUpdates(domainUser.Login, options.LastOnline);
                var missedChatUpdatesOptions = this.mapper.Map<IList<CreateOrUpdateChatOptions>>(domainMissedChatUpdates);
                foreach (var missedChatUpdatesOption in missedChatUpdatesOptions)
                {
                    var domainChatMembers = this.chatService.GetMembersOfChat(missedChatUpdatesOption.ChatId);
                    missedChatUpdatesOption.MembersLogins = domainChatMembers
                        .Select(chm => chm.Login)
                        .ToList();
                    CreateOrUpdateChatForClient(domainUser.Login, missedChatUpdatesOption);
                }
                
                var domainMissedMessages = this.chatService.GetMissedMessages(domainUser.Login, options.LastOnline);
                var missedMessages = mapper.Map<IList<SendMessageOptions>>(domainMissedMessages);
                foreach (var message in missedMessages)
                {
                    callback.ReceiveMessage(message);
                }

                var domainMissedDeletedMessages = this.chatService.GetMissedDeletedMessages(domainUser.Login, options.LastOnline);
                foreach (var message in domainMissedDeletedMessages)
                {
                    callback.RemoveMessage(message.MessageId);
                }

                var domainMissedDeletedChats = this.chatService.GetMissedDeletedChats(domainUser.Login, options.LastOnline);
                foreach (var chat in domainMissedDeletedChats)
                {
                    callback.RemoveChat(chat.ChatId);
                }

                var domainMissedChatPermissions =
                    this.chatPermissionService.GetMissedChatPermissions(options.Login, options.LastOnline);
                var chatPermissionOptions =
                    this.mapper.Map<IList<AddOrUpdateChatPermissionOptions>>(domainMissedChatPermissions);
                foreach (var chatPermission in chatPermissionOptions)
                {
                    callback.UpdateChatPermission(chatPermission);
                }
                
                var userData = mapper.Map<User>(domainUser);
                operationResult = new OperationResult(OperationResultTypes.Success, extraData: userData);
            }
            catch (Exception exception)
            {
                operationResult = new OperationResult(
                    operationResultType: OperationResultTypes.Failure,
                    message: exception.ToString());
            }

            return operationResult;
        }

        public void Disconnect(AuthorizeUserOptions options)
        {
            try
            {
                var domainOptions = mapper.Map<DomainOptions.AuthorizeUserOptions>(options);
                var domainUser = this.userService.SignIn(domainOptions);

                this.chatServiceCallbacks.Remove(domainUser.Login);

                // var affectedUsers = this.chatService.GetAffectedUsers(domainUser.Login);
                // foreach (var user in affectedUsers)
                // {
                //     if (this.chatServiceCallbacks.ContainsKey(user.Login))
                //     {
                //         var userChatServiceCallback = this.chatServiceCallbacks[user.Login];
                //     }
                // }
            }
            catch (Exception exception)
            {
                // Exception logging
            }
        }

        public OperationResult GetUserData(string userLogin)
        {
            OperationResult operationResult;
            
            try
            {
                var domainUser = this.userService.GetUser(userLogin);
                var user = this.mapper.Map<User>(domainUser);
                operationResult = new OperationResult(
                    operationResultType: OperationResultTypes.Success,
                    extraData: user);
            }
            catch (Exception exception)
            {
                operationResult = new OperationResult(
                    operationResultType: OperationResultTypes.Failure,
                    message: exception.ToString());
            }

            return operationResult;
        }

        public OperationResult CreateOrUpdateChat(CreateOrUpdateChatOptions options)
        {
            OperationResult operationResult;
            
            try
            {
                var domainOptions = mapper.Map<DomainOptions.CreateOrUpdateChatOptions>(options);
                var domainChat = this.chatService.CreateOrUpdateChat(domainOptions);
                var resultOptions = this.mapper.Map<CreateOrUpdateChatOptions>(domainChat);
                resultOptions.MembersLogins = options.MembersLogins;
                if (!resultOptions.MembersLogins.Contains(options.InitiatorLogin))
                {
                    resultOptions.MembersLogins.Add(options.InitiatorLogin);
                }

                foreach (var userLogin in resultOptions.MembersLogins)
                {
                    var chatMember = this.userService.GetUser(userLogin);
                    CreateOrUpdateChatForClient(
                        chatMember.Login,
                        resultOptions);
                }
                
                operationResult = new OperationResult(OperationResultTypes.Success);
            }
            catch (Exception exception)
            {
                operationResult = new OperationResult(
                    operationResultType: OperationResultTypes.Failure,
                    message: exception.ToString());
            }

            return operationResult;
        }

        private void CreateOrUpdateChatForClient(
            string userLogin,
            CreateOrUpdateChatOptions options)
        {
            try
            {
                if (this.chatServiceCallbacks.ContainsKey(userLogin))
                {
                    var domainChatPermission = this.chatPermissionService.GetChatPermission(userLogin, options.ChatId);
                    var chatPermissionOptions = this.mapper.Map<AddOrUpdateChatPermissionOptions>(domainChatPermission);
                    options.ExtraData = chatPermissionOptions;

                    var usersChatServiceCallback = this.chatServiceCallbacks[userLogin];
                    usersChatServiceCallback.AddOrUpdateChat(options);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public OperationResult DeleteChat(DeleteChatOptions options)
        {
            OperationResult operationResult;
            
            try
            {
                var chatMembers = this.chatService.GetMembersOfChat(options.ChatId);
                this.chatService.DeleteChat(options.ChatId, options.InitiatorLogin);
                operationResult = new OperationResult(OperationResultTypes.Success);

                foreach (var chatMember in chatMembers)
                {
                    if (this.chatServiceCallbacks.ContainsKey(chatMember.Login))
                    {
                        var userChatServiceCallback = this.chatServiceCallbacks[chatMember.Login];
                        userChatServiceCallback.RemoveChat(options.ChatId);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                operationResult = new OperationResult(
                    operationResultType: OperationResultTypes.Failure,
                    message: exception.ToString());
            }

            return operationResult;
        }

        public OperationResult SendMessage(SendMessageOptions sendMessageOptions)
        {
            OperationResult operationResult;
            
            try
            {
                var domainMessage = mapper.Map<DomainModels.ChatMessage>(sendMessageOptions);
                this.chatService.SendMessage(domainMessage);
                
                var chatMembers = this.chatService.GetMembersOfChat(sendMessageOptions.ChatId);
                foreach (var member in chatMembers)
                {
                    try
                    {
                        if (this.chatServiceCallbacks.ContainsKey(member.Login))
                        {
                            var userChatServiceCallback = this.chatServiceCallbacks[member.Login];
                            userChatServiceCallback.ReceiveMessage(sendMessageOptions);
                        }
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                    }
                }
                
                operationResult = new OperationResult(OperationResultTypes.Success);
            }
            catch (Exception exception)
            {
                operationResult = new OperationResult(
                    operationResultType: OperationResultTypes.Failure,
                    message: exception.ToString());
            }

            return operationResult;
        }

        public OperationResult DeleteMessage(DeleteMessageOptions options)
        {
            OperationResult operationResult;
            
            try
            {
                int chatId = this.chatMessageService.GetChatIdByMessageId(options.MessageId);
                var chatMembers = this.chatService.GetMembersOfChat(chatId);
                this.chatMessageService.DeleteMessage(options.MessageId, options.InitiatorLogin);
                operationResult = new OperationResult(OperationResultTypes.Success);

                foreach (var chatMember in chatMembers)
                {
                    var userChatServiceCallback = this.chatServiceCallbacks[chatMember.Login];
                    userChatServiceCallback.RemoveMessage(options.MessageId);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                operationResult = new OperationResult(
                    operationResultType: OperationResultTypes.Failure,
                    message: exception.ToString());
            }

            return operationResult;
        }

        public OperationResult UpdateUserPermissionInChat(AddOrUpdateChatPermissionOptions options)
        {
            OperationResult operationResult;

            try
            {
                var domainOptions = mapper.Map<DomainOptions.AddOrUpdateChatPermissionOptions>(options);
                this.chatPermissionService.AddOrUpdateChatPermission(domainOptions);

                if (this.chatServiceCallbacks.ContainsKey(options.ChatMemberLogin))
                {
                    var domainResultPermission = this.chatPermissionService.GetChatPermission(
                        options.ChatMemberLogin,
                        options.ChatId);
                    var resultPermissionOptions = this.mapper.Map<AddOrUpdateChatPermissionOptions>(domainResultPermission);
                    var userChatServiceCallback = this.chatServiceCallbacks[options.ChatMemberLogin];
                    userChatServiceCallback.UpdateChatPermission(resultPermissionOptions);
                    if (options.PermissionType == ChatPermissionTypes.Banned
                        || options.PermissionType == ChatPermissionTypes.None)
                    {
                        userChatServiceCallback.RemoveChat(domainOptions.ChatId);
                    }
                }
                
                operationResult = new OperationResult(OperationResultTypes.Success);
            }
            catch (Exception exception)
            {
                operationResult = new OperationResult(
                    operationResultType: OperationResultTypes.Failure,
                    message: exception.ToString());
            }
            
            return operationResult;
        }
    }
}

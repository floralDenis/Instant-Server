using AutoMapper;
using Instant.Server.Communication.DataContracts;
using DomainModels = Instant.Server.Domain.Models;
using DomainOptions = Instant.Server.Domain.OperationOptions;
using DomainEnums = Instant.Server.Domain.Enums;
using User = Instant.Server.Communication.DataContracts.User;

namespace Instant.Server.Hosting.Mapping
{
    public class CommunicationDomainProfile : Profile
    {
        public CommunicationDomainProfile()
        {
            CreateModelsMapping();
            CreateOptionsMapping();
        }

        private void CreateModelsMapping()
        {
            CreateMap<User, DomainModels.User>().ReverseMap();

            CreateMap<SendMessageOptions, DomainModels.ChatMessage>()
                .ConstructUsing(message => new DomainModels.ChatMessage(
                    message.MessageId,
                    message.ChatId,
                    message.SenderLogin,
                    message.Text,
                    message.DateSent))
                .ReverseMap();

            CreateMap<DomainModels.ChatPermission, AddOrUpdateChatPermissionOptions>();
        }

        private void CreateOptionsMapping()
        {
            CreateMap<CreateOrUpdateChatOptions, DomainOptions.CreateOrUpdateChatOptions>()
                .ConstructUsing(options => new DomainOptions.CreateOrUpdateChatOptions(
                    options.ChatId,
                    (int) options.ChatType,
                    options.Title,
                    options.InitiatorLogin,
                    options.LastUpdate,
                    options.MembersLogins));

            CreateMap<AddOrUpdateChatPermissionOptions, DomainOptions.AddOrUpdateChatPermissionOptions>()
                .ConstructUsing(options => new DomainOptions.AddOrUpdateChatPermissionOptions(
                    options.ChatPermissionId,
                    options.ChatMemberLogin,
                    options.ChatId,
                    (DomainEnums.ChatPermissionTypes)(int) options.PermissionType,
                    options.InitiatorLogin,
                    options.Date));
            CreateMap<AddOrUpdateChatPermissionOptions, DomainModels.ChatPermission>().ReverseMap();

            CreateMap<AuthorizeUserOptions, DomainOptions.AuthorizeUserOptions>()
                .ConstructUsing(options => new DomainOptions.AuthorizeUserOptions(
                    options.Login,
                    options.Password));
        }
    }
}
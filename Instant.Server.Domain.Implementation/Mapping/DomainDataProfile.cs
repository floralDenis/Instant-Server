using AutoMapper;
using Instant.Server.Domain.Models;
using DataContracts = Instant.Server.Communication.DataContracts;
using AddOrUpdateChatPermissionOptions = Instant.Server.Domain.OperationOptions.AddOrUpdateChatPermissionOptions;
using AuthorizeUserOptions = Instant.Server.Domain.OperationOptions.AuthorizeUserOptions;
using CreateOrUpdateChatOptions = Instant.Server.Domain.OperationOptions.CreateOrUpdateChatOptions;
using DataEntities = Instant.Server.Data.Entities;
using User = Instant.Server.Domain.Models.User;

namespace Instant.Server.Domain.Implementation.Mapping
{
    public class DomainDataProfile : Profile
    {
        public DomainDataProfile()
        {
            CreateModelEntitiesMapping();
        }
        
        private void CreateModelEntitiesMapping()
        {
            CreateMap<AuthorizeUserOptions, DataEntities.User>()
                .ForMember(dst => dst.Login, options => options.MapFrom(src => src.Login))
                .ForMember(dst => dst.Password, options => options.MapFrom(src => src.Password));
            CreateMap<AuthorizeUserOptions, User>()
                .ForMember(dst => dst.Login, options => options.MapFrom(src => src.Login));
            CreateMap<AuthorizeUserOptions, DataEntities.User>()
                .ForMember(dst => dst.Login, options => options.MapFrom(src => src.Login))
                .ForMember(dst => dst.Password, options => options.MapFrom(src => src.Password));
            CreateMap<User, DataEntities.User>().ReverseMap();
            
            CreateMap<AddOrUpdateChatPermissionOptions, DataEntities.ChatPermission>()
                .ForMember(dst => dst.ChatPermissionId, options => options.MapFrom(src => src.PermissionId))
                .ForMember(dst => dst.PermissionType, options => options.MapFrom(src => src.PermissionType))
                .ForMember(dst => dst.LastUpdate, options => options.MapFrom(src => src.LastUpdate));
            CreateMap<DataEntities.ChatPermission, ChatPermission>()
                .ForMember(dst => dst.ChatPermissionId, options => options.MapFrom(src => src.ChatPermissionId))
                .ForMember(dst => dst.ChatId, options => options.MapFrom(src => src.Chat.ChatId))
                .ForMember(dst => dst.ChatMemberLogin, options => options.MapFrom(src => src.ChatUser.Login))
                .ForMember(dst => dst.PermissionType, options => options.MapFrom(src => src.PermissionType))
                .ForMember(dst => dst.LastUpdate, options => options.MapFrom(src => src.LastUpdate));
            
            CreateMap<CreateOrUpdateChatOptions, DataEntities.Chat>();
            CreateMap<DataContracts.CreateOrUpdateChatOptions, Chat>().ReverseMap();
            CreateMap<DataEntities.Chat, Chat>().ReverseMap();

            CreateMap<ChatMessage, DataEntities.ChatMessage>()
                .ReverseMap()
                .ConstructUsing(chatMessage => new ChatMessage(
                    chatMessage.ChatMessageId,
                    chatMessage.Chat.ChatId,
                    chatMessage.Sender.Login,
                    chatMessage.Text,
                    chatMessage.DateSent));
        }   
    }
}
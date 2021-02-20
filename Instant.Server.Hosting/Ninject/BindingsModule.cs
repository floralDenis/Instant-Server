using AutoMapper;
using Instant.Server.Data;
using Instant.Server.Domain.Implementation.Mapping;
using Instant.Server.Domain.Implementation.Repositories;
using Instant.Server.Domain.Implementation.Services;
using Instant.Server.Domain.Repositories;
using Instant.Server.Domain.Services;
using Instant.Server.Hosting.Mapping;
using Ninject;
using Ninject.Modules;

namespace Instant.Server.Hosting.Ninject
{
    public class BindingsModule : NinjectModule
    {
        public override void Load()
        {
            ConfigureServiceBindings();
            ConfigureRepositoryBindings();
            ConfigureBindingForAutoMapper();
        }

        private void ConfigureServiceBindings()
        {
            Bind<IChatService>().To<ChatService>();
            Bind<IUserService>().To<UserService>();
            Bind<IChatPermissionService>().To<ChatPermissionService>();
            Bind<IChatMessageService>().To<ChatMessageService>();
        }
        
        private void ConfigureRepositoryBindings()
        {
            Bind<InstantServerDbContext>().ToSelf().InSingletonScope();
            Bind<IChatRepository>().To<ChatRepository>();
            Bind<IUserRepository>().To<UserRepository>();
            Bind<IChatPermissionRepository>().To<ChatPermissionRepository>();
            Bind<IChatMessageRepository>().To<ChatMessageRepository>();
        }
        
        private void ConfigureBindingForAutoMapper()
        {
            var mapperConfiguration = GetAutoMapperConfiguration();
            Bind<MapperConfiguration>().ToConstant(mapperConfiguration).InSingletonScope();
            
            Bind<IMapper>().ToMethod(ctx =>
                new Mapper(mapperConfiguration, type => ctx.Kernel.Get(type)));
        } 
        
        private MapperConfiguration GetAutoMapperConfiguration()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CommunicationDomainProfile>();
                cfg.AddProfile<DomainDataProfile>();
            });

            return config;
        }
    }
}
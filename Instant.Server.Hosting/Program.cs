using System;
using System.Reflection;
using Instant.Server.Communication.ServiceContractsImplementation;
using Ninject;
using Ninject.Extensions.Wcf;
using Ninject.Extensions.Wcf.SelfHost;
using Ninject.Web.Common.SelfHost;

namespace Instant.Server.Hosting
{
    public static class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var chatServiceConfiguration = NinjectWcfConfiguration.Create<ChatService, NinjectServiceSelfHostFactory>();
                using (var host = new NinjectSelfHostBootstrapper(CreateKernel, chatServiceConfiguration))
                {
                    host.Start();
                    Console.ReadKey();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
        
        private static StandardKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());
            return kernel;
        }
    }
}
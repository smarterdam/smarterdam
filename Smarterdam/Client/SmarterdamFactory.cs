using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject;
using Smarterdam.Api;
using Smarterdam.DataSource;
using Smarterdam.Helpers;
using Smarterdam.Server;

namespace Smarterdam.Client
{
    public class SmarterdamFactory
    {
        private static IKernel kernel;

        static SmarterdamFactory()
        {
            kernel = new StandardKernel();

            kernel.Bind<IIntelligenceManager>().To<IntelligenceManager>();
            kernel.Bind<IDataSource>().To<EcoScadaDataSource>();
            kernel.Bind<IDataGenerator>().To<RabbitMQDataGenerator>();
            kernel.Bind<IQueryParser>().To<QueryParser>();
            kernel.Bind<IStreamServerCallback>().To<StreamServerCallback>();

            kernel.Bind<IMessageQueue>().To<SimpleMessageQueue>();

            kernel.Bind<ISmarterdamClient>().To<Starter>();
            
            kernel.Bind<ISmarterdamServer>().To<NullSmarterdamServer>();

            Context.RandomValueProvider = new RandomValueProvider();
        }

        public static ISmarterdamClient CreateClient()
        {
            return kernel.Get<ISmarterdamClient>();
        }

        public static ISmarterdamServer CreateServer()
        {
            return kernel.Get<ISmarterdamServer>();
        }
    }
}

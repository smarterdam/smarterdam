﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject;
using Smarterdam.Api;
using Smarterdam.DataSource;
using Smarterdam.Helpers;
using Smarterdam.PipelineModels;
using Smarterdam.Server;
using Smarterdam.DataAccess;

namespace Smarterdam.Client
{
    public class SmarterdamFactory
    {
        private static IKernel kernel;
        private static bool _initialized;

        public static IKernel Kernel 
        { 
            get
            {
                CheckInit();
                return kernel;
            } 
        }

        static SmarterdamFactory()
        {
            
        }

        static public void Init(Action<IKernel> postBind = null)
        {
            kernel = new StandardKernel();

            kernel.Bind<IIntelligenceManager>().To<IntelligenceManager>();
            kernel.Bind<IDataSource>().To<EcoScadaDataSource>();
            kernel.Bind<IDataGenerator>().To<RabbitMQDataGenerator>();
            kernel.Bind<IQueryParser>().To<QueryParser>().InSingletonScope();
            kernel.Bind<IModelsStarter>().To<ModelsStarter>();
            kernel.Bind<ITestStartDateProvider>().To<TestStartDateProvider>().InSingletonScope();

            kernel.Bind<IPipelineModel>().To<NaivePipelineModel>();
            kernel.Bind<IPipelineModel>().To<MeanPipelineModel>();
            //kernel.Bind<IPipelineModel>().To<NeuralNetworkPipelineModel>();

            kernel.Bind<IMessageQueue>().To<SimpleMessageQueue>();

            kernel.Bind(typeof (IRepository<>))
                  .To(typeof (MongoRepositoryAdapter<>))
                  .WithConstructorArgument("collectionName", "measurements");

            kernel.Bind<ISmarterdamClient>().To<Starter>();
            kernel.Bind<Func<ISmarterdamClient>>().ToMethod(c => (() => kernel.Get<ISmarterdamClient>()));

            kernel.Bind<ISmarterdamServer>().To<NullSmarterdamServer>();

            Context.RandomValueProvider = new RandomValueProvider();

            if (postBind != null)
            {
                postBind(kernel);
            }

            _initialized = true;
        }

        //public static ISmarterdamClient CreateClient()
        //{
        //    CheckInit();
        //    return kernel.Get<ISmarterdamClient>();
        //}

        //public static ISmarterdamServer CreateServer()
        //{
        //    CheckInit();
        //    return kernel.Get<ISmarterdamServer>();
        //}

        private static void CheckInit()
        {
            if(!_initialized) Init();
        }
    }
}

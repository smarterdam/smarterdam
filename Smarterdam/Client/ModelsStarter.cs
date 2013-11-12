using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Ninject;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Smarterdam.Api;
using Smarterdam.PipelineModels;
using Smarterdam.Pipelines;

namespace Smarterdam.Client
{
    public class ModelsStarter : IModelsStarter
    {
	    private readonly IEnumerable<IPipelineModel> models;
        private readonly IMessageQueue messageQueue;

        [Inject]
        public ModelsStarter(IEnumerable<IPipelineModel> models, IMessageQueue messageQueue)
        {
            this.messageQueue = messageQueue;
	        this.models = models;
        }

        public void StartModels(string measurementId)
        {
	        foreach (var model in models)
	        {
		        model.Start(measurementId, messageQueue);
	        }
        }
    }
}

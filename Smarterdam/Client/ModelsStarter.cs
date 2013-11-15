using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MongoRepository;
using Newtonsoft.Json.Linq;
using Ninject;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Smarterdam.Api;
using Smarterdam.Entities;
using Smarterdam.PipelineModels;
using Smarterdam.Pipelines;

namespace Smarterdam.Client
{
    public class ModelsStarter : IModelsStarter
    {
	    private readonly IEnumerable<IPipelineModel> models;
        private readonly IMessageQueue messageQueue;
        //private readonly IRepository<Measurement> measurementRepository;
        private readonly MongoRepository<Measurement> measurementRepository = new MongoRepository<Measurement>("mongodb://localhost/smarterdam", "measurements");

        [Inject]
        public ModelsStarter(IEnumerable<IPipelineModel> models, IMessageQueue messageQueue)
        {
            this.messageQueue = messageQueue;
	        this.models = models;
            //this.measurementRepository = measurementRepository;
        }

        public void StartModels(string measurementId)
        {
            measurementRepository.Delete(x => x.MeasurementId == measurementId);
            var measurement = new Measurement { MeasurementId = measurementId };
            measurementRepository.Add(measurement);

	        foreach (var model in models)
	        {
		        model.Start(measurementId, messageQueue);
	        }
        }
    }
}

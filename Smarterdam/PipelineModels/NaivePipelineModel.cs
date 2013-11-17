using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoRepository;
using Smarterdam.Client;
using Smarterdam.Entities;
using Smarterdam.Filters;
using Smarterdam.Pipelines;

namespace Smarterdam.PipelineModels
{
	public class NaivePipelineModel : IPipelineModel
	{
        private Random random = new Random();
        private readonly MongoRepository<Measurement> repository = new MongoRepository<Measurement>("mongodb://localhost/smarterdam", "measurements");

	    private string name;
	    public string Name
	    {
            get { return name; }
	    }

        public NaivePipelineModel()
		{
            this.name = this.GetType().ToString();
		}

		public void Start(string id, IMessageQueue queue)
		{
			var suffix = Guid.NewGuid().ToString();

		    var pipeline = new StreamPipeline();
            
            pipeline.Register(new NaivePredictionFilter());
            pipeline.Register(new ResultOutputFilter(repository) { MeasurementId = id, ForecastModelId = name });

            var task1 = Task.Factory.StartNew(() => { RegisterStream(queue, pipeline, id + "-online-" + suffix, id); });
			

			Task.WaitAll(task1);
		}

		private void RegisterStream(IMessageQueue messageQueue, StreamPipeline pipeline, string pipelineName, string id)
		{
			while (true)
			{
				try
				{
					var units = messageQueue.Dequeue(Int32.Parse(id), pipelineName);
					pipeline.Execute(units);
				}
				catch (EndOfStreamException)
				{
					Console.WriteLine("End of stream");
					break;
				}
			}
		}
	}
}

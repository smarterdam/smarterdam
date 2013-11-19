using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Smarterdam.Client;
using Smarterdam.DataAccess;
using Smarterdam.Entities;
using Smarterdam.Filters;
using Smarterdam.Log;
using Smarterdam.Pipelines;

namespace Smarterdam.PipelineModels
{
	public class NaivePipelineModel : IPipelineModel
	{
        private readonly IRepository<Measurement> repository;

	    private string name;
	    public string Name
	    {
            get { return name; }
	    }

        public NaivePipelineModel(IRepository<Measurement> repository)
		{
            this.name = this.GetType().ToString();
            this.repository = repository;
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
					var units = messageQueue.Dequeue(id, pipelineName);
					pipeline.Execute(units);
				}
				catch (EndOfStreamException)
				{
					Console.WriteLine("End of stream");
					break;
				}
				catch (Exception ex)
				{
					Logging.Debug(ex.ToString());
				}
			}
		}
	}
}

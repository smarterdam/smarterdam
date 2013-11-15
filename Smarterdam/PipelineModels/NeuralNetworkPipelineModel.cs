using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Smarterdam.Client;
using Smarterdam.Pipelines;

namespace Smarterdam.PipelineModels
{
	public class NeuralNetworkPipelineModel : IPipelineModel
	{
        private Random random = new Random();

	    private string name;
	    public string Name
	    {
            get { return name; }
	    }
		private readonly IIntelligenceManager intelligenceManager;

		public NeuralNetworkPipelineModel(IIntelligenceManager intelligenceManager)
		{
			this.intelligenceManager = intelligenceManager;
            this.name = this.GetType().ToString() + random.Next();
		}

		public void Start(string id, IMessageQueue queue)
		{
			var suffix = Guid.NewGuid().ToString();

			PipelinePack pipelines = intelligenceManager.ComposePipeline(id, Name);
            
			var task1 = Task.Factory.StartNew(() => { RegisterStream(queue, pipelines.OfflinePipeline, id + "-offline-" + suffix, id); });
			var task2 = Task.Factory.StartNew(() => { RegisterStream(queue, pipelines.OnlinePipeline, id + "-online-" + suffix, id); });
			var task3 = Task.Factory.StartNew(() => { RegisterStream(queue, pipelines.DatabasePipeline, id + "-database-" + suffix, id); });

			Task.WaitAll(task1, task2, task3);
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

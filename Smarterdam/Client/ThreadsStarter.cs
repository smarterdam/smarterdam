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
using Smarterdam.Pipelines;

namespace Smarterdam.Client
{
    public class ThreadsStarter : IThreadsStarter
    {
        PipelinePack pipelines = null;
        Queue<DataStreamUnit>[] stream;

        private readonly IIntelligenceManager manager;
        private readonly IMessageQueue messageQueue;

        [Inject]
        public ThreadsStarter(IIntelligenceManager manager, IMessageQueue messageQueue)
        {
            this.manager = manager;
            this.messageQueue = messageQueue;
        }

        public void StartThreads(Commands command, string measurementId)
        {
            stream = new Queue<DataStreamUnit>[3];
            stream[0] = new Queue<DataStreamUnit>();
            stream[1] = new Queue<DataStreamUnit>();
            stream[2] = new Queue<DataStreamUnit>();

            if (pipelines == null)
            {
                pipelines = manager.ComposePipeline(command, measurementId);
            }

            var task1 = Task.Factory.StartNew(() => { RegisterStream(pipelines.OfflinePipeline, measurementId + "-offline", measurementId); });
            var task2 = Task.Factory.StartNew(() => { RegisterStream(pipelines.OnlinePipeline, measurementId + "-online", measurementId); });
            var task3 = Task.Factory.StartNew(() => { RegisterStream(pipelines.DatabasePipeline, measurementId + "-database", measurementId); });

            Task.WaitAll(task1, task2, task3);
        }

        private void RegisterStream(StreamPipeline pipeline, string pipelineName, string id)
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

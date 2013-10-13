using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Ninject;

namespace Smarterdam.Client
{
    public class Starter : ISmarterdamClient
    {
        private IIntelligenceManager manager;
        private IQueryParser queryParser;
        private IStreamServerCallback streamServerCallback;

        [Inject]
        public Starter(IIntelligenceManager manager, IQueryParser queryParser, IStreamServerCallback streamServerCallback)
        {
            this.manager = manager;
            this.queryParser = queryParser;
            this.streamServerCallback = streamServerCallback;
        }

        public void Start(string query, string measurementId)
        {
            StartThread(query, measurementId);
            //var t = new Thread(() => { StartThread(url, query, measurementId); });
            //t.IsBackground = true;
            //t.Start();
        }

        public void StartThread(string query, string measurementId)
        {
            var command = queryParser.Parse(query);

            if (command.ListCommand[3].GetMethodName() == "SAVE")
            {
                var pipelinePack = manager.ComposePipeline(command, measurementId);
                
                streamServerCallback.StartThreads(command, measurementId);

                //Thread.Sleep(Timeout.Infinite);
            }

            if (command.ListCommand[3].GetMethodName() == "KILL")
            {
                //"УБИВАЕМ" процесс
            }
        }
    }
}

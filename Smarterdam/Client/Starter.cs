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
        private IQueryParser queryParser;
        private IStreamServerCallback streamServerCallback;

        [Inject]
        public Starter(IQueryParser queryParser, IStreamServerCallback streamServerCallback)
        {
            this.queryParser = queryParser;
            this.streamServerCallback = streamServerCallback;
        }

        public void Start(string query, string measurementId)
        {
            StartThread(query, measurementId);
        }

        private void StartThread(string query, string measurementId)
        {
            var command = queryParser.Parse(query);

            //if (command.ListCommand[3].GetMethodName() == "SAVE")
            //{
            streamServerCallback.StartThreads(command, measurementId);
            //}

            //if (command.ListCommand[3].GetMethodName() == "KILL")
            //{
            //    //"УБИВАЕМ" процесс
            //}
        }
    }
}

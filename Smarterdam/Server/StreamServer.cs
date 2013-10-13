using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smarterdam.Server
{
    //DEPRECATED
    //[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class StreamServer : IStreamServer
    {
        Dictionary<string, IDataGenerator> generators;

        public StreamServer(IEnumerable<IDataGenerator> generators)
        {
            //this.generators = generators.ToDictionary(x => x.Name, x => x);
        }

        public void Subscribe(string streamName)
        {
            if (generators.ContainsKey(streamName))
            {
                var streamGenerator = generators[streamName];
                //streamGenerator.Subscribe(OperationContext.Current.GetCallbackChannel<IGeneratorCallback>());

                Console.WriteLine(String.Format("Client subscribed to stream '{0}'", streamName));
            }
        }

        public void Unsubscribe(string streamName)
        {
            if (generators.ContainsKey(streamName))
            {
                var streamGenerator = generators[streamName];
                //streamGenerator.Unsubscribe(OperationContext.Current.GetCallbackChannel<IGeneratorCallback>());

                Console.WriteLine(String.Format("Client unsubscribed from stream '{0}'", streamName));
            }
        }

        public IEnumerable<string> GetListOfSensors()
        {
            return generators.Keys;
        }
    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Smarterdam.Api;

namespace Smarterdam.Client
{
    public class SimpleMessageQueue : IMessageQueue
    {
        private readonly IDataSource dataSource;
        private Dictionary<string, IEnumerable<DataStreamUnit>> innerExchanges = new Dictionary<string, IEnumerable<DataStreamUnit>>();
        private static object _lockObject = new Object();

        private ConcurrentDictionary<string, Queue<DataStreamUnit>> _queues = new ConcurrentDictionary<string, Queue<DataStreamUnit>>();

        public SimpleMessageQueue(IDataSource dataSource)
        {
            this.dataSource = dataSource;
            this.dataSource.SetDate(DateTime.Now.AddDays(-21)); //нужно для ретроспективных экспериментов, то есть таких, для которых будут использоваться данные о прошлом

        }

        public DataStreamUnit[] Dequeue(string measurementId, string queueId)
        {
            lock (_lockObject)
            {
                if (!innerExchanges.ContainsKey(measurementId))
                {
                    GetData(measurementId);
                }
            }

            if (!_queues.ContainsKey(queueId))
            {
                _queues.TryAdd(queueId, new Queue<DataStreamUnit>(innerExchanges[measurementId]));
            }
            
            if(!_queues[queueId].Any()) throw new EndOfStreamException();

            return new DataStreamUnit[] {_queues[queueId].Dequeue()};
        }

        private void GetData(string measurementId)
        {
            innerExchanges.Add(measurementId, dataSource.GetNewData(Int32.Parse(measurementId)));
        }
    }
}

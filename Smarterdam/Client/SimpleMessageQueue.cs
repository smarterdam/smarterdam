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
        private IEnumerable<DataStreamUnit> innerExchange = new List<DataStreamUnit>();
        private int? measurementId = null;
        private static object _lockObject = new Object();

        private ConcurrentDictionary<string, Queue<DataStreamUnit>> _queues = new ConcurrentDictionary<string, Queue<DataStreamUnit>>();

        public SimpleMessageQueue(IDataSource dataSource)
        {
            this.dataSource = dataSource;
            this.dataSource.SetDate(DateTime.Now.AddDays(-21)); //нужно для ретроспективных экспериментов, то есть таких, для которых будут использоваться данные о прошлом

        }

        public DataStreamUnit[] Dequeue(int measurementId, string queueId)
        {
            if (this.measurementId != null && this.measurementId != measurementId) throw new Exception("Недопустимая операция");

            lock (_lockObject)
            {
                if (this.measurementId != null && this.measurementId != measurementId) throw new Exception("Недопустимая операция");
                if (!innerExchange.Any())
                {
                    GetData(measurementId);
                }
            }

            if (!_queues.ContainsKey(queueId))
            {
                _queues.TryAdd(queueId, new Queue<DataStreamUnit>(innerExchange));
            }
            
            if(!_queues[queueId].Any()) throw new EndOfStreamException();

            return new DataStreamUnit[] {_queues[queueId].Dequeue()};
        }

        private void GetData(int measurementId)
        {
            innerExchange = dataSource.GetNewData(measurementId);
            this.measurementId = measurementId;
        }
    }
}

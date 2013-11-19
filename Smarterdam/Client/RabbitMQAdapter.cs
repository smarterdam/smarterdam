using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Smarterdam.Api;

namespace Smarterdam.Client
{
    public class RabbitMQAdapter : IMessageQueue, IDisposable
    {
        private readonly string brokerUrl = "localhost";
        private bool initialized;
        private static object lockObject = new object();

        private IConnection connection;
        private ConnectionFactory factory;
        
        private ConcurrentDictionary<string, IModel> channels;
        private ConcurrentDictionary<string, QueueingBasicConsumer> consumers;

        public RabbitMQAdapter()
        {
            channels = new ConcurrentDictionary<string, IModel>();
            consumers = new ConcurrentDictionary<string, QueueingBasicConsumer>();
        }

        private void Initialize()
        {
            factory = new ConnectionFactory();
            factory.HostName = brokerUrl;
            connection = factory.CreateConnection();
            initialized = true;
        }

        private void InitConsumer(string measurementId, string queueId)
        {
            var channel = connection.CreateModel();
            channels.TryAdd(queueId, channel);

            var queueName = String.Format("smarterdam-queue-{0}", queueId);
            var exchangeName = String.Format("smarterdam-exchange-{0}", measurementId);

            channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, false, false, null);
            channel.QueueDeclare(queueName, false, false, false, null);
            channel.QueueBind(queueName, exchangeName, "");

            QueueingBasicConsumer consumer = new QueueingBasicConsumer(channel);
            consumers.TryAdd(queueId, consumer);
            channel.BasicConsume(queueName, true, consumer);
        }

        public DataStreamUnit[] Dequeue(string measurementId, string queueId)
        {
            if (!initialized)
            {
                lock (lockObject)
                {
                    if (!initialized) Initialize();
                }
            }

            if (!consumers.ContainsKey(queueId)) InitConsumer(measurementId, queueId);

            var consumer = consumers[queueId];

            object item = null;
            consumer.Queue.Dequeue(15000, out item);

            if (item == null)
            {
                QueueingBasicConsumer outConsumer = null;
                consumers.TryRemove(queueId, out outConsumer);
                
                IModel channel = null;
                channels.TryRemove(queueId, out channel);

                var exchangeName = String.Format("smarterdam-exchange-{0}", measurementId);
                var queueName = String.Format("smarterdam-queue-{0}", queueId);
                channel.QueueUnbind(queueName, exchangeName, "", null);
                channel.QueueDelete(queueName);
                //channel.ExchangeDelete(exchangeName);
                channel.Close();
                
                throw new EndOfStreamException();
            }

            return ConvertToUnits((item as BasicDeliverEventArgs).Body);
        }

        private DataStreamUnit[] ConvertToUnits(byte[] source)
        {
            string message = System.Text.Encoding.UTF8.GetString(source);

            JObject unit = JObject.Parse(message);

            DataStreamUnit dataStreamUnit = new DataStreamUnit();
            var valuesDict = new Dictionary<string, object>();
            foreach (var child in unit.Children())
            {
                var prop = (child as JProperty);
                dataStreamUnit.Values[prop.Name] = prop.Value.ToString();
            }

            var unitArray = new DataStreamUnit[] { dataStreamUnit };

            return unitArray;
        }

        public void Dispose()
        {
            foreach (var channel in channels)
            {
                channel.Value.Dispose();
            }
            connection.Dispose();
        }
    }
}

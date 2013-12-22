using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Newtonsoft.Json;
using Ninject;
using RabbitMQ.Client;
using Smarterdam.Api;

namespace Smarterdam.Server
{
    public class RabbitMQDataGenerator : IDataGenerator
    {
        private IDataSource dataSource;

        [Inject]
        public RabbitMQDataGenerator(IDataSource dataSource)
        {
            this.dataSource = dataSource;
            dataSource.SetDate(DateTime.Now.AddDays(-21));
        }

        public void Start(int updatePeriodInSeconds, int id)
        {
            Task.Factory.StartNew(() =>
                {
                    ConnectionFactory factory = new ConnectionFactory();
                    factory.HostName = "localhost";
                    using (IConnection connection = factory.CreateConnection())
                    using (IModel channel = connection.CreateModel())
                    {
                        channel.ExchangeDeclare("smarterdam-exchange-" + id.ToString(), ExchangeType.Direct, false,
                                                false, null);

                        Action<object, ElapsedEventArgs> timer_Elapsed = (sender, e) =>
                            {
                                if (dataSource.HasNewData(id))
                                {
                                    var newData = dataSource.GetNewData(id);

                                    foreach (var unit in newData)
                                    {
                                        dynamic dynamicUnit = new ExpandoObject();
                                        var dynUnitDict = dynamicUnit as IDictionary<string, object>;
                                        foreach (var key in unit.Values.Keys)
                                        {
                                            dynUnitDict[key] = unit.Values[key];
                                        }
                                        dynamicUnit.TimeStamp = unit.TimeStamp;

                                        var message = JsonConvert.SerializeObject(dynamicUnit);

                                        byte[] body = System.Text.Encoding.UTF8.GetBytes(message);

                                        channel.BasicPublish("smarterdam-exchange-" + id.ToString(), "", null, body);
                                    }
                                }
                            };

                        timer_Elapsed(null, null);
                        var timer = new System.Timers.Timer(updatePeriodInSeconds*1000);
                        timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
                        timer.Enabled = true;
                        timer.Start();

                        //Thread.Sleep(Timeout.Infinite);

                    }
                });
        }
    }
}

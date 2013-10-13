//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Timers;
//using Smarterdam.Api;
//using Smarterdam.Pipelines;

//namespace Smarterdam.Server
//{
//    public class DataGenerator : IDataGenerator
//    {
//        List<IGeneratorCallback> channels;

//        private DateTime previouslyUpdated;

//        IDataSource dataSource;

//        public string Name { get; set; }

//        DataGenerator()
//        {
//            channels = new List<IGeneratorCallback>();
//            previouslyUpdated = DateTime.Now;
//        }

//        public DataGenerator(string name, IDataSource dataSource, double updatePeriodInSeconds = 30)
//            : this()
//        {
//            this.Name = name;

//            this.dataSource = dataSource;

//            var timer = new Timer(updatePeriodInSeconds * 1000);
//            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
//            timer.Enabled = true;
//            timer.Start();
//        }

//        void timer_Elapsed(object sender, ElapsedEventArgs e)
//        {
//            if (channels.Any())
//            {
//                if (dataSource.HasNewData(previouslyUpdated))
//                {
//                    var newData = dataSource.GetNewData(previouslyUpdated);

//                    channels.ForEach(x => x.Update(newData));
//                }
//            }
//        }

//        public void Subscribe(IGeneratorCallback callbackChannel)
//        {
//            channels.Add(callbackChannel);
//        }

//        public void Unsubscribe(IGeneratorCallback callbackChannel)
//        {
//            channels.Remove(callbackChannel);
//        }
//    }

//    public class NewDataEventArgs
//    {
//        public IEnumerable<DataStreamUnit> UpdateData;

//        public NewDataEventArgs(IEnumerable<DataStreamUnit> updateData)
//        {
//            this.UpdateData = updateData;
//        }
//    }
//}

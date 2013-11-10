using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smarterdam.Api;

namespace Smarterdam.DataSource
{
    public class MockDataSource : IDataSource
    {
        private Random random = new Random();
        private DateTime lastReceivedDateTime = DateTime.Now;

        public bool HasNewData(DateTime sinceWhen, int measurementId)
        {
            return true;
        }

        public bool HasNewData(int measurementId)
        {
            return true;
        }

        public IEnumerable<DataStreamUnit> GetNewData(DateTime sinceWhen, int measurementId)
        {
            var now = DateTime.Now;
            var ts = sinceWhen;

            var result = new List<DataStreamUnit>();
            
            while (ts <= now)
            {
                result.Add(GenerateUnit(ts));
                ts = ts.AddMinutes(15);
            }

            return result;
        }

        private DataStreamUnit GenerateUnit(DateTime timeStamp)
        {
            var values = new ConcurrentDictionary<string, object>();
            values["Value"] = random.NextDouble()*100000;
            values["TimeStamp"] = timeStamp;

            return new DataStreamUnit() { Values = values, TimeStamp = timeStamp };
        }

        public IEnumerable<DataStreamUnit> GetNewData(int measurementId)
        {
            return GetNewData(lastReceivedDateTime, measurementId);
        }

        public DateTime GetLastTimestamp(int measurementId)
        {
            return DateTime.Now;
        }

        public void SetDate(DateTime newDate)
        {
            lastReceivedDateTime = newDate;
        }
    }
}

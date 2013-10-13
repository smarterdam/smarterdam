using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smarterdam.Api
{
    public class DataStreamUnit
    {
        public DateTime TimeStamp { get; set; }
        public ConcurrentDictionary<string, object> Values { get; set; }

        public DataStreamUnit()
        {
            Values = new ConcurrentDictionary<string, object>();
        }

        public override string ToString()
        {
            var result = new StringBuilder();
            result.AppendLine("Sent:\t\t" + TimeStamp.TimeOfDay.ToString());

            var receivedTime = DateTime.Now;
            result.AppendLine("Received:\t" + receivedTime.TimeOfDay.ToString());
            result.AppendLine("Transfer time:\t" + (receivedTime - TimeStamp).TotalMilliseconds + " ms");
            foreach (var value in Values)
            {
                result.AppendLine("\t" + value.Key + ": " + value.Value);
            }
            result.AppendLine();

            return result.ToString();
        }
    }
}

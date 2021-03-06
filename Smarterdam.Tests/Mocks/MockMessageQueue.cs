﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smarterdam.Api;
using Smarterdam.Client;

namespace Smarterdam.Tests.Mocks
{
    public class MockMessageQueue : IMessageQueue
    {
        public DataStreamUnit[] Dequeue(string measurementId, string queueId)
        {
            var timeStamp = DateTime.Now;

            var values = new ConcurrentDictionary<string, object>();
            values["Value"] = 50000.0;
            values["TimeStamp"] = timeStamp;

            var unit = new DataStreamUnit() {TimeStamp = timeStamp, Values = values};

            return new DataStreamUnit[] { unit };
        }
    }
}

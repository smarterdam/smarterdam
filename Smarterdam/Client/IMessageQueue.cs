using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smarterdam.Api;

namespace Smarterdam.Client
{
    public interface IMessageQueue
    {
        DataStreamUnit[] Dequeue(int measurementId, string queueId);
    }
}

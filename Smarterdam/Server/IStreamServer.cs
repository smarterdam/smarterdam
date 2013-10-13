using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smarterdam.Server
{
    public interface IStreamServer
    {
        void Subscribe(string streamName);

        void Unsubscribe(string streamName);

        IEnumerable<string> GetListOfSensors();
    }
}

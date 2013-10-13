using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smarterdam.Client
{
    public interface IStreamServerCallback
    {
        void StartThreads(Commands command, string measurementId);
    }
}

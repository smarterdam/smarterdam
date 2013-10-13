using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smarterdam.Client
{
    public interface ISmarterdamClient
    {
        void Start(string query, string measurementId);
    }
}

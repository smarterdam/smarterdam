using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smarterdam.Api;
using Smarterdam.Pipelines;

namespace Smarterdam.Server
{
    public interface IGeneratorCallback
    {
        void Update(IEnumerable<DataStreamUnit> data);
    }
}

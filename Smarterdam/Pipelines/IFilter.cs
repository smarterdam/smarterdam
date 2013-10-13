using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smarterdam.Api;

namespace Smarterdam.Pipelines
{
    public interface IFilter
    {
        DataStreamUnit[] Execute(DataStreamUnit[] stream);
        void Register(IFilter nextFilter);
    }
}

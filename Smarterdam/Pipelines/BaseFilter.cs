using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smarterdam.Api;

namespace Smarterdam.Pipelines
{
    public abstract class BaseFilter : IFilter
    {
        private IFilter _nextFilter;

        protected abstract DataStreamUnit[] _Execute(DataStreamUnit[] input);

        public DataStreamUnit[] Execute(DataStreamUnit[] input)
        {
            DataStreamUnit[] retVal = _Execute(input);

            if (_nextFilter != null)
            {
                retVal = _nextFilter.Execute(retVal);
            }

            return retVal;
        }

        public void Register(IFilter nextFilter)
        {
            _nextFilter = nextFilter;
        }

        public virtual void UpdateParameters(FilterParameters parameters)
        {

        }
    }
}

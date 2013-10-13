using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smarterdam.Api;

namespace Smarterdam.Pipelines
{
    public class StreamPipeline
    {
        List<IFilter> filters;

        StreamPipeline()
        {
            filters = new List<IFilter>();
        }

        public StreamPipeline(params IFilter[] args)
            : this()
        {
            foreach (var filter in args)
            {
                Register(filter);
            }
        }

        public DataStreamUnit[] Execute(DataStreamUnit[] source)
        {
            IFilter root = null;
            IFilter previous = null;

            foreach (IFilter phrase in filters)
            {
                if (root == null)
                {
                    root = phrase;
                }
                else
                {
                    previous.Register(phrase);
                }
                previous = phrase;
            }

            var result = root == null ? source : root.Execute(source);

            return result;
        }

        public void Register(IFilter filter)
        {
            filters.Add(filter);
        }
    }
}

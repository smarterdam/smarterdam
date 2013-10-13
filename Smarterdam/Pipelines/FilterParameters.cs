using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smarterdam.Pipelines
{
    public class FilterParameters
    {
        public Dictionary<string, object> Values;

        public FilterParameters()
        {
            Values = new Dictionary<string, object>();
        }

        public object this[string key]
        {
            get
            {
                if (Values.ContainsKey(key))
                {
                    return Values[key];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                Values[key] = value;
            }
        }
    }
}

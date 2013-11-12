using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smarterdam.Filters
{
    public class DataExchange
    {
        private ConcurrentDictionary<string,object> _dictionary = new ConcurrentDictionary<string, object>();
        private object locker = new object();

        public object this[string key]
        {
            get
            {
                lock (locker)
                {
                    return _dictionary.ContainsKey(key) ? _dictionary[key] : null;
                }
            }
            set
            {
                lock (locker)
                {
                    _dictionary.AddOrUpdate(key, value, (s, o) => { return value; });
                }
            }
        }
    }
}

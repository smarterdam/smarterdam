using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smarterdam.Helpers;

namespace Smarterdam
{
    public class Context
    {
        private static IRandomValueProvider _randomValueProvider = null;
        public static IRandomValueProvider RandomValueProvider
        {
            get { return _randomValueProvider; }
            set
            {
                if (_randomValueProvider == null)
                {
                    _randomValueProvider = value;
                }
            }
        }
    }
}

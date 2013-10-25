using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smarterdam.Helpers
{
    public class MockRandomValueProvider : IRandomValueProvider
    {
        public double Next()
        {
            return 0.0;
        }
    }
}

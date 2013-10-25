using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smarterdam.Helpers
{
    public class RandomValueProvider : IRandomValueProvider
    {
        private static Random rdm = new Random();

        public double Next()
        {
            return rdm.Next();
        }
    }
}

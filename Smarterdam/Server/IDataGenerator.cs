using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smarterdam.Server
{
    public interface IDataGenerator
    {
        void Start(int updatePeriodInSeconds, int id);
    }
}

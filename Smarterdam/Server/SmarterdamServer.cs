using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smarterdam.Server
{
    public class SmarterdamServer : ISmarterdamServer
    {
        private IDataGenerator generator;

        public SmarterdamServer(IDataGenerator generator)
        {
            this.generator = generator;
        }

        public void Start(int id)
        {
            generator.Start(60*10, id);
        }
    }
}

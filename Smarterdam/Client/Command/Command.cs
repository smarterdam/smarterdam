using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smarterdam.Client
{
    public class Command
    {
        public string MethodName { get; set; }
        public Dictionary<string, string> Parameters { get; set; }

        public Command(string MethodName, Dictionary<string, string> Parameters)
        {
            this.MethodName = MethodName;
            this.Parameters = Parameters;
        }
        public Command()
        {
            this.MethodName = null;
            this.Parameters = null;
        }
        public void SetMethodName(string MethodName)
        {
            this.MethodName = MethodName;
        }
        public void SetParameters(Dictionary<string, string> Parameters)
        {
            this.Parameters = Parameters;
        }
        public string GetMethodName()
        {
            return (this.MethodName);
        }
    }
}

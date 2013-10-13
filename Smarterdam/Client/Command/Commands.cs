using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smarterdam.Client
{
    public class Commands
    {
        public List<Command> ListCommand = new List<Command>();

        public Commands()
        {
            Dictionary<string, string> dict = new Dictionary<string, string> {
            {"null", "null"},
            };
            Command db = new Command("null", dict);
            //KeyValuePair r = new KeyValuePair<string, object>("key1", 0);
            //// ListCommand.Add(db);
            //// //Command owner = new Command();
            //// ListCommand.Add(db);
            ////// Command stream = new Command();
            //// ListCommand.Add(db);
            //// //Command com1 = new Command();
            //// ListCommand.Add(db);
            ////// Command com2 = new Command();
            //// ListCommand.Add(db);
        }
    }
}

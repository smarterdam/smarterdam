﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Ninject;
using Smarterdam.Client;

namespace Smarterdam.UI
{
    public class Program
    {
        private static ISmarterdamClient starter;

        [Inject]
        public static ISmarterdamClient Starter { set { starter = value; } }

        static Program()
        {
              
        }

        //[STAThread]
        public static void Main(string[] args)
        {
            String query;

            string url = ReadUrl();

            if (args.Length > 0)
            {
                query = args[0];
            }
            else
            {
                query = ReadQuery();
            }

            while (query != "quit")
            {
                //var starter = new Starter(new IntelligenceManager(), new QueryParser(), new _threadsStarter(new IntelligenceManager()));
                starter.Start(query, "0");
                url = ReadUrl();
                query = ReadQuery();
            }
        }

        public static string ReadQuery()
        {
            Console.WriteLine("Enter query:");
            Console.ReadLine();
            return "db.ownerMax.streamA.SAVE";
        }

        public static string ReadUrl()
        {
            return "localhost";
        }
    }
}

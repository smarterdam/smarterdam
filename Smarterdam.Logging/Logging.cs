using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;

namespace Smarterdam.Log
{
    public static class Logging
    {
        private static Logger logger;
        
        static Logging()
        {
            logger = LogManager.GetLogger("smarterdam_logger");
        }

        public static void Info(string msg)
        {
            logger.Info(msg);
        }

        public static void Debug(string msg)
        {
            logger.Debug(msg);
        }

        public static void Debug(string msg, params object[] values)
        {
            logger.Debug(msg, values);
        }
    }
}

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
        private static bool DEBUG = false;

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
            if (DEBUG)
            {
                logger.Info(msg);
            }
        }
    }
}

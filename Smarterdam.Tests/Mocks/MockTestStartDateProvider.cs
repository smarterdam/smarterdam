using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smarterdam.Client;

namespace Smarterdam.Tests.Mocks
{
    public class MockTestStartDateProvider : ITestStartDateProvider
    {
        public DateTime GetTimestampOfTestStart(string measurementId)
        {
            return DateTime.Now.AddSeconds(5);
        }


        public DateTime GetTimestampOfTestStart(DateTime lastTimeStamp)
        {
            return lastTimeStamp.AddDays(-5);
        }
    }
}

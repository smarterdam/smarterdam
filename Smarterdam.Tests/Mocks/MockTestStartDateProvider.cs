using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smarterdam.Client;

namespace Smarterdam.Tests.Mocks
{
    public class MockTestStartDateProvider : ITestStartDateProvider
    {
        public DateTime GetTimestampOfTestStart(int measurementId)
        {
            return DateTime.Now.AddSeconds(5);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject;
using Smarterdam.Api;

namespace Smarterdam.Client
{
    public class TestStartDateProvider : ITestStartDateProvider
    {
        private readonly IDataSource dataSource;

        [Inject]
        public TestStartDateProvider(IDataSource dataSource)
        {
            this.dataSource = dataSource;
        }

        public DateTime GetTimestampOfTestStart(int measurementId)
        {
            var finalDate = dataSource.GetLastTimestamp(measurementId);
            return finalDate.AddDays(-3);
        }
    }
}

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

		public DateTime GetTimestampOfTestStart(string measurementId)
        {
            var finalDate = dataSource.GetLastTimestamp(Int32.Parse(measurementId));
            return finalDate.AddDays(-3);
        }
    }
}

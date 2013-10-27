using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smarterdam.Api;

namespace Smarterdam.Tests.Mocks
{
    public class MockDataSource : IDataSource
    {
        public bool HasNewData(DateTime sinceWhen, int measurementId)
        {
            throw new NotImplementedException();
        }

        public bool HasNewData(int measurementId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DataStreamUnit> GetNewData(DateTime sinceWhen, int measurementId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DataStreamUnit> GetNewData(int measurementId)
        {
            throw new NotImplementedException();
        }

        public DateTime GetLastTimestamp(int measurementId)
        {
            throw new NotImplementedException();
        }

        public void SetDate(DateTime newDate)
        {
            throw new NotImplementedException();
        }
    }
}

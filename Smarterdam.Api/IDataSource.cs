using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smarterdam.Api
{
    public interface IDataSource
    {
        bool HasNewData(DateTime sinceWhen, int measurementId);
        bool HasNewData(int measurementId);

        IEnumerable<DataStreamUnit> GetNewData(DateTime sinceWhen, int measurementId);
        IEnumerable<DataStreamUnit> GetNewData(int measurementId);

        void SetDate(DateTime newDate);
    }
}

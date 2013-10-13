using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smarterdam.Entities;

namespace Smarterdam.DataAccess
{
    public interface IForecastResultRepository : IResultRepository
    {
        void Purge(int id);
        void Add(ForecastResult result);
        IEnumerable<ForecastResult> GetAll(int measurementId);
        ForecastResult GetLast(int measurementId);
        IEnumerable<int> GetTasks();
    }
}

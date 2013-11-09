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
        Forecast Create(int id);
        void Add(int measurementId, ForecastResult result);
        Forecast Get(int measurementId);
        ForecastResult GetLast(int measurementId);
        IEnumerable<int> GetTasks();
    }
}

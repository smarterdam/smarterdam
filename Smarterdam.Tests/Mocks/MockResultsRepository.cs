using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smarterdam.DataAccess;
using Smarterdam.Entities;

namespace Smarterdam.Tests.Mocks
{
    public class MockResultsRepository : IForecastResultRepository
    {
        private List<ForecastResult> _values = new List<ForecastResult>();
        public IEnumerable<ForecastResult> Values 
        {
            get { return _values; }
        }

        public void Purge(int id)
        {
            _values = new List<ForecastResult>();
        }

        public void Add(ForecastResult result)
        {
            _values.Add(result);
        }

        public IEnumerable<Entities.ForecastResult> GetAll(int measurementId)
        {
            return _values;
        }

        public Entities.ForecastResult GetLast(int measurementId)
        {
            return _values.LastOrDefault();
        }

        public IEnumerable<int> GetTasks()
        {
            return new List<int>();
        }
    }
}

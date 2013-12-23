using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smarterdam.Models
{
    public class TimeSeries2<T>
    {
        private const int DAILY_PATTERNS = 96;
        private Dictionary<DateTime, T> _values;

        public T this[DateTime key]
        {
            get { return _values[key]; }
        }

        public TimeSeries2(Dictionary<DateTime, T> values)
        {
            this._values = values;
        }

        public IEnumerable<T> GetPeriod(DateTime from, DateTime to)
        {
            return _values.OrderBy(x => x.Key).Where(x => x.Key >= from && x.Key <= to).Select(x => x.Value);
        }

        public IEnumerable<T> GetDay(DateTime date)
        {
            var from = date.Date;
            var to = date.Date.AddDays(1).AddSeconds(-1);

            return GetPeriod(from, to);
        }
    }
}

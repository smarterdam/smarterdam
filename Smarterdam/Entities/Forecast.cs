using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smarterdam.Entities
{
    public class Forecast
    {
        public int MeasurementId { get; set; }
        public double? Error { get; set; }

        public IEnumerable<ForecastResult> Results { get; set; }

        public Forecast()
        {
            Results = new List<ForecastResult>();
        }
    }
}

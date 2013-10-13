using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smarterdam.Entities
{
    public class ForecastResult : IPipelineResult
    {
        public int MeasurementID { get; set; }

        public DateTime TimeStamp { get; set; }
        public double? RealValue { get; set; }
        public double? PredictedValue { get; set; }
        public double? Error { get; set; }
    }
}

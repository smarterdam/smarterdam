using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoRepository;

namespace Smarterdam.Entities
{
    public class ForecastResult : Entity
    {
        public DateTime TimeStamp { get; set; }
        public double? RealValue { get; set; }
        public double? PredictedValue { get; set; }
        public double? Error { get; set; }
    }
}

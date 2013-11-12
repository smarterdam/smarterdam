using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoRepository;

namespace Smarterdam.Entities
{
    public class Forecast : Entity
    {
		public string ForecastModelId { get; set; }
        public double? Error { get; set; }

        public List<ForecastResult> Results { get; set; }

        public Forecast()
        {
            Results = new List<ForecastResult>();
        }
    }
}

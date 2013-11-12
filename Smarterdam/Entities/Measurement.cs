using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoRepository;

namespace Smarterdam.Entities
{
	public class Measurement : Entity
	{
		public string MeasurementId { get; set; }
		public List<Forecast> Forecasts { get; set; }

		public Measurement()
		{
			Forecasts = new List<Forecast>();
		}
	}
}

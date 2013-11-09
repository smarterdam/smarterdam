using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using Smarterdam.Entities;

namespace Smarterdam.DataAccess.DbEntities
{
    public class DbForecast
    {
        public ObjectId Id { get; set; }

        public int MeasurementId { get; set; }

        public double? Error { get; set; }

        public List<DbForecastResult> Results { get; set; }

        public DbForecast(Forecast source)
        {
            this.Results = source.Results.Select(x => new DbForecastResult(x)).ToList();
            this.MeasurementId = source.MeasurementId;
            this.Error = source.Error;
        }

        public Forecast ConvertBack()
        {
            var result = new Forecast();
            
            result.Error = Error;
            result.Results = this.Results.Select(x => x.ConvertBack());

            return result;
        }
    }
}

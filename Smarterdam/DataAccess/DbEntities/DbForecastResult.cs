using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using Smarterdam.Entities;

namespace Smarterdam.DataAccess.DbEntities
{
    public class DbForecastResult
    {
        public ObjectId Id { get; set; }

        public int MeasurementID { get; set; }
        public DateTime TimeStamp { get; set; }
        public double? RealValue { get; set; }
        public double? PredictedValue { get; set; }
        public double? Error { get; set; }

        public DbForecastResult(ForecastResult source)
        {
            this.MeasurementID = source.MeasurementID;
            this.TimeStamp = source.TimeStamp;
            this.RealValue = source.RealValue;
            this.PredictedValue = source.PredictedValue;
            this.Error = source.Error;
        }

        public ForecastResult ConvertBack()
        {
            var result = new ForecastResult();
            result.MeasurementID = MeasurementID;
            result.TimeStamp = TimeStamp;
            result.RealValue = RealValue;
            result.PredictedValue = PredictedValue;
            result.Error = Error;

            return result;
        }
    }
}

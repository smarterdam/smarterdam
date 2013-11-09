using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Smarterdam.Api;
using Smarterdam.DataAccess;
using Smarterdam.Entities;
using Smarterdam.Log;
using Smarterdam.Pipelines;

namespace Smarterdam.Filters
{
    public class ResultOutputFilter : BaseFilter
    {
        private readonly IForecastResultRepository repository;

        public int MeasurementId { get; set; }
        private bool _purged;

        public ResultOutputFilter(IForecastResultRepository repository)
        {
            this.repository = repository;
        }

        protected override DataStreamUnit[] _Execute(DataStreamUnit[] source)
        {
            try
            {
                if (!_purged)
                {
                    repository.Purge(this.MeasurementId);
                    repository.Create(this.MeasurementId);
                    _purged = true;
                }

                var data = source.ToList();

                var result = new ForecastResult();
                result.MeasurementID = this.MeasurementId;
                result.Error = data[0].Values["MAPE"] as double?;
                result.TimeStamp = (DateTime)data[0].Values["TimeStamp"];
                result.RealValue = data[0].Values["Value"] as double?;
                result.PredictedValue = data[0].Values["PredictedValue"] as double?;

                repository.Add(this.MeasurementId, result);
            }
            catch (Exception e)
            {
                Logging.Info(e.Message);
            }

            return source;
        }
    }
}

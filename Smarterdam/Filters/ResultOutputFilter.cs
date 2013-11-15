using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using MongoRepository;
using Smarterdam.Api;
using Smarterdam.Entities;
using Smarterdam.Log;
using Smarterdam.Pipelines;

namespace Smarterdam.Filters
{
    public class ResultOutputFilter : BaseFilter
    {
        private readonly IRepository<Measurement> repository;

        public string MeasurementId { get; set; }
        public string ForecastModelId { get; set; }
        private bool _initialized;
        private Measurement _measurement;
        private Forecast _forecast;

        public ResultOutputFilter(IRepository<Measurement> measurementRepository)
        {
            this.repository = measurementRepository;
        }

        protected override DataStreamUnit[] _Execute(DataStreamUnit[] source)
        {
            try
            {
                if (!_initialized)
                {
                    _forecast = new Forecast() { ForecastModelId = this.ForecastModelId };
                    _measurement = repository.FirstOrDefault(x => x.MeasurementId == this.MeasurementId);
                    if (_measurement == null)
                    {
                        _measurement = new Measurement {MeasurementId = this.MeasurementId};
                    }

                    _measurement.Forecasts.Add(_forecast);
                    _initialized = true;
                }

                var data = source.ToList();

                var result = new ForecastResult();
                result.Error = data[0].Values["MAPE"] as double?;
                result.TimeStamp = (DateTime)data[0].Values["TimeStamp"];
                result.RealValue = data[0].Values["Value"] as double?;
                result.PredictedValue = data[0].Values["PredictedValue"] as double?;

                _forecast.Error = result.Error;
                _forecast.Results.Add(result);
                repository.Update(_measurement);
            }
            catch (Exception e)
            {
                Logging.Info(e.Message);
            }

            return source;
        }
    }
}

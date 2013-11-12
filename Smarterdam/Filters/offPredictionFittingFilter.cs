using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Smarterdam.Api;
using Smarterdam.Models.NeuralNetwork;
using Smarterdam.Pipelines;

namespace Smarterdam.Filters
{
    public class offPredictionFittingFilter : BaseFilter
    {
        FilterParameters parameters;

        private List<TimeSeries> storeTimeSeries;
        private int counter = 0;
        private DataExchange exchange;

        private int PACKAGE_SIZE = 96;
        private DateTime waitUntil;
        private int globalCounter;
        private ForecastSettings settings;

        private Dictionary<string, int> time;

        private List<TimeSeries> timeSeriesEnsemble = new List<TimeSeries>();

        public offPredictionFittingFilter(FilterParameters parameters, DateTime trainTill, Dictionary<string, int> time, DataExchange exchange = null)
        {
            this.exchange = exchange;
            this.time = time;
            this.waitUntil = trainTill;
            this.parameters = parameters;

            settings = new ForecastSettings();
            for (int i = 0; i < 5; i++)
            {
                settings.energyLags.Add(i + 1);
            }

            for (int i = 0; i < PACKAGE_SIZE; i++)
            {
                timeSeriesEnsemble.Add(new TimeSeries(0));
            }
        }

        protected override DataStreamUnit[] _Execute(DataStreamUnit[] input)
        {
            var newValue = input[0];

            var sw = new Stopwatch();
            sw.Start();

            var currentTimeSeriesNumber = counter % PACKAGE_SIZE;

            var dateTimeStr = newValue.Values["TimeStamp"].ToString();
            DateTime dateTime;
            DateTime.TryParse(dateTimeStr, out dateTime);

            var currentTimeSeries = this.timeSeriesEnsemble[currentTimeSeriesNumber];

            currentTimeSeries.timeseries.Add(double.Parse(newValue.Values["Value"].ToString()));
            currentTimeSeries.timestamps.Add(dateTime);
            currentTimeSeries.temperature.Add(0);
            currentTimeSeries.status.Add(0);
            currentTimeSeries.cluster.Add(0);

            counter++;

            if (counter == PACKAGE_SIZE)
            {
                exchange["TrainingInProcess"] = true;
                exchange["LastDate"] = dateTime;
                var model = parameters["model"] as MultipleNeuralNetworksModel;
                model.Train(timeSeriesEnsemble, settings, true);
                counter = 0;
                this.time[globalCounter + "Training"] = 1;
                exchange["TrainingInProcess"] = false;
            }
            else
            {
                this.time[globalCounter + "Training"] = 0;
            }

            if (dateTime > waitUntil && (parameters["TrainingFinished"] == null || (bool)parameters["TrainingFinished"] == false))
            {
                parameters["TrainingFinished"] = true;
            }

            sw.Stop();
            this.time[globalCounter + "Value"] = (int)sw.ElapsedTicks;

            globalCounter++;

            return input;
        }
    }
}

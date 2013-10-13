using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Smarterdam.Api;
using Smarterdam.Models.NeuralNetwork;
using Smarterdam.Pipelines;

namespace Smarterdam.Filters
{
    public class onNeuralPredictionFilter : BaseFilter
    {
        FilterParameters parameters;

        private timeSeries timeSeries;
        private forecastSettings settings;
        private mns96_incremental model;
        private bool trainingFinished;

        private DateTime waitUntil;

        private int globalCounter;
        private const int NN_NUMBER = 4;

        public onNeuralPredictionFilter(FilterParameters parameters, DateTime trainTill)
        {
            this.waitUntil = trainTill;
            //this.iterations = testSize;
            this.timeSeries = new timeSeries(0);

            settings = new forecastSettings();
            for (int i = 0; i < 5; i++)
            {
                settings.energyLags.Add(i + 1);
            }

            model = new mns96_incremental();
            this.parameters = parameters;
            parameters.Values["model"] = model;
        }

        protected override DataStreamUnit[] _Execute(DataStreamUnit[] input)
        {
            var newValue = input[0];

            var sw = new Stopwatch();
            sw.Start();
            newValue.Values["Stopwatch"] = sw;

            var dateTimeStr = newValue.Values["TimeStamp"].ToString();
            DateTime dateTime;
            DateTime.TryParse(dateTimeStr, out dateTime);
            
            if (dateTime > waitUntil && !trainingFinished)
            {
                while (parameters["TrainingFinished"] == null)
                {
                    Thread.Sleep(100);
                }
                trainingFinished = true;
            }

            var realValue = double.Parse(newValue.Values["Value"].ToString());

            this.timeSeries.timeseries.Add(realValue);
            this.timeSeries.timestamps.Add(dateTime);
            //this.timeSeries.outlierLabel.Add(int.Parse(newValue.Values["Result"].ToString()));
            this.timeSeries.temperature.Add(0);
            this.timeSeries.status.Add(0);
            this.timeSeries.cluster.Add(0);

            var forecastResult = model.forecast(timeSeries, 0, settings);
            newValue.Values["PredictedValue"] = forecastResult.timeseries.Last();
            forecastResult.timeseries[forecastResult.timeseries.Count - 1] = realValue;

            newValue.Values["EnergyLag"] = waitUntil;

            if (model.mlnset.Count > globalCounter % NN_NUMBER)
            {
                parameters.Values["NeuralNetwork"] = model.mlnset[globalCounter % NN_NUMBER];
            }

            globalCounter++;

            return input;
        }
    }
}

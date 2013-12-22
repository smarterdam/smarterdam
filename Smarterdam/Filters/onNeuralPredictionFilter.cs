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

        private TimeSeries timeSeries;
        private ForecastSettings settings;
        private MultipleNeuralNetworksModel model;
        private bool trainingFinished;
        private DataExchange exchange;

        private DateTime waitUntil;

        private int globalCounter;
        private const int NN_NUMBER = 4;

        public onNeuralPredictionFilter(FilterParameters parameters, DateTime trainTill, DataExchange exchange = null)
        {
            this.exchange = exchange;
            this.waitUntil = trainTill;
            this.timeSeries = new TimeSeries(0);

            settings = new ForecastSettings();
            for (int i = 0; i < 5; i++)
            {
                settings.energyLags.Add(i + 1);
            }

            model = new MultipleNeuralNetworksModel();
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

            //нужно, чтобы симулировать приход значений раз в 15 минут
            //то есть чтобы онлайн ждал, пока оффлайн дотренирует НС до текущей даты
            while(exchange["TrainingInProcess"] as bool? == true && dateTime >= (exchange["LastDate"] as DateTime?))
            {
                Thread.Sleep(100);
            }
            
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
            //this.TimeSeries.outlierLabel.Add(int.Parse(newValue.Values["Result"].ToString()));
            this.timeSeries.temperature.Add(0);
            this.timeSeries.status.Add(0);
            this.timeSeries.cluster.Add(0);

            var forecastResult = model.Forecast(timeSeries, 0, settings);
            if(forecastResult.HasValue) forecastResult = Math.Max(forecastResult.Value, 0.0);
            newValue.Values["PredictedValue"] = forecastResult;
            
            newValue.Values["EnergyLag"] = waitUntil;

            if (model.NetworkSet.Count > globalCounter % NN_NUMBER)
            {
                parameters.Values["NeuralNetwork"] = model.NetworkSet[globalCounter % NN_NUMBER];
            }

            globalCounter++;

            return input;
        }
    }
}

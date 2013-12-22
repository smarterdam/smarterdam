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
    public class MeanPredictionFilter : BaseFilter
    {
        private double sum = 0;
        private double errorSum = 0;
        private double counter = 0;
        public MeanPredictionFilter()
        {
            
        }

        protected override DataStreamUnit[] _Execute(DataStreamUnit[] input)
        {
            var newValue = input[0];
            var predictedValue = 0.0;

            if (newValue.Values.ContainsKey("Value"))
            {
                var actualValue = double.Parse(newValue.Values["Value"].ToString());
                counter++;
                sum += actualValue;
                predictedValue = sum / counter;
                if (actualValue > 0)
                {
                    errorSum += Math.Abs((actualValue - predictedValue)/actualValue);
                    newValue.Values["MAPE"] = errorSum / counter;
                }
            }

            newValue.Values["PredictedValue"] = predictedValue;

            return new DataStreamUnit[] { newValue };
        }
    }
}

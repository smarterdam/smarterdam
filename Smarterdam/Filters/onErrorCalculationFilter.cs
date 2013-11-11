using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NeuralNetworksLibrary.NNLibrary.EvolvingNN;
using Smarterdam.Api;
using Smarterdam.Pipelines;

namespace Smarterdam.Filters
{
    public class onErrorCalculationFilter : BaseFilter
    {
        private int counter = 0;
        private int D;
        private int testCounter = 0; //счетчик замеров в тестовой выборке
        private double mapeSum = 0; //сумма ошибок MAPE до текущего момента * testCounter

        private FilterParameters parameters;

        private MultiLayersNN neuralNetwork;

        public onErrorCalculationFilter(FilterParameters parameters)
        {
            this.parameters = parameters;
        }

        protected override DataStreamUnit[] _Execute(DataStreamUnit[] input)
        {
            var value = input[0];

            value.Values["Number"] = counter;
            var waitUntil = DateTime.Parse(value.Values["EnergyLag"].ToString());
            var dateTime = DateTime.Parse(value.Values["TimeStamp"].ToString());
            if (dateTime > waitUntil)
            {
                List<double> absoluteErrors = new List<double>();
                neuralNetwork = parameters["NeuralNetwork"] as MultiLayersNN;
                if (neuralNetwork != null)
                {
                    absoluteErrors = neuralNetwork.absoluteErrors;
                }

                var realValue = Double.Parse(value.Values["Value"].ToString());
                var predictedValue = value.Values["PredictedValue"] != null ? Double.Parse(value.Values["PredictedValue"].ToString()) : (double?)null;

                if (predictedValue != null)
                {
                    var absoluteError = Math.Abs(predictedValue.Value - realValue);

                    if (realValue != 0)
                    {
                        counter++;
                        mapeSum += Math.Abs(absoluteError/realValue);
                    }

                    testCounter++;
                    value.Values["MAPE"] = mapeSum/testCounter;
                }

                D = (int)Math.Round(absoluteErrors.Count / 1.0, MidpointRounding.AwayFromZero);

                value.Values["D"] = D;

                double errorSum = 0;
                for (int i = absoluteErrors.Count - D; i < absoluteErrors.Count; i++)
                {
                    errorSum += absoluteErrors[i];
                }
                var mu = errorSum / D;

                value.Values["mu"] = mu;

                double sum = 0;
                for (int i = makeNonNegative(absoluteErrors.Count - D) + 1; i < absoluteErrors.Count; i++)
                {
                    sum += Math.Pow(absoluteErrors[i] - mu, 2);
                }

                var resultError = Math.Sqrt(sum) / D;

                value.Values["Error"] = resultError;

            }
            else
            {
                value.Values["MAPE"] = null;
            }
            return input;
        }

        protected int makeNonNegative(int source)
        {
            return source >= 0 ? source : 0;
        }
    }
}

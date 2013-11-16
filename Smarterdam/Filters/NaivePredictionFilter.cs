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
    public class NaivePredictionFilter : BaseFilter
    {
        Queue<DataStreamUnit> queue = new Queue<DataStreamUnit>(96);
        private double sum;
        private double counter;
        public NaivePredictionFilter()
        {
            
        }

        private DataStreamUnit Dequeue()
        {
            if (queue.Count < 96) return null;
            else return queue.Dequeue();
        }

        private void Enqueue(DataStreamUnit unit)
        {
            while (queue.Count >= 96) queue.Dequeue();
            queue.Enqueue(unit);
        }

        protected override DataStreamUnit[] _Execute(DataStreamUnit[] input)
        {
            var newValue = input[0];

            Enqueue(newValue);

            var pred = Dequeue();
            var actualValue = double.Parse(newValue.Values["Value"].ToString());
            if (pred != null)
            {
                var predictedValue = double.Parse(pred.Values["Value"].ToString());

                newValue.Values["PredictedValue"] = predictedValue;

                if (actualValue > 0)
                {
                    counter++;
                    sum += Math.Abs((actualValue - predictedValue)/actualValue);
                }

                newValue.Values["MAPE"] = sum/counter;
            }
            else
            {
                newValue.Values["PredictedValue"] = null;
                newValue.Values["MAPE"] = null;
            }

            return new DataStreamUnit[] { newValue };
            

            return input;
        }
    }
}

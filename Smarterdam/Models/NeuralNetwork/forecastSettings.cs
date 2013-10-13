using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smarterdam.Models.NeuralNetwork
{
    public class forecastSettings
    {
        public List<int> energyLags;
        public List<int> statusLags;
        public List<int> temparatureLags;
        public List<int> dayofweekLags;



        public List<timeSeries> tsList;


        public int neuronsInHiddenLayer;

        public forecastSettings()
        {
            energyLags = new List<int>();
            statusLags = new List<int>();
            temparatureLags = new List<int>();
            dayofweekLags = new List<int>();

            neuronsInHiddenLayer = 5;
        }

    }
}

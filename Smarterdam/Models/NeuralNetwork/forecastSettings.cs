using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smarterdam.Models.NeuralNetwork
{
    public class ForecastSettings
    {
        //лаги для прогнозирования энергии. 
        //Значения лагов 1,2,3,4 означают, что для прогнозирования следующего 
        //значения будут взяты значения 1-, 2-, 3- и 4-дневной давности
        public List<int> energyLags; 
        
        public List<TimeSeries> tsList;
        
        public int neuronsInHiddenLayer;

        public ForecastSettings()
        {
            energyLags = new List<int>();

            neuronsInHiddenLayer = 5;
        }

    }
}

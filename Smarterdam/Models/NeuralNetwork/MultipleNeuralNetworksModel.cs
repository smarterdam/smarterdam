using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NeuralNetworksLibrary.NNLibrary.EvolvingNN;

namespace Smarterdam.Models.NeuralNetwork
{
    /// <summary>
    ////смысл этой модели: разбиваем весь временной ряд на 96 временных рядов соответствующих времени измерения.
    /// для каждого нового временного ряда формируем НС модель
    /// </summary>
    public class MultipleNeuralNetworksModel
    {
       //количество замеров в одном дне
        private int timestampNumber;

        private double[] maxValues;
        private double[] minValues;

        private List<MultiLayersNN> networkSet = new List<MultiLayersNN>();

        public List<MultiLayersNN> NetworkSet
        {
            get { return networkSet; }
        }
        
        public MultipleNeuralNetworksModel(int timestampNumber = 96)
        {
            this.timestampNumber = timestampNumber;

            maxValues = new double[timestampNumber];
            minValues = new double[timestampNumber];
        }


        public double? Forecast(TimeSeries source, int horizon, ForecastSettings settings)
        {
            var i = source.timeseries.Count - 1; //это означает, что хотим получить прогноз для последнего значения в ряду
            var counter = i % timestampNumber;
            List<double> inputVector = createInputVector(i, source, settings, maxValues[counter], minValues[counter], timestampNumber);

            if (inputVector == null) return null; //не получилось
            if (networkSet.Count <= counter) return null;

            networkSet[counter].propagate(inputVector);

            return networkSet[counter].Outputs.First();
        }

        /// <summary>
        /// Тренировка модели.
        /// </summary>
        /// <param name="source">Подготовленный список значений для тренировки. 
        /// "Подготовленность" заключается в том, что каждое значение списка содержит все значения в соответствующее время суток.
        /// Например, source[0] содержит все значения энергопотребления в 00:00, source[1] - в 00:15, и так далее. В данном примере длина списка будет равна 96.</param>
        /// <param name="settings">Настройки прогнозирования.</param>
        /// <param name="additionalTraining">Производится ли дообучение модели.</param>
        public void Train(List<TimeSeries> source, ForecastSettings settings, bool additionalTraining)
        {
            var trainSet = new List<StructuredDataSet>();
            var cvSet = new List<StructuredDataSet>();

            for (int i = 0; i < timestampNumber; i++)
            {
                trainSet.Add(new StructuredDataSet());
                cvSet.Add(new StructuredDataSet());

                double valueMax = 0;
                double valueMin = double.MaxValue;
                for (int day = 0; day < source[i].timeseries.Count; day++)
                {
                    var value = source[i].timeseries[day];
                    valueMax = Math.Max(value, valueMax);
                    valueMin = Math.Min(value, valueMin);
                }

                maxValues[i] = valueMax;
                minValues[i] = valueMin;

                trainSet[i].Pairs = new List<DataPair>();

                //преобразуем то, что нужно в выборки данных.
                //формируем trainingset
                for (int time = settings.energyLags.Max(); time < source[i].timeseries.Count(); time++)
                {
                    DataPair trainingPair = new DataPair();

                    trainingPair.InputVector = createInputVector(time, source[i], 
                        settings, maxValues[i], minValues[i], 1);

                    trainingPair.OutputVector = createOutputVector(time, source[i], 
                        maxValues[i], minValues[i]);

                    if(trainingPair.InputVector != null)
                        trainSet[i].Pairs.Add(trainingPair);
                }

                if (trainSet[i].Pairs.Any())
                {
                    var neuralNetwork = new MultiLayersNN(
                        trainSet[i].Pairs[0].InputVector.Count,
                        trainSet[i].Pairs[0].OutputVector.Count,
                        4,
                        new[]
                            {
                                trainSet[i].Pairs[0].InputVector.Count, 
                                8, 8,
                                trainSet[i].Pairs[0].OutputVector.Count
                            });

                    neuralNetwork.trainNetwork(trainSet[i], 1000, 0.1, true);

                    for (int e = 0; e < neuralNetwork.absoluteErrors.Count; e++)
                    {
                        //денормализация
                        neuralNetwork.absoluteErrors[e] = (neuralNetwork.absoluteErrors[i] * (maxValues[i] - minValues[i]) + minValues[i]);
                    }

                    if (networkSet.Count <= i)
                        networkSet.Add(neuralNetwork);
                    else
                        networkSet[i] = neuralNetwork;
                }
            }
        }
        
        /// <summary>
        /// Извлечь входной вектор для обучения
        /// </summary>
        /// <param name="counter">Порядковый номер значения во временном ряду, которое будет прогнозироваться.</param>
        /// <param name="source">Временной ряд, из которого брать значения.</param>
        /// <param name="settings">Настройки прогнозирования.</param>
        /// <param name="_maxValue">Максимальное значение для данного времени суток.</param>
        /// <param name="_minValue">Минимальное значение для данного времени суток.</param>
        /// <param name="timestampNumber">Количество отсчетов в день.</param>
        /// <returns></returns>
        private List<double> createInputVector(int counter, TimeSeries source, ForecastSettings settings, double _maxValue, double _minValue, int timestampNumber)
        {
            //вычисляем нижнюю границу, т.е. минимальное количество ретроспективных значений,
            //необходимое для осуществления прогноза
            var minLength = settings.energyLags.Max() * timestampNumber;

            //если доступно значений меньше, чем нужно, то ничего не получится
            if (counter < minLength) return null;

            List<double> InputVector = new List<double>();

            InputVector.Add(1.0); //это вот точно здесь нужно? зачем?

            for (int j = 0; j < settings.energyLags.Count; j++)
            {
                var energyValue = source.timeseries[counter - (settings.energyLags[j] * timestampNumber)];
                InputVector.Add((energyValue - _minValue) / (_maxValue - _minValue));
            }

            return InputVector;
        }

        /// <summary>
        /// Извлечь выходные данные для обучения.
        /// </summary>
        /// <param name="counter"></param>
        /// <param name="_ts"></param>
        /// <param name="_fs"></param>
        /// <param name="_maxValue"></param>
        /// <param name="_minValue"></param>
        /// <returns></returns>
        private List<double> createOutputVector(int counter, TimeSeries source, double maxValue, double minValue)
        {
            List<double> OutputVector = new List<double>();

            OutputVector.Add((source.timeseries[counter] - minValue) / (maxValue - minValue));
            return OutputVector;
        }
    }
}

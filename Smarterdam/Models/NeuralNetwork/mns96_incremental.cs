using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NeuralNetworksLibrary.NNLibrary.EvolvingNN;

namespace Smarterdam.Models.NeuralNetwork
{
    /// <summary>
    ////смысл этой модели: разбиваем весь временной ряд на 96 временных рядов соответствующих времени измерения.
    /// для каждого нового временног ряда формируем НС модель
    /// </summary>
    public class mns96_incremental
    {
        private List<StructuredDataSet> trainingSet;
        private List<StructuredDataSet> cvSet;

        //количество замеров в одном дне
        private const int n_timestamps = 96;

        private double[] maxValue;
        private double[] minValue;

        private double[] maxValueT;
        private double[] meanValueT;

        public List<MultiLayersNN> mlnset;

        private int counter = 0;

        public List<double> absoluteErrors = new List<double>();


        public mns96_incremental()
        {
            trainingSet = new List<StructuredDataSet>();

            cvSet = new List<StructuredDataSet>();
            mlnset = new List<MultiLayersNN>();
            maxValue = new double[n_timestamps];
            minValue = new double[n_timestamps];
            maxValueT = new double[n_timestamps];
            meanValueT = new double[n_timestamps];
        }

        public timeSeries forecast(timeSeries _ts, int _horizon, forecastSettings _fs)
        {
            timeSeries f = _ts;
            _horizon = 0;

            //формируем входной вектор
            List<double> InputVector = new List<double>();
            int i = _ts.timeseries.Count - 1;
            InputVector = createInputVector(i, _ts, _fs, maxValue[counter], minValue[counter], n_timestamps, maxValueT[counter], meanValueT[counter], false);
            if (InputVector != null)
            {
                if (mlnset.Count > counter)
                {
                    mlnset[counter].propagate(InputVector);

                    foreach (var output in mlnset[counter].Outputs)
                    {
                        f.timestamps[i] = _ts.timestamps[i];
                        f.timeseries[i] = (output * (maxValue[counter] - minValue[counter]) + minValue[counter]);

                    }
                    counter++;
                    if (counter == n_timestamps)
                    {
                        counter = 0;
                    }
                }
            }

            //if (_ts.timeseries.Count == trainingSize)
            //{
            //    //extract training set;

            //    timeSeries trainingTS = _ts.getSample(0, _ts.timeseries.Count);

            //    int _hour = 0;
            //    int _minutes = 0;
            //    for (int i = 0; i < n_timestamps; i++)
            //    {

            //        tslocal.Add(trainingTS.getSample(_hour, _minutes, true));

            //        _minutes = _minutes + 15;
            //        if (_minutes == 60)
            //        {
            //            _hour++;
            //            _minutes = 0;
            //        }
            //    }

            //    //traing
            //    Console.WriteLine("train");
            //    trainingEnsemble(tslocal, _fs);
            //}
            //else
            //{
            //    if (_ts.timeseries.Count > trainingSize)
            //    {
            //        //формируем входной вектор
            //        List<double> InputVector = new List<double>();
            //        int i = _ts.timeseries.Count - 1;
            //        InputVector = createInputVector(i, _ts, _fs, maxValue[counter], minValue[counter], n_timestamps, maxValueT[counter], meanValueT[counter]);
            //        mlnset[counter].propagate(InputVector);

            //        foreach (var output in mlnset[counter].Outputs)
            //        {
            //            f.timestamps[i] = _ts.timestamps[i];
            //            f.timeseries[i] = (output + minValue[counter]) * maxValue[counter];

            //        }
            //        counter++;
            //        if (counter == n_timestamps)
            //        {
            //            counter = 0;
            //        }   
            //    }
            //}

            return f;
        }

        public void trainingEnsemble(List<timeSeries> tslocal, forecastSettings _fs, bool additionalTraining)
        {
            trainingSet = new List<StructuredDataSet>();
            for (int ts = 0; ts < n_timestamps; ts++)
            {
                //create training set 

                //Console.WriteLine("train nns for timestamp = " + ts.ToString() + " out of 95");
                trainingSet.Add(new StructuredDataSet());
                cvSet.Add(new StructuredDataSet());

                double valueMax = 0;
                double valueMin = double.MaxValue;
                for (int i = 0; i < tslocal[ts].timeseries.Count; i++)
                {
                    //if (tslocal[ts].timeseries[i] == 0)
                    {
                        var value = tslocal[ts].timeseries[i];
                        if (value > valueMax) valueMax = value;
                        if (value < valueMin) valueMin = value;
                    }
                }

                maxValue[ts] = valueMax;
                minValue[ts] = 0.0;// valueMin;//_ts.timeseries.Average() / maxValue; //

                maxValueT[ts] = tslocal[ts].temperature.Max();
                meanValueT[ts] = 0.0;//_ts.timeseries.Average() / maxValue; //


                trainingSet[ts].Pairs = new List<DataPair>();

                //преобразуем то, что нужно в выборки данных.
                //формируем trainingset
                for (int time = _fs.energyLags.Max(); time < tslocal[ts].timeseries.Count(); time++)
                //var time = tslocal[ts].timeseries.Count - 1;
                {
                    //if (tslocal[ts].outlierLabel[time] == 0)
                    {
                        DataPair trainingPair = new DataPair();

                        trainingPair.InputVector = new List<double>();
                        trainingPair.InputVector = createInputVector(time, tslocal[ts], _fs, maxValue[ts], minValue[ts],
                                                                     1, maxValueT[ts], meanValueT[ts], false);

                        trainingPair.OutputVector = new List<double>();
                        trainingPair.OutputVector = createOutputVector(time, tslocal[ts], _fs, maxValue[ts],
                                                                       minValue[ts]);
                        //trainingPair.OutputVector.Add(_ts.timeseries[i] / maxValue[counter] - minValue[counter]);

                        if (trainingPair.InputVector != null)
                            trainingSet[ts].Pairs.Add(trainingPair);
                    }
                }

                //do training

                if (trainingSet[ts].Pairs.Any())
                {
                    var mlNN = new MultiLayersNN(trainingSet[ts].Pairs[0].InputVector.Count, trainingSet[ts].Pairs[0].OutputVector.Count, 4, new[] { trainingSet[ts].Pairs[0].InputVector.Count, 8, 8, trainingSet[ts].Pairs[0].OutputVector.Count });
                    mlNN.trainNetwork(trainingSet[ts], 1000, 0.1, true);

                    int errorsNumber = mlNN.absoluteErrors.Count;

                    for (int i = 0; i < errorsNumber; i++)
                    {
                        mlNN.absoluteErrors[i] = (mlNN.absoluteErrors[i] * (maxValue[ts] - minValue[ts]) + minValue[ts]);
                    }

                    if (mlnset.Count <= ts)
                    {
                        mlnset.Add((mlNN));
                    }
                    else
                    {
                        mlnset[ts] = mlNN;
                    }
                }

            }
        }

        private List<double> createInputVector(int counter, timeSeries _ts, forecastSettings _fs, double _maxValue, double _minValue, int _multiplier, double _maxValueT, double _meanValueT, bool replaceOutliers)
        {
            var lowerBound = counter - (_fs.energyLags.Max() * _multiplier);
            if (lowerBound < 0)
            {
                return null;
            }

            List<double> InputVector = new List<double>();

            InputVector.Add(1.0);

            //добавляем все что только можно

            //InputVector.Add(Convert.ToDouble(_ts.timestamps[counter].DayOfWeek) / 7);

            //InputVector.Add(Convert.ToDouble(_ts.timestamps[counter].Month) / 12);


            //cvPair.InputVector.Add(Convert.ToDouble(_ts.timestamps[i].Hour) / 24);

            //add status input
            for (int j = 0; j < _fs.statusLags.Count; j++)
            {
                InputVector.Add((Double)_ts.status[counter - _fs.statusLags[j]]);
            }

            //add energy inputs
            double? correctValue = null;
            if (replaceOutliers)
            {
                for (int j = 0; j < _fs.energyLags.Count; j++)
                {
                    //if (_ts.outlierLabel[counter - (_fs.energyLags[j]*_multiplier)] == 0)
                    {
                        correctValue = _ts.timeseries[counter - (_fs.energyLags[j] * _multiplier)];
                        break;
                    }
                }

                if (correctValue == null) return null;
            }

            for (int j = 0; j < _fs.energyLags.Count; j++)
            {
                double energyValue;
                if (replaceOutliers /*&& _ts.outlierLabel[counter - (_fs.energyLags[j]*_multiplier)] != 0*/)
                {
                    energyValue = correctValue.Value;
                }
                else
                {
                    energyValue = _ts.timeseries[counter - (_fs.energyLags[j] * _multiplier)];
                }
                InputVector.Add((energyValue - _minValue) / (_maxValue - _minValue));
            }

            //int k = 0;
            //for (int j = 0; j < _fs.energyLags.Count; j++)
            //{
            //    if (_ts.status[counter] == _ts.status[counter - (_fs.energyLags[j] * _multiplier)])
            //    {
            //        InputVector.Add(_ts.timeseries[counter - (_fs.energyLags[j] * _multiplier)] / _maxValue - _minValue);
            //        k++;
            //    }                
            //}

            //if (k == 0)
            //{
            //    Console.WriteLine("SHIT!!!");
            //    InputVector = null;
            //}
            //else
            //{

            //    if (k < _fs.energyLags.Count)
            //    {
            //        int dif = Math.Abs(k - _fs.energyLags.Count);


            //        for (int m = 0; m < dif; m++)
            //        {
            //            InputVector.Add(InputVector[_fs.statusLags.Count  + 1]);
            //        }

            //    }

            //}
            //add temperature inputs
            for (int j = 0; j < _fs.temparatureLags.Count; j++)
            {
                InputVector.Add(_ts.timeseries[counter - (_fs.energyLags[j] * _multiplier)] / _maxValueT - _meanValueT);
            }


            //bool ender = false;
            //int cx = 0;
            //while (!ender)
            //{
            //    if (_ts.status[counter] == _ts.status[counter - (cx + 1) *  7 * _multiplier])
            //    {
            //        InputVector.Add(_ts.timeseries[counter - ((cx + 1) * 7 * _multiplier)] / _maxValue - _minValue);
            //        cx++;
            //    }

            //    if (cx == _fs.energyLags.Count)
            //    {
            //        ender = true;
            //    }

            //}


            return InputVector;
        }

        private List<double> createOutputVector(int counter, timeSeries _ts, forecastSettings _fs, double _maxValue, double _minValue)
        {
            List<double> OutputVector = new List<double>();

            OutputVector.Add((_ts.timeseries[counter] - _minValue) / (_maxValue - _minValue));
            return OutputVector;
        }

    }
}

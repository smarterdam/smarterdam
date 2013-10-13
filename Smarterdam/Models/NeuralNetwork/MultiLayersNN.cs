using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EvolvingNN;
using Smarterdam.Models.NeuralNetwork;

namespace NeuralNetworksLibrary.NNLibrary.EvolvingNN
{
    public class MultiLayersNN
    {
        private readonly List<Layer> layers;
        public double[] Outputs;

        public List<double> absoluteErrors = new List<double>(); 

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="inputCount">Число входов</param>
        /// <param name="outputCount">Число выходов</param>
        /// <param name="numberOfLayers">число слоев</param>
        /// <param name="numberOfNeuronsInLayers">число нейронов в слое</param>
        public MultiLayersNN(int inputCount, int outputCount, int numberOfLayers, IList<int> numberOfNeuronsInLayers)
        {
            
            layers = new List<Layer> { };
            //input layer
            layers.Add(new InputLayer(numberOfNeuronsInLayers[0], inputCount));
            //hidden & output layers
            for (int i = 0; i < numberOfLayers - 2; i++)
            {
                layers.Add(new AdditiveNonlinearLayer(numberOfNeuronsInLayers[i + 1], numberOfNeuronsInLayers[i]));
            }
            layers.Add(new AdditiveNonlinearOutputLayer(numberOfNeuronsInLayers[numberOfLayers - 1], numberOfNeuronsInLayers[numberOfLayers - 2]));
        }

        /// <summary>
        /// Прямое распространение
        /// </summary>
        /// <param name="inputs">Входной вектор</param>
        public void propagate(IList<double> inputs)
        {
            var fooVector = inputs;
            foreach (var layer in layers)
            {
                layer.propagate(fooVector);
                fooVector = layer.Outputs;
            }

            Outputs = (double[])fooVector;
        }

        public void backPropagate(IList<double> inputs, IList<double> outputs)
        {
           
                //calculate gradient;

                layers[layers.Count - 1].calculateDelta(outputs);
                for (int i = layers.Count - 2; i >= 1; i--)
                {
                    layers[i].calculateDelta(layers[i + 1].getDeltaSet(), layers[i + 1].getSynapses());
                }
                //change weights;
                for (int i = layers.Count - 1; i >= 1; i--)
                {
                    layers[i].correctWeight(layers[i - 1].Outputs);
                }
          
        }
        


        /// <summary>
        /// Обучение нейронной сети
        /// </summary>
        /// <param name="trainingSet">Обучающая выборка</param>
        public void trainNetwork(StructuredDataSet trainingSet, int numberOfIteration, double maximalError, bool additionalTraining = false)
        {
            int cx = 0;
            var errorNumber = 0;
            double overallError = maximalError + maximalError;
            double overallErrorSum = 0;

            for (int i = 0; i < trainingSet.Pairs.Count; i++)
            {
                absoluteErrors.Add(0);
            }

            while ((cx < numberOfIteration) && (overallError > maximalError))
            {
                overallError = 0;
                var errorCounter = 0;
                foreach (var pair in trainingSet.Pairs)
                {
                    propagate(pair.InputVector);
                    
                    //getMap(); Console.ReadKey();
                    backPropagate(pair.InputVector, pair.OutputVector);

                    double error = 0.0;
                    int j = 0;
                    foreach (var output in pair.OutputVector)
                    {   
                        error = Math.Abs(output - Outputs[j]);

                        absoluteErrors[errorCounter] = error;
                        errorCounter++;
                        
                        j++;
                        overallError += error;
                    }
                }
                overallError = overallError / (trainingSet.Pairs.Count);
                overallErrorSum += overallError;
                
                //Console.WriteLine("it#" + cx.ToString() + ";  e="+overallError.ToString());
                cx++;
            }

            //Console.WriteLine("\n number of iterations:" + cx);
            //Console.WriteLine("Final error = " + overallError);
        }

        public void getMap()
        {
            int i = 0;
            //Console.WriteLine("==============================================");
            foreach (var layer in layers)
            {
                //Console.WriteLine("\n layer = " + i );
                layer.getMap();
                i++;
            }
        }

    }
}

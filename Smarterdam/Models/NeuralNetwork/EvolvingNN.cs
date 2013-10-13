using System;
using System.Collections.Generic;
using System.Linq;
using Smarterdam.Models.NeuralNetwork;

namespace EvolvingNN
{
    public class EvolvingNeuralNetwork
    {
        private readonly List<Layer> layers;
        private const double Ethr = 0.3;
        private const double Sthr = 0.1;
        public double[] Outputs;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="inputCount">Размер входного вектора</param>
        /// <param name="outputCount">Размер выходного вектора</param>
        public EvolvingNeuralNetwork(int inputCount, int outputCount, IList<double> inputsSynapses, IList<double> outputsSynapses)
        {
            //без входного слоя
            layers = new List<Layer> { new EvolvingLayer(1, inputCount, inputsSynapses), new OutputLayer(outputCount, 1, outputsSynapses) };
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

        /// <summary>
        /// Обучение нейронной сети
        /// </summary>
        /// <param name="trainingSet">Обучающая выборка</param>
        public void trainNetwork(StructuredDataSet trainingSet)
        {
            foreach (var pair in trainingSet.Pairs)
            {
                propagate(pair.InputVector);

                int j;
                var aj = layers[0].getMaxOutput(out j);
                //j - индекс нейрона имеющего максимальную активацию.

                if (aj < Sthr)
                {
                    layers[0].addNeuron(pair.InputVector.Count, pair.InputVector);
                    Console.WriteLine("CASE 1. New neuron has been added");

                    layers[1].addSynapse(pair.OutputVector);//{todo} передать выход
                 }
                else
                {
                    var eo = distance(Outputs, pair.OutputVector);
                    if (eo > Ethr)
                    {
                        layers[0].addNeuron(pair.InputVector.Count, pair.InputVector);
                        Console.WriteLine("CASE 2. New neuron has been added");

                        layers[1].addSynapse(pair.OutputVector);
                    }
                    else
                    {

                        layers[0].correctWeight(j, pair.InputVector); //{todo} неверно настраивается

                        layers[1].correctWeight(j, eo);
                        Console.WriteLine("CASE 3. Weights were corrected");
                    }
                }
            }
        }

        private static double distance(IEnumerable<double> x1, IList<double> x2)
        {
            return Math.Sqrt(x1.Select((t, i) => Math.Pow(t - x2[i], 2)).Sum());
        }
    }
}

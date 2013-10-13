using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EvolvingNN;

namespace NeuralNetworksLibrary.NNLibrary.EvolvingNN
{
    class InputLayer : Layer
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="neuronCount">Число нейронов  в слое</param>
        /// <param name="inputCount">число входов  </param>
        public InputLayer(int neuronCount, int inputCount)
        {
            for (var i = 0; i < neuronCount; i++)
                addNeuron(inputCount);
        }

        public override void propagate(IList<double> inputs)
        {
            Outputs = new double[Neurons.Count];
            for (var i = 0; i < Neurons.Count; i++)
            {
                IList<double> tmpInputs = new[] { inputs[i]};
                Outputs[i] = Neurons[i].getOutput(tmpInputs);
            }
        }

        public override sealed void addNeuron(int inputCount)
        {
            Neurons.Add(new InputNeuron(inputCount));
        }
        /// <summary>
        /// No correct weights for input layer
        /// </summary>
        /// <param name="inputCount"></param>
        /// <param name="outputsSynapses"></param>
        public override sealed void addNeuron(int inputCount, IList<double> outputsSynapses) { }
        public override void correctWeight(int weightIndex, double eo){      }

        public override void correctWeight(int neuronIndex, IList<double> inputs) { }
        public override void calculateDelta(IList<double> desireOutputs) { }
        public override void calculateDelta(IList<double> deltaNextLayer, double[][] synapsesNextLayer) { }

        public override void correctWeight(double[] outputsPreviousLayer) { }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EvolvingNN;

namespace NeuralNetworksLibrary.NNLibrary.EvolvingNN
{
    class AdditiveNonlinearOutputLayer : Layer
    {
        public AdditiveNonlinearOutputLayer(int neuronCount, int inputCount)
        {
            for (var i = 0; i < neuronCount; i++)
                addNeuron(inputCount);
        }

        public override void propagate(IList<double> inputs)
        {
            Outputs = new double[Neurons.Count];
            for (var i = 0; i < Neurons.Count; i++)
            {
                Outputs[i] = Neurons[i].getOutput(inputs);
            }
        }

        public override sealed void addNeuron(int inputCount)
        {
            Neurons.Add(new AdditiveNonlinearOutputNeuron(inputCount));
        }
        public override sealed void addNeuron(int inputCount, IList<double> outputsSynapses) { }
        public override void correctWeight(int weightIndex, double eo)
        {
            //foreach (var neuron in Neurons)
            //{
            //    neuron.correctWeight(weightIndex, eo);
            //}
        }

        public override void correctWeight(int neuronIndex, IList<double> inputs) { }
        public override void correctWeight(double[] outputsPreviousLayer)
        {
            foreach (var neuron in Neurons)
            {
                neuron.correctWeight(outputsPreviousLayer);
            }        
        }


        public override void calculateDelta(IList<double> deltaNextLayer, double [][] synapsesNextLayer) { }

        public override void calculateDelta(IList<double> desireOutputs) 
        {
            for (var i = 0; i < Neurons.Count; i++)
            {
                Neurons[i].calculateDelta(desireOutputs[i]);
            }

        }

        //public override IList<double> getDeltaSet() { return null; }
    }
}

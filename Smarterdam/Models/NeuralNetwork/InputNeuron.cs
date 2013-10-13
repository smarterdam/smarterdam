using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EvolvingNN;

namespace NeuralNetworksLibrary.NNLibrary.EvolvingNN
{
    class InputNeuron: Neuron
    {
        //public InputNeuron(int inputCount)
        //    : base(inputCount)
        //{
        //}

        public InputNeuron(int inputCount) 
        {
            Synapses = new List<Synapse>();
            addSynapse(1);
        }

        public override double calculateActivation(IList<double> inputs)     
        {
            var summ = 0.0;
            summ = inputs[0];
            return summ;
        }

        public override void correctWeight(int index, IList<double> inputs) { }
        public override void correctWeight(double[] inputs) { }
        public override void correctWeight(int weightIndex, double eo) { }

        public override void calculateDelta(double desireOutput) { }
        public override void calculateDelta(IList<double> deltasNextLayer, double [] synapsesNextLayer) { }

    }
}

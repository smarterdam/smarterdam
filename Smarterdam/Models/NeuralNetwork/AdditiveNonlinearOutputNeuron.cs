using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EvolvingNN;

namespace NeuralNetworksLibrary.NNLibrary.EvolvingNN
{
    class AdditiveNonlinearOutputNeuron : AdditiveNonlinearNeuron
{
        
        public AdditiveNonlinearOutputNeuron(int inputCount)
            : base(inputCount)
        {

        }

        public override void correctWeight(int weightIndex, double eo) { }
        public override void correctWeight(int index, IList<double> inputs) { }

        public override void correctWeight(double [] inputs)
        {
            for (var i = 0; i < Synapses.Count; i++ )
            {
                Synapses[i].value += -0.5 * delta * inputs[i];
            }
        }

        public override void calculateDelta(double desireOutput)
        {
            //do calculation of delta
            delta = (state - desireOutput) * (1 - Math.Pow(state, 2));
        }
    }
}

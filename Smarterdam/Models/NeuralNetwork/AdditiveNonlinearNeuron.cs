using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EvolvingNN;

namespace NeuralNetworksLibrary.NNLibrary.EvolvingNN
{
    class AdditiveNonlinearNeuron :Neuron
    {
        
        public AdditiveNonlinearNeuron(int inputCount)
            : base(inputCount)
        {

        }

        public override void correctWeight(int weightIndex, double eo) { }

        public override void correctWeight(int index, IList<double> inputs) { }



        public override double calculateActivation(IList<double> inputs)
        {
            double sum = 0.0;

            state = 0.0;

            for (var i = 0; i < Synapses.Count; i++)
            {
                sum += Synapses[i].value * inputs[i];      
            }
            //state = sum;
            state =  Math.Tanh(sum + 0.5) ;

            return state;

            //A = 1 -distance(inputs);

            //return A;
        }
        public override void correctWeight(double[] inputs)
        {
            for (var i = 0; i < Synapses.Count; i++)
            {
                Synapses[i].value += -0.5 * delta * inputs[i];
            }
        }
        public override void calculateDelta(IList<double> deltasNextLayer, double[] synapsesNextLayer) 
        {
            //do calculation of delta
            double sum = 0.0;
            for (var i = 0; i < deltasNextLayer.Count; i++)
            {
                sum += deltasNextLayer[i] * synapsesNextLayer[i];
            }
            delta = sum * (1 - Math.Pow(state, 2));

        }

        public override void calculateDelta(double desireOutput) { }
       
    }
}

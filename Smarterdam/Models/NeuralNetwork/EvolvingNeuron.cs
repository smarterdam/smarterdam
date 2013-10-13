using System.Collections.Generic;
using System;
namespace EvolvingNN
{
    class EvolvingNeuron: Neuron
    {
        private double A;

        public EvolvingNeuron(int inputCount, IList<double> inputsSynapses)
            : base(inputCount, inputsSynapses)
        {
        }

        public override void correctWeight(int weightIndex, double eo) { }
        public override void correctWeight(double[] inputs) { }
        public override void correctWeight(int index, IList<double> inputs)
        {
            for (var i = 0; i < Synapses.Count; i++ )
            {
                Synapses[i].value += 0.1 * (inputs[i] - Synapses[i].value);
            }
        }

        private double distance(IList<double> x)
        {
            double summ = 0.0;
            for (var i = 0; i < x.Count; i++)
            {
                summ += Math.Pow(x[i] - Synapses[i].value, 2);
            }
            return Math.Sqrt(summ);
        }

        public override double calculateActivation(IList<double> inputs)
        {
            A = 1 - distance(inputs);

            return A;
        }

        public override void calculateDelta(double desireOutput) { }
        public override void calculateDelta(IList<double> deltasNextLayer, double[] synapsesNextLayer) { }

    }
}

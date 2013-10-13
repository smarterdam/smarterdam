using System.Collections.Generic;
namespace EvolvingNN
{
    class OutputNeuron: Neuron
    {
        public OutputNeuron(int inputCount, IList<double> outputsSynapses)
            : base(inputCount, outputsSynapses)
        {
        }
        public override void correctWeight(int index, IList<double> inputs) { }
        public override void correctWeight(double[] inputs) { }
        public override void correctWeight(int weightIndex, double eo)
        {
            Synapses[weightIndex].value += 0.1*Inputs[weightIndex]*eo;
        }

        public override double calculateActivation(IList<double> inputs)
        {            
            var summ = 0.0;
            for (var i = 0; i < inputs.Count; i++)
            {
                ////summ += inputs[i] * Synapses[i].value;
                summ += inputs[i] * Synapses[i].value;
            }
            return summ;
        }


        //overrided procedures

        public override void calculateDelta(double desireOutput) { }
        public override void calculateDelta(IList<double> deltasNextLayer, double[] synapsesNextLayer) { }

    }
}

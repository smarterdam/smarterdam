using System.Collections.Generic;
namespace EvolvingNN
{
    class OutputLayer: Layer
    {
        public OutputLayer(int neuronCount, int inputCount, IList<double> outputsSynapses)
        {
            for (var i = 0; i < neuronCount; i++)
                addNeuron(inputCount, outputsSynapses);
        }

        public override void propagate(IList<double> inputs)
        {
            Outputs = new double[Neurons.Count];
            for (var i = 0; i < Neurons.Count; i++)
            {
                Outputs[i] = Neurons[i].getOutput(inputs);
            }
        }

        public override sealed void addNeuron(int inputCount, IList<double> outputsSynapses)
        {
            Neurons.Add(new OutputNeuron(inputCount, outputsSynapses));
        }

        public override void correctWeight(int weightIndex, double eo)
        {
            foreach (var neuron in Neurons)
            {
                neuron.correctWeight(weightIndex, eo);
            }
        }
        public override sealed void addNeuron(int inputCount) { }
        public override void correctWeight(int neuronIndex, IList<double> inputs) { }

        public override void calculateDelta(IList<double> desireOutputs) { }
        public override void calculateDelta(IList<double> deltaNextLayer, double[][] synapsesNextLayer) { }

        public override void correctWeight(double[] outputsPreviousLayer) { }

    }
}

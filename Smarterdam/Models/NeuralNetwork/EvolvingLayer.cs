using System.Collections.Generic;
namespace EvolvingNN
{
    class EvolvingLayer: Layer
    {
        public EvolvingLayer(int neuronCount, int inputCount, IList<double> inputsSynapses)
        {
            for (var i = 0; i < neuronCount; i++)
                addNeuron(inputCount, inputsSynapses);
        }

        public override void propagate(IList<double> inputs)
        {
            Outputs = new double[Neurons.Count];
            Outputs[0] = Neurons[0].getOutput(inputs);
            int indexWinnerNeuron = 0;
            double maxValue = Outputs[0];

            for (var i = 0; i < Neurons.Count; i++)
            {
                Outputs[i] = Neurons[i].getOutput(inputs);
                if (Outputs[i] > maxValue)
                {
                    maxValue = Outputs[i];
                    indexWinnerNeuron = i;
                }
            }
            
            //for (var i = 0; i < Neurons.Count; i++)
            //{
            //    if (i == indexWinnerNeuron)
            //    {
            //        Outputs[i] = 1;
            //    }
            //    else
            //        Outputs[i] = 0;
            //}
            for (var i = 0; i < Neurons.Count; i++)
            {
                if (i != indexWinnerNeuron)
                {
                    Outputs[i] = 0;
                }
            }


        }

        public override sealed void addNeuron(int inputCount, IList<double> inputsSynapses)
        {
            Neurons.Add(new EvolvingNeuron(inputCount, inputsSynapses));
        }
        public override sealed void addNeuron(int inputCount) { }
        public override void correctWeight(int neuronIndex, IList<double> inputs)
        {
            Neurons[neuronIndex].correctWeight(neuronIndex, inputs);
        }
        public override void correctWeight(int neuronIndex, double eo){}
        //public override void calculateDelta() { }
        public override void calculateDelta(IList<double> desireOutputs) { }
        public override void calculateDelta(IList<double> deltaNextLayer, double[][] synapsesNextLayer) { }

        public override void correctWeight(double[] outputsPreviousLayer) { }
    }
}

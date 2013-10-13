using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EvolvingNN;

namespace NeuralNetworksLibrary.NNLibrary.EvolvingNN
{
    class AdditiveNonlinearLayer : Layer
    {
        public AdditiveNonlinearLayer(int neuronCount, int inputCount)
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
            Neurons.Add(new AdditiveNonlinearNeuron(inputCount));
        }
        public override sealed void addNeuron(int inputCount, IList<double> outputsSynapses) { }


        public override void correctWeight(int weightIndex, double eo){}

        public override void correctWeight(double [] outputsPreviousLayer)
        {
            foreach (var neuron in Neurons)
            {
                neuron.correctWeight(outputsPreviousLayer);
            }
        }

        public override void correctWeight(int neuronIndex, IList<double> inputs) { }
        public override void calculateDelta(IList<double> desireOutputs) { }

        public override void calculateDelta(IList<double> deltasNextLayer, double[][] synapsesNextLayer)
        {
            double[] tmp = new double[deltasNextLayer.Count];

            for (var i = 0; i < Neurons.Count; i++)
            {
                for (var j = 0; j < deltasNextLayer.Count; j++)
                {
                    tmp[j] = synapsesNextLayer[j][i];
                }
                Neurons[i].calculateDelta(deltasNextLayer, tmp);
            }
        }

        //public override IList<double> getDeltaSet()
        //{
        //    IList<double> deltaSet = new List<double>();
        //    //Neurons = new List<Neuron>();
        //    for (var i = 0; i < Neurons.Count; i++)
        //    {
        //        deltaSet.Add (Neurons[i].getDelta());
        //    }

        //    return deltaSet;
        //}

    }
}

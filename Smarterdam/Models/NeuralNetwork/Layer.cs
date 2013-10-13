using System.Collections.Generic;
using System;

namespace EvolvingNN
{
    abstract class Layer
    {
        protected readonly List<Neuron> Neurons;
        public double[] Outputs;


        protected Layer()
        {
            Neurons = new List<Neuron>();
        }

        public abstract void propagate(IList<double> inputs);
        //{
        //    Outputs = new double[Neurons.Count];
        //    for (var i = 0; i < Neurons.Count; i++ )
        //    {
        //        Outputs[i] = Neurons[i].getOutput(inputs);
        //    }
        //}

        public double getMaxOutput(out int maxIndex)
        {
            var max = Outputs[0];
            maxIndex = 0;
            for(var i=0; i< Outputs.GetLength(0); i++)
                if(Outputs[i]>max)
                {
                    max = Outputs[i];
                    maxIndex = i;
                }
            return max;
        }

        public void addSynapse(IList<double> synapses)
        {
            int i = 0;

            foreach (var neuron in Neurons)
            {
                neuron.addSynapse(synapses[i]);
                i++;

            }
        }



        public abstract void addNeuron(int inputCount);
        public abstract void addNeuron(int inputCount, IList<double> inputsSynapses);

        public abstract void correctWeight(int neuronIndex, IList<double> inputs);
        public abstract void correctWeight(int neuronIndex, double eo);

        public abstract void correctWeight(double [] outputsPreviousLayer);


        public abstract void calculateDelta(IList<double> desireOutputs);
        public abstract void calculateDelta(IList<double> deltaNextLayer, double [][] synapsesNextLayer);

        public IList<double> getDeltaSet()
        {
            IList<double> tmp = new List<double>();
            for (var i = 0; i < Neurons.Count; i++)
            {
                tmp.Add(Neurons[i].getDelta());
            }

            return tmp;
        }
        /// <summary>
        /// Возвращает двумерный массив синапсов слоя [индекс нейрона в текущем слое][индекс нейрона в предыдущем слое]
        /// </summary>
        /// <returns></returns>
        public  double[][] getSynapses()
        {
            double[][] synapsesList = new double[Neurons.Count][];
            for (var i = 0; i < Neurons.Count; i++)
            {
                synapsesList[i] =  Neurons[i].getSynapses();
            }
            return synapsesList;
        }

        public void getMap()
        {

            for (var i = 0; i < Neurons.Count; i++)
            {
                Console.WriteLine("\n neuron ["+i+"]");
                Neurons[i].getMap();
            }
        }
    }
}

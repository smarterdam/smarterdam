using System;
using System.Collections.Generic;
using System.Linq;

namespace EvolvingNN
{
    abstract class Neuron
    {
        protected IList<Synapse> Synapses;
        protected IList<double> Inputs;
        protected double state;
        protected double delta;

        protected Neuron() { }

        protected Neuron(int inputCount)
        {

            state = 0.0;
            delta = 0.0;


            Synapses = new List<Synapse>();
            for (var i = 0; i < inputCount; i++)
            {
                addSynapse();
            }
        }


        protected Neuron(int inputCount, IList<double> synapses)
        {
            state = 0.0;
            delta = 0.0;


            Synapses = new List<Synapse>();
            for (var i = 0; i < inputCount; i++)
            {
                addSynapse(synapses[i]);
            }
        }
        public double getOutput(IList<double> inputs)
        {
            Inputs = inputs;
            return calculateActivation(inputs);
        }

        public double[] getSynapses()
        {
            double[] tmp = new double[Synapses.Count];
            for (var i = 0; i < Synapses.Count; i++)
            {
                tmp[i] = Synapses[i].value;
            }
            return tmp;
        }

        public double getSynapseByIndex(int indexOfSynapse)
        {
            return Synapses[indexOfSynapse].value;
        }

        public abstract double calculateActivation(IList<double> inputs);
        //{
            //return 1 - distance(inputs);            
            //var summ = 0.0;
            //for (var i = 0; i < inputs.Count; i++)
            //{
            //    summ += inputs[i] * Synapses[i].value;
            //}
            //return summ;
        //}



        public void addSynapse()
        {
            Synapses.Add(new Synapse());
        }

        public void addSynapse(double value)
        {
            Synapses.Add(new Synapse(value));
        }

        public abstract void correctWeight(int index, double eo);

        public abstract void correctWeight(int index, IList<double> inputs);
        public abstract void correctWeight(double [] inputs);

        public abstract void calculateDelta(double desireOutput);
        public abstract void calculateDelta(IList<double> deltasNextLayer, double[] synapsesNextLayer);

        public double getDelta()
        {
            return delta;
        }


        public void getMap()
        {

            Console.Write("state = "+state + "; delta = " + delta);
            Console.Write("\n Synapses [");
            for (var i = 0; i < Synapses.Count; i++)
            {
                Console.Write(Synapses[i].value + ",");               
            }
            Console.Write("]");               
        }

    }
}


using System;
using Ninject;
using Smarterdam;
using Smarterdam.Helpers;

namespace EvolvingNN
{
    class Synapse
    {
        public double value { get; set; }

        public Synapse()
        {
            var rdm = Context.RandomValueProvider;
            double rdmValue = rdm.Next();
            value = rdmValue % 1000 * 0.001 * Math.Pow(-1, rdmValue % 2);//+ 1 * signhValue;
//            if (value > 0.5) value = 0.0; else value = 1.0;
            //value = 1.0;

        }
        public Synapse(double value)
        {
            this.value = value;
        }
    }
}

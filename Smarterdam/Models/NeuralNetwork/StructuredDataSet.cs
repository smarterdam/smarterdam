using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Smarterdam.Models.NeuralNetwork
{
    public class DataPair
    {
        public List<double> InputVector;
        public List<double> OutputVector;
    }

    public class StructuredDataSet
    {
        public List<DataPair> Pairs;


        public void saveToFile(String _fileName)
        {
            String aHeader = "";
            String aString = "";


            //create a header


            for (int i = 0; i < Pairs[0].InputVector.Count; i++)
            {
                aHeader += "input_" + i.ToString() + "; ";

            }
            aHeader += "output  \r\n";


            File.WriteAllText(_fileName, aHeader);

            foreach (var pair in Pairs)
            {
                foreach (var input in pair.InputVector)
                {
                    aString += input + "; ";

                }
                foreach (var output in pair.OutputVector)
                {
                    aString += output + "; ";

                }
                aString += " \r\n";

                File.AppendAllText(_fileName, aString);
                aString = "";
            }

        }

    }
}

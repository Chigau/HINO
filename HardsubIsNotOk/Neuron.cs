using System;
using System.Collections.Generic;
using System.Text;

namespace HardsubIsNotOk
{
    public class Neuron
    {
        public int index;
        public List<Neuron> inputs, outputs;
        public float[] weights, learningWeights;

        public float[] partDerivativesWeights, partDerivativesInputs;

        public float output = 1, learningOutput = 1;

        public Neuron(int index, List<Neuron> inputs)
        {
            this.index = index;
            this.inputs = inputs;
            if (inputs.Count > 0)
            {
                weights = new float[inputs.Count];
                learningWeights = new float[inputs.Count];
                partDerivativesWeights = new float[inputs.Count];
                partDerivativesInputs = new float[inputs.Count];
                for (int c = 0; c < weights.Length; c++)
                {
                    weights[c] = (float)Program.rand.NextDouble() * 2 - 1;
                    learningWeights[c] = weights[c];
                }
            }
        }

        public void CalculateOutput()
        {
            float z = 0;
            for (int c = 0; c < inputs.Count; c++)
                z -= inputs[c].output * weights[c];
            output = (float)(1 / (1 + Math.Exp(z)));
        }
        public void CalculateLearningOutput()
        {
            float z = 0;
            for (int c = 0; c < inputs.Count; c++)
                z -= inputs[c].learningOutput * learningWeights[c];
            learningOutput = (float)(1 / (1 + Math.Exp(z)));
        }
        public void CalculateDerivatives(float target = -1)
        {
            float out_net, etot_out;
            out_net = learningOutput * (1 - learningOutput);
            if (target != -1)
                etot_out = learningOutput - target;
            else
            {
                etot_out = 0;
                for (int c = 0; c < outputs.Count - 1; c++)
                    etot_out += outputs[c].partDerivativesInputs[index];

                if (outputs[outputs.Count - 1].index != -1) //controllo che non sia un bias
                    etot_out += outputs[outputs.Count - 1].partDerivativesInputs[index];
            }
            for (int c = 0; c < inputs.Count; c++)
            {
                partDerivativesWeights[c] = inputs[c].learningOutput * out_net * etot_out;
                partDerivativesInputs[c] = weights[c] * out_net * etot_out;
            }
        }
        public void UpdateLearningWeights(float n)
        {
            for (int c = 0; c < inputs.Count; c++)
                learningWeights[c] -= n * partDerivativesWeights[c];
        }
        public void ConfirmLearningWeights()
        {
            for (int c = 0; c < inputs.Count; c++)
                weights[c] = learningWeights[c];
        }
        public StringBuilder Serialize()
        {
            StringBuilder serialized = new StringBuilder();
            if (weights != null)
                foreach (float d in weights)
                {
                    serialized.Append(d.ToString("r"));
                    serialized.Append(';');
                }
            return serialized;
        }
        public void Load(string s)
        {
            string w = "";
            int index = 0;
            for (int c = 0; c < s.Length; c++)
            {
                if (s[c] != ';')
                    w += s[c];
                else
                {
                    weights[index] = float.Parse(w);
                    learningWeights[index] = weights[index];
                    w = "";
                    index++;
                }

            }
        }
    }
}
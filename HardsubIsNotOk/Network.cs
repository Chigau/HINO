﻿using System;
using System.Collections.Generic;
using System.Threading;

namespace HardsubIsNotOk
{
    class Network
    {
        public string value;
        public List<Neuron>[] neurons;
        public LearningThread learning;
        bool wait = false;

        public Network(string value, params int[] lengths)
        {
            this.value = value;
            learning = new LearningThread(this, 0.1);
            neurons = new List<Neuron>[lengths.Length + 1];
            for (int c = 0; c < neurons.Length - 1; c++)
            {
                neurons[c] = new List<Neuron>();
                for (int d = 0; d < lengths[c]; d++)
                {
                    if (c == 0)
                        neurons[c].Add(new Neuron(d, new List<Neuron>()));
                    else
                        neurons[c].Add(new Neuron(d, neurons[c - 1]));
                }
            }
            int output = neurons.Length - 1;
            neurons[output] = new List<Neuron>();
            neurons[output].Add(new Neuron(0, neurons[output - 1]));

            for (int c = 1; c < neurons.Length - 1; c++)
                for (int d = 0; d < lengths[c]; d++)
                    neurons[c][d].outputs = neurons[c + 1];

        }
        public void StartLearning()
        {
            if (!learning.running)
            {
                Thread learningThread = new Thread(learning.Learn);
                learningThread.IsBackground = true;
                learningThread.Start();
            }
        }
        public void StopLearning()
        {
            if (learning.running)
            {
                learning.stop = true;
                //while (learning.running) ;
                //learning.stop = false;
            }
        }
        public double GetLastError()
        {
            return learning.lastErrorRate;
        }

        public double SetLetter(Letter l) //ATTENZIONE: ritorna l'errore
        {
            for (int c = 0; c < l.pixelsMatrix.Length; c++)
                neurons[0][c].output = l.pixelsMatrix[c];

            for (int layer = 1; layer < neurons.Length - 1; layer++)
                for (int c = 0; c < neurons[layer].Count - 1; c++)
                    neurons[layer][c].CalculateOutput();

            int output = neurons.Length - 1;
            neurons[output][0].CalculateOutput();
            return value != l.value ? neurons[output][0].output * neurons[output][0].output / 2 : (1 - neurons[output][0].output) * (1 - neurons[output][0].output) / 2;
        }
        public double SetLearningLetter(Letter l) //ATTENZIONE: ritorna l'errore
        {
            for (int c = 0; c < l.pixelsMatrix.Length; c++)
                neurons[0][c].learningOutput = l.pixelsMatrix[c];

            for (int layer = 1; layer < neurons.Length - 1; layer++)
                for (int c = 0; c < neurons[layer].Count - 1; c++)
                    neurons[layer][c].CalculateLearningOutput();

            int output = neurons.Length - 1;
            neurons[output][0].CalculateLearningOutput();
            return value != l.value ? neurons[output][0].learningOutput * neurons[output][0].learningOutput / 2 : (1 - neurons[output][0].learningOutput) * (1 - neurons[output][0].learningOutput) / 2;
        }

        public double Learn(Letter l, double n)
        {
            int output = neurons.Length - 1;
            double err = SetLearningLetter(l);
            
            if (value == l.value)
                neurons[output][0].CalculateDerivatives(1);
            else
                neurons[output][0].CalculateDerivatives(0);

            for (int layer = neurons.Length - 2; layer > 0; layer--)
                for (int c = 0; c < neurons[layer].Count - 1; c++)
                    neurons[layer][c].CalculateDerivatives();

            for (int layer = 1; layer < neurons.Length; layer++)
                for (int c = 0; c < neurons[layer].Count - 1; c++)
                    neurons[layer][c].UpdateLearningWeights(n);

            return err;
        }
        public void UpdateWeights()
        {
            for (int layer = 1; layer < neurons.Length; layer++)
                for (int c = 0; c < neurons[layer].Count - 1; c++)
                    neurons[layer][c].ConfirmLearningWeights();
        }

        public double GetOutput()
        {
            return neurons[neurons.Length - 1][0].output;
        }
        public double GetLearningOutput()
        {
            return neurons[neurons.Length - 1][0].learningOutput;
        }

        public string Serialize()
        {
            string serialized = "";
            for (int layer = 1; layer < neurons.Length; layer++)
                for (int c = 0; c < neurons[layer].Count; c++)
                    serialized += neurons[layer][c].Serialize() + "\n";
            return serialized;
        }
        public void Load(string[] data)
        {
            int layer = 1, index = 0;

            for (int c = 0; c < data.Length; c++)
            {
                neurons[layer][index].Load(data[c]);
                index++;
                if (index == neurons[layer].Count)
                {
                    index = 0;
                    layer++;
                }
            }
        }
    }
}
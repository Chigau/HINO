using System;
using System.Collections.Generic;

namespace HardsubIsNotOk
{
    class LearningThread
    {
        public bool stat = false;
        public bool running;
        public bool stop = false;
        //public List<Letter> confusions = new List<Letter>();
        public double lastErrorRate = 0;
        private double errorRate = 0;
        Network nn;
        double rate;

        public LearningThread(Network nn, double rate)
        {
            this.nn = nn;
            this.rate = rate;
        }
        public void Learn()
        {
            Console.WriteLine("starting thread: " + nn.value);

            running = true;
            stop = false;
            while (!stop)
            {
                errorRate = 0;
                int c = 0;
                List<Letter> remainingLetters = new List<Letter>(Program.examples);
                while (remainingLetters.Count > 0)
                {
                    int n = (int)(Program.rand.NextDouble() * remainingLetters.Count);
                    if (nn.value == "l")
                    {
                    }
                    if (remainingLetters[n].value == nn.value)
                        for (int i = 0; i < Program.neuralNetwork.Count; i++)
                        {
                            c++;
                            errorRate += nn.Learn(remainingLetters[n], rate);
                        }
                    else
                    {
                        c++;
                        errorRate += nn.Learn(remainingLetters[n], rate);
                    }

                    remainingLetters.RemoveAt(n);
                }
                //if (stat)
                //Console.WriteLine("Letter " + nn.value + ": " + (errorRate / c));
                nn.UpdateWeights();
                lastErrorRate = errorRate;
            }
            Console.WriteLine("ending thread " + nn.value);
            running = false;
        }
        /*
        public void LearnConfusions()
        {
            Console.WriteLine("starting thread " + Letter.IntToChar(nn.value));
            running = true;
            while (!stop)
            {
                errorRate = 0;
                int c = 0;
                List<Letter> remainingLetters = new List<Letter>(confusions);
                while (remainingLetters.Count > 0)
                {
                    int n = (int)(Program.rand.NextDouble() * remainingLetters.Count);
                    for (int i = 0; i < Program.numberOfLetters; i++)
                    {
                        c++;
                        errorRate += nn.Learn(remainingLetters[n], rate);
                    }
                    remainingLetters.RemoveAt(n);
                }
                remainingLetters = new List<Letter>(Program.numbers);
                while (remainingLetters.Count > 0)
                {
                    int n = (int)(Program.rand.NextDouble() * remainingLetters.Count);
                    c++;
                    errorRate += nn.Learn(remainingLetters[n], rate);
                    remainingLetters.RemoveAt(n);
                }


                if (stat)
                    Console.WriteLine("Letter " + Letter.IntToChar(nn.value) + ": " + (errorRate / c));
            }
            Console.WriteLine("ending thread " + Letter.IntToChar(nn.value));
            running = false;
        }
        */
    }
}
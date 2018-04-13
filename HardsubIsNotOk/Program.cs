using System;
using AForge.Video.FFMPEG;
using System.Drawing;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Text;
using System.Globalization;

namespace HardsubIsNotOk
{
    static class Settings
    {
        public static string dictionaryPath = "it_dictionary.txt";
        public static string properNamesDictionaryPath = "proper_names_dictionary.txt";
        public static string saveSubsPath = "";
        public static int nnSize = 25;
        public static int maxLearningThreads = 4;
        //public static string fileName = "";

        public static int cutTop = -1, cutBottom = -1;
        public static int minSpaceWidth = 7;
        public static int charDistance = 2;
        public static int lineDistance = 10;
        public static int minCharPixelSize = 10;
        public static int maxCharPixelSize = 1000;
        public static float defaultCharScale = 1;

        public static float newCharacterThreshold = 5;
        public static float sameCharacterThreshold = 50;
        public static float outlineThreshold = 20;
        public static int outlineWidth = 2;

        public static Color subColor = Color.White, outSubtitleColor = Color.Black;
        public static int minSubLength = 8; //in frames

        public static float maxError = 0.075f;
        public static float minCorrectness = 0.35f; //in centesimi
        public static float maxDictionaryError = 0.15f;
        public static float minDictionaryCorrectness = 0.05f; //in centesimi

        public static float frameRate = 23.976f;
        //public static float frameRate = 30;
        public static float sameSubRange = 0.98f; //in centesimi

        public static bool whiteAndBlack = false;
        public static bool dictionaryMode = false;
        public static bool ignoreTopSubtitles = false;
        public static bool discardNonPassingThroughTheCenterLines = true;
        public static bool discardNonCenteredLines = true;
        public static int nonCenteredThreshold = 20;

        public static bool learningDisabled = false;
    }
    static class Program
    {
        public static Random rand = new Random();
        public static Dictionary<string, Network> neuralNetwork = new Dictionary<string, Network>();
        public static List<string> learningThreads = new List<string>();
        public static List<string> dictionary, namesDictionary;

        public static Dictionary<string, VideoFileReader> videos = new Dictionary<string, VideoFileReader>();
        public static List<Letter> examples = new List<Letter>();

        [STAThread]
        static void Main()
        {
            dictionary = new List<string>(File.ReadAllLines(Settings.dictionaryPath));
            dictionary.Sort();
            namesDictionary = new List<string>(File.ReadAllLines(Settings.properNamesDictionaryPath));
            namesDictionary.Sort();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("it-IT");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
        public static bool FindWord(string word)
        {
            int index;
            int min = 0, max = dictionary.Count;
            while (max - min > 1)
            {
                index = min + (max - min) / 2;
                int compare = string.Compare(dictionary[index], word, true);

                if (compare < 0)
                    min = index;
                else if (compare > 0)
                    max = index;
                else
                    return true;
            }
            return word == dictionary[min] || word == dictionary[max];
        }
        public static bool FindName(string word)
        {
            if (char.IsLower(word[0]))
                return false;
            int index;
            int min = 0, max = namesDictionary.Count;
            while (max - min > 1)
            {
                index = min + (max - min) / 2;
                int compare = string.Compare(namesDictionary[index], word, true);

                if (compare < 0)
                    min = index;
                else if (compare > 0)
                    max = index;
                else
                    return true;
            }
            return word == namesDictionary[min] || word == namesDictionary[max];
        }

        public static void AddToDictionary(string word)
        {
            word = word.ToLower();
            int index = 0;
            int min = 0, max = dictionary.Count;
            while (max - min > 1)
            {
                index = min + (max - min) / 2;
                int compare = string.Compare(dictionary[index], word);

                if (compare < 0)
                    min = index;
                else if (compare > 0)
                    max = index;
                else
                    return;
            }
            int c = string.Compare(word, dictionary[index]);
            if (c < 0)
                dictionary.Insert(index, word);
            else
                dictionary.Insert(index + 1, word);
        }
        public static void AddToProperNames(string word)
        {
            word = word.ToLower();
            int index = 0;
            int min = 0, max = namesDictionary.Count;
            while (max - min > 1)
            {
                index = min + (max - min) / 2;
                int compare = string.Compare(namesDictionary[index], word);

                if (compare < 0)
                    min = index;
                else if (compare > 0)
                    max = index;
                else
                    return;
            }
            int c = string.Compare(word, namesDictionary[index]);
            if (c < 0)
                namesDictionary.Insert(index, word);
            else
                namesDictionary.Insert(index + 1, word);
        }
        static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
        /*
        static int threadIndex = 0;
        public static void AddLearningThread(string l)
        {
            if (l == "" || learningThreads.Contains(l))
                return;

            if (learningThreads.Count < Settings.maxLearningThreads - 1)
            {
                learningThreads.Add(l);
                threadIndex++;
            }
            else if ((learningThreads.Count == Settings.maxLearningThreads - 1))
            {
                learningThreads.Add(l);
                foreach (string ln in learningThreads)
                    neuralNetwork[ln].StartLearning();
            }
            else
            {
                threadIndex = threadIndex == learningThreads.Count - 1 ? 0 : threadIndex + 1;

                neuralNetwork[learningThreads[threadIndex]].StopLearning();
                learningThreads[threadIndex] = l;
                neuralNetwork[learningThreads[threadIndex]].StartLearning();
            }
        }
        */

        public static void AddLearningThread(string l)
        {
            if (l == null || learningThreads.Contains(l))
                return;
            if (learningThreads.Count < Settings.maxLearningThreads - 1)
            {
                learningThreads.Add(l);
                Console.WriteLine("Adding: " + l);
            }
            else if (learningThreads.Count == Settings.maxLearningThreads - 1)
            {
                learningThreads.Add(l);
                Console.WriteLine("Adding: " + l + ", starting");
                foreach (string ln in learningThreads)
                    neuralNetwork[ln].StartLearning();
            }
            else
            {
                float min = float.MaxValue;
                string toReplace = "";
                foreach (string ln in learningThreads)
                {
                    float err = neuralNetwork[ln].GetLastError();
                    if (err < min)
                    {
                        min = err;
                        toReplace = ln;
                    }
                }
                neuralNetwork[toReplace].StopLearning();
                learningThreads.Remove(toReplace);

                neuralNetwork[l].StartLearning();
                learningThreads.Add(l);
                Console.WriteLine("Adding: " + l + " Removing" + toReplace);
            }
        }
        public static void AddLearningThread(string l1, string l2)
        {
            if (l1 == null || learningThreads.Contains(l1))
            {
                AddLearningThread(l2);
                return;
            }
            if (l2 == null || learningThreads.Contains(l2))
            {
                AddLearningThread(l1);
                return;
            }
            if (learningThreads.Count < Settings.maxLearningThreads)
            {
                AddLearningThread(l1);
                AddLearningThread(l2);
                return;
            }

            float min1 = float.MaxValue;
            float min2 = float.MaxValue;
            string toReplace1 = "";
            string toReplace2 = "";

            foreach (string ln in learningThreads)
            {
                float err = neuralNetwork[ln].GetLastError();
                if (err < min1)
                {
                    min2 = min1;
                    toReplace2 = toReplace1;

                    min1 = err;
                    toReplace1 = neuralNetwork[ln].value;
                }
                else
                {
                    min2 = err;
                    toReplace2 = neuralNetwork[ln].value;
                }
            }
            neuralNetwork[toReplace1].StopLearning();
            learningThreads.Remove(toReplace1);
            neuralNetwork[l1].StartLearning();
            learningThreads.Add(l1);
            Console.WriteLine("Adding: " + l1 + " Removing" + toReplace1);

            neuralNetwork[toReplace2].StopLearning();
            learningThreads.Remove(toReplace2);
            neuralNetwork[l2].StartLearning();
            learningThreads.Add(l2);
            Console.WriteLine("Adding: " + l2 + " Removing" + toReplace2);
        }
        public static void StopLearning()
        {
            while(learningThreads.Count > 0)
            {
                neuralNetwork[learningThreads[0]].StopLearning();
                learningThreads.RemoveAt(0);
            }
        }
    }
}

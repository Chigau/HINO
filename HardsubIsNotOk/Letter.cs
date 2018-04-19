using System;
using System.Collections.Generic;
using System.Drawing;

namespace HardsubIsNotOk
{
    public class Letter
    {
        public string value, secondChoice;
        public float error = 1;
        public float firstOverSecondCorrectness = 1;
        public float[] pixelsMatrix = new float[24*24];
        public HashSet<Coord> pixels = new HashSet<Coord>();
        public HashSet<Coord> outlinePixels = new HashSet<Coord>();
        public int xMax = 0, xMin = int.MaxValue, yMax = 0, yMin = int.MaxValue;
        public Letter() { }
        public Letter(Letter from)
        {
            value = from.value;
            pixelsMatrix = (float[])from.pixelsMatrix.Clone();
        }
        public void AddPixel(Coord coord)
        {
            pixels.Add(coord);
            if (coord.x > xMax)
                xMax = coord.x;
            if (coord.x < xMin)
                xMin = coord.x;

            if (coord.y > yMax)
                yMax = coord.y;
            if (coord.y < yMin)
                yMin = coord.y;
        }
        public Bitmap ArrayToBitmap()
        {
            Bitmap toRet = new Bitmap(24, 24);
            for(int x = 0; x < 24; x++)
            {
                for(int y = 0; y < 24; y++)
                {
                    if(pixelsMatrix[y * 24 + x] == 1)
                        toRet.SetPixel(x, y, Color.Black);
                }
            }
            return toRet;
        }
        public void GenerateArray()
        {
            pixelsMatrix = new float[24 * 24];
            int width = xMax - xMin;
            int height = yMax - yMin;
            if(height > width)
            {
                if(height * Settings.defaultCharScale > 23)
                {
                    float scaleConst = (float) 23 / height;
                    foreach(Coord p in pixels)
                    {
                        int x = RoundToInt((p.x - xMin) * scaleConst);
                        int y = RoundToInt((p.y - yMin) * scaleConst);
                        pixelsMatrix[y * 24 + x] = 1;
                    }
                }
                else
                {
                    foreach (Coord p in pixels)
                    {
                        int x = RoundToInt((p.x - xMin) * Settings.defaultCharScale);
                        int y = RoundToInt((p.y - yMin) * Settings.defaultCharScale);
                        pixelsMatrix[y * 24 + x] = 1;
                    }
                }
            }
            else
            {
                if (width * Settings.defaultCharScale > 23)
                {
                    float scaleConst = (float) 23 / width;
                    foreach (Coord p in pixels)
                    {
                        int x = RoundToInt((p.x - xMin) * scaleConst);
                        int y = RoundToInt((p.y - yMin) * scaleConst);
                        pixelsMatrix[y * 24 + x] = 1;
                    }
                }
                else
                {
                    foreach (Coord p in pixels)
                    {
                        int x = RoundToInt((p.x - xMin) * Settings.defaultCharScale);
                        int y = RoundToInt((p.y - yMin) * Settings.defaultCharScale);
                        pixelsMatrix[y * 24 + x] = 1;
                    }
                }
            }
        }
        static int RoundToInt(float n)
        {
            int result = (int)n;
            return n - result < 0.5 ? result : result + 1;
        }
        public void RecalcCoords()
        {
            xMax = 0; xMin = int.MaxValue; yMax = 0; yMin = int.MaxValue;
            foreach (Coord coord in pixels)
            {
                if (coord.x > xMax)
                    xMax = coord.x;
                if (coord.x < xMin)
                    xMin = coord.x;

                if (coord.y > yMax)
                    yMax = coord.y;
                if (coord.y < yMin)
                    yMin = coord.y;
            }
        }
        public void Recognize()
        {
            if (Program.neuralNetwork.Count < Settings.maxLearningThreads)
                return;
            float higher = 0;
            float err = 0;
            //Console.WriteLine("Riconoscimento lettera");
            List<string> keys = new List<string>(Program.neuralNetwork.Keys);

            foreach (String s in keys)
            {
                Program.neuralNetwork[s].SetLetter(this);
                float output = Program.neuralNetwork[s].GetOutput();
                err += output * output;
                if (output > higher)
                {
                    firstOverSecondCorrectness = higher;
                    secondChoice = value;

                    higher = output;
                    value = s;
                }
                else if (output > firstOverSecondCorrectness)
                {
                    firstOverSecondCorrectness = output;
                    secondChoice = s;
                }
                //Console.WriteLine("Caso " + nn.value + ": " + output);
            }
            firstOverSecondCorrectness = higher - firstOverSecondCorrectness;

            err -= higher * higher;
            err += (1 - higher) * (1 - higher);
            err /= Program.neuralNetwork.Count;
            //Console.WriteLine("Risultato: " + to);
            error = (float)Math.Sqrt(err);
        }
    }
    public class Space : Letter { }
    public struct Coord
    {
        public int x, y;
        public Coord Top
        {
            get { return new Coord(x, y - 1); }
        }
        public Coord Bottom
        {
            get { return new Coord(x, y + 1); }
        }
        public Coord Right
        {
            get { return new Coord(x + 1, y); }
        }
        public Coord Left
        {
            get { return new Coord(x - 1, y); }
        }
        public Coord TopRight
        {
            get { return new Coord(x + 1, y - 1); }
        }
        public Coord TopLeft
        {
            get { return new Coord(x - 1, y - 1); }
        }
        public Coord BottomRight
        {
            get { return new Coord(x + 1, y + 1); }
        }
        public Coord BottomLeft
        {
            get { return new Coord(x - 1, y + 1); }
        }
        public Coord(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public Coord(Point p)
        {
            x = p.X;
            y = p.Y;
        }
        public override bool Equals(object obj)
        {
            Coord o = (Coord) obj;
            return o.x == x && o.y == y;
        }
        public override int GetHashCode()
        {
            return x * 10000 + y;
        }
        public override string ToString()
        {
            return x + "," + y;
        }
    }
}
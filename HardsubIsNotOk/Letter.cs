using System;
using System.Collections.Generic;
using System.Drawing;

namespace HardsubIsNotOk
{
    public class Letter
    {
        public string value, secondChoice;
        public double correctness = 1;
        public double[] pixelsMatrix = new double[24*24];
        public HashSet<Coord> pixels = new HashSet<Coord>();
        public HashSet<Coord> outlinePixels;
        public int xMax = 0, xMin = int.MaxValue, yMax = 0, yMin = int.MaxValue;
        public Letter() { }
        public Letter(Letter from)
        {
            value = from.value;
            pixelsMatrix = (double[])from.pixelsMatrix.Clone();
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
        /*
        public Bitmap ToBitmap()
        {
            Bitmap toRet = new Bitmap(xMax - xMin + 1, yMax - yMin + 1);
            foreach (Coord c in pixels)
            {
                toRet.SetPixel(c.x - xMin, c.y - yMin, Color.Black);
            }
            return toRet;
        }
        */
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
            pixelsMatrix = new double[24 * 24];
            int width = xMax - xMin;
            int height = yMax - yMin;
            if(height > width)
            {
                if(height * Settings.defaultCharScale > 24)
                {
                    double scaleConst = (double) 23 / height;
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
                if (width * Settings.defaultCharScale > 24)
                {
                    double scaleConst = (double) 23 / width;
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
        static int RoundToInt(double n)
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
        public Coord[] Edge
        {
            get { return new Coord[] { Top, Bottom, Left, Right, Top.Left, Top.Right, Bottom.Left, Bottom.Right }; }
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
        public static double Distance(Coord c1, Coord c2)
        {
            return Math.Sqrt(Math.Pow(c1.x - c2.x, 2) + Math.Pow(c1.y - c2.y, 2));
        }
    }
}
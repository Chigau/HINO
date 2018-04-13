using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardsubIsNotOk
{
    public class Subtitle
    {
        public bool top = false;
        public List<Line> lines = new List<Line>();
        public List<Coord> discardedPixels = new List<Coord>();
        public long startFrame = -1, endFrame = -1;
        public string value = "";

        private List<int> SubtitleMinHeight = new List<int>(), SubtitleMaxHeight = new List<int>();
        public void AddLetter(Letter l1)
        {
            if (l1 == null)
                return;
            int index = WhatLine(l1);
            if (index == -1)
            {
                Line newLine = new Line();
                newLine.AddLetter(l1);

                lines.Add(newLine);
            }
            else
            {
                Letter l2 = lines[index].Last;
                int min, max;

                int minX = int.MaxValue; //checking distance between two letters
                if (l1.xMin - l2.xMax < 2) 
                {
                    foreach(Coord c1 in l1.pixels)
                        foreach (Coord c2 in l2.pixels)
                        {
                            int distX = c1.x - c2.x;
                            int distYAbs = Math.Abs(c1.y - c2.y);
                            if (Math.Abs(distX) + distYAbs < 3) //parametrizzabile?
                            {
                                foreach (Coord c in l1.pixels)
                                    l2.AddPixel(c);
                                lines[index].Update(l2);
                                return;
                            }
                            if (distYAbs < 2 && distX < minX)
                                minX = distX;
                        }
                }
                if (minX > Settings.charDistance && minX != int.MaxValue)
                {
                    lines[index].AddLetter(l1);
                    return;
                }
                
                
                //DA RIVEDERE
                if (l1.xMax - l1.xMin < l2.xMax - l2.xMin)
                {
                    l1.xMin--;
                    l1.xMax++;
                }
                else
                {
                    l2.xMin--;
                    l2.xMax++;
                }
                //----------

                min = l1.xMin > l2.xMin ? l1.xMin : l2.xMin;
                max = l1.xMax < l2.xMax ? l1.xMax : l2.xMax;

                max -= min;

                min = Math.Min(l1.xMax - l1.xMin, l2.xMax - l2.xMin);
                if (max > min / 3)
                {
                    foreach (Coord c in l1.pixels)
                        l2.AddPixel(c);
                    lines[index].Update(l2);
                }
                else
                    lines[index].AddLetter(l1);
                    
            }
        }

        public int WhatLine(Letter who)
        {
            if (lines.Count == 0)
                return -1;
            int minIndex = -1, min = Settings.lineDistance;
            for (int c = 0; c < lines.Count; c++)
            {
                int dist = lines[c].yMin < who.yMin ? who.yMin - lines[c].yMax : lines[c].yMin - who.yMax;
                if (dist < min)
                {
                    min = dist;
                    minIndex = c;
                }
            }

            return minIndex;
        }

        public bool GetEndFromBuffer(LockBitmap[] buffer)
        {
            int c;
            float ok, total;
            ok = 0; total = 0;
            foreach (Line line in lines)
                foreach (Letter l in line.letters)
                    foreach (Coord p in l.pixels)
                    {
                        total++;
                        if (ConversionThread.AreSimilar(ConversionThread.frame.GetPixel(p.x, p.y), Settings.subColor))
                            ok++;
                    }
            if (ok / total > Settings.sameSubRange)
                return false;

            for (c = buffer.Length - 2; c >= 0; c--)
            {
                ok = 0; total = 0;
                foreach (Line line in lines)
                    foreach (Letter l in line.letters)
                        foreach (Coord p in l.pixels)
                        {
                            total++;
                            if (ConversionThread.AreSimilar(buffer[c].GetPixel(p.x, p.y), Settings.subColor))
                                ok++;
                        }
                if (ok / total > Settings.sameSubRange)
                    break;
            }

            if (c < 0)
                c = 0;
            endFrame = ConversionThread.frameIndex - buffer.Length + c;
            //Console.WriteLine("Fine frame: " + endFrame);
            //Console.WriteLine("Fine secondi: " + endFrame / Settings.frameRate);
            CalculateSpaces();
            return true;
        }
        public void GetStartFromBuffer(LockBitmap[] buffer)
        {
            int c;
            float ok, total;
            for (c = buffer.Length - 2; c >= 0; c--)
            {
                ok = 0; total = 0;
                foreach (Line line in lines)
                    foreach (Letter l in line.letters)
                        foreach (Coord p in l.pixels)
                        {
                            total++;
                            if (ConversionThread.AreSimilar(buffer[c].GetPixel(p.x, p.y), Settings.subColor))
                                ok++;
                        }
                if (ok / total < Settings.sameSubRange)
                    break;
            }
            if (c < 0)
                c = 0;
            startFrame = ConversionThread.frameIndex - buffer.Length + c;
            //Console.WriteLine("Inizio frame: " + startFrame);
            //Console.WriteLine("Inizio secondi: " + startFrame / Settings.frameRate);
        }
        public void CalculateSpaces()
        {

            foreach(Line l in lines)
            {
                for(int c = 0; c < l.letters.Count - 1; c++)
                {
                    Letter l1 = l.letters[c], l2 = l.letters[c + 1];
                    if (l2.xMin - l1.xMax <= Settings.minSpaceWidth)
                    {
                        foreach (Coord c1 in l1.pixels)
                            foreach (Coord c2 in l2.pixels)
                            {
                                if (Math.Abs(c1.x - c2.x) + Math.Abs(c1.y - c2.y) < Settings.minSpaceWidth)
                                    goto checkNext;
                            }
                    }
                    c++;
                    l.letters.Insert(c, new Space());
                    checkNext:;
                }
            }
            List<Line> newLines = new List<Line>();
            while(lines.Count > 0)
            {
                Line min = new Line();
                foreach(Line l in lines)
                {
                    if(l.yMin < min.yMin)
                        min = l;
                }
                newLines.Add(min);
                lines.Remove(min);
            }
            lines = newLines;
        }
        public Bitmap GetFrame()
        {
            Bitmap toRet = new Bitmap(ConversionThread.frame.Width, ConversionThread.frame.Height);
            foreach (Line line in lines)
            {
                foreach (Letter l in line.letters)
                {
                    foreach (Coord p in l.pixels)
                        toRet.SetPixel(p.x, p.y, Color.Black);
                }
            }
            foreach (Coord p in discardedPixels)
                toRet.SetPixel(p.x, p.y, Color.LightCoral);

            return toRet;
        }

        public class Line
        {
            public List<Letter> letters = new List<Letter>();
            public Letter Last { get { return letters[letters.Count - 1]; } }
            public int yMax = 0, yMin = int.MaxValue;
            public int xMax = 0, xMin = int.MaxValue;
            public void AddLetter(Letter l)
            {
                letters.Add(l);
                if (l.yMax > yMax)
                    yMax = l.yMax;
                if (l.yMin < yMin)
                    yMin = l.yMin;

                if (l.xMax > xMax)
                    xMax = l.xMax;
                if (l.xMin < xMin)
                    xMin = l.xMin;
            }
            public void Update(Letter l)
            {
                if (l.yMax > yMax)
                    yMax = l.yMax;
                if (l.yMin < yMin)
                    yMin = l.yMin;

                if (l.xMax > xMax)
                    xMax = l.xMax;
                if (l.xMin < xMin)
                    xMin = l.xMin;
            }
            public void RecalcCoords()
            {
                xMax = 0; xMin = int.MaxValue; yMax = 0; yMin = int.MaxValue;
                foreach (Letter l in letters)
                {
                    if (l.yMax > yMax)
                        yMax = l.yMax;
                    if (l.yMin < yMin)
                        yMin = l.yMin;

                    if (l.xMax > xMax)
                        xMax = l.xMax;
                    if (l.xMin < xMin)
                        xMin = l.xMin;
                }
            }
        }
    }
}
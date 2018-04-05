using System;
using AForge.Video.FFMPEG;
using System.Drawing;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Text;

namespace HardsubIsNotOk
{
    class ConversionThread
    {
        public const int MinSubPixels = 500;

        public static LockBitmap frame;
        public static long frameIndex;
        static LockBitmap[] buffer, buffer1, buffer2;

        public static bool[,] filled;
        private static Letter newLetter;
        public static List<Subtitle> subtitles;
        private static bool notALetterFlag;
        static int bufferSize, whatBuffer = 0;
        static bool bufferingPause, bufferingComplete;

        public static void Go()
        {
            var watch = Stopwatch.StartNew();

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            subtitles = new List<Subtitle>();
            
            bufferSize = Program.reader.FrameRate / 2;

            buffer1 = new LockBitmap[bufferSize];
            buffer2 = new LockBitmap[bufferSize];

            Thread bufferThread = new Thread(Buffering);
            bufferThread.IsBackground = true;
            bufferingComplete = false;
            bufferingPause = false;
            bufferThread.Start();
            while (!bufferingPause) ;

            Subtitle subTop = null, subBottom = null;
            Thread subRec = new Thread(RecognizeSubtitle);
            subRec.IsBackground = true;
            subRec.Start();


            while (!bufferingComplete)
            {

                buffer = whatBuffer == 0 ? buffer2 : buffer1;

                bufferingPause = false;

                frameIndex += buffer.Length;
                frame = buffer[bufferSize - 1];
                if (!Settings.ignoreTopSubtitles)
                {
                    if (subTop == null)
                    {
                        subTop = GetSubtitleTop();
                        if (subTop != null)
                            subTop.GetStartFromBuffer(buffer);
                    }
                    else if (subTop.GetEndFromBuffer(buffer))
                    {
                        if(subTop.endFrame - subTop.startFrame >= Settings.minSubLength)
                            subtitles.Add(subTop);
                        subTop = GetSubtitleTop();
                        if (subTop != null)
                            subTop.GetStartFromBuffer(buffer);
                    }
                }

                if (subBottom == null)
                {
                    subBottom = GetSubtitleBottom();
                    if (subBottom != null)
                        subBottom.GetStartFromBuffer(buffer);
                }
                else if (subBottom.GetEndFromBuffer(buffer))
                {
                    if (subBottom.endFrame - subBottom.startFrame >= Settings.minSubLength)
                        subtitles.Add(subBottom);
                    subBottom = GetSubtitleBottom();
                    if (subBottom != null)
                        subBottom.GetStartFromBuffer(buffer);
                }

                while (!bufferingPause) ;
            }
            Form1.progressBar.Invoke(new Form1.EventHandle(() => Form1.progressBar.Value = 1000));

            stopWatch.Stop();
            Console.WriteLine("-------------TEMPO DI ESTRAZIONE-----------");
            Console.WriteLine(stopWatch.Elapsed);
        }
        public static void Buffering()
        {
            Bitmap b;
            int bufferIndex;
            start:

            bufferIndex = 0;
            if (whatBuffer == 0)
            {
                while ((b = Program.reader.ReadVideoFrame()) != null)
                {
                    try
                    {
                        buffer1[bufferIndex].source.Dispose();
                    }
                    catch { }
                    buffer1[bufferIndex] = new LockBitmap(b);
                    buffer1[bufferIndex].LockBits();
                    bufferIndex++;
                    //frameIndex++;
                    if (frameIndex % 50 == 0 && Program.reader.FrameCount != 0)
                        Form1.progressBar.Invoke(new Form1.EventHandle(() => Form1.progressBar.Value = (int)(((float)frameIndex / Program.reader.FrameCount) * 1000)));
                    if (bufferIndex == bufferSize)
                    {
                        whatBuffer = 1;
                        bufferingPause = true;
                        while (bufferingPause) ;
                        goto start;
                    }
                }
            }
            else
            {
                while ((b = Program.reader.ReadVideoFrame()) != null)
                {
                    try
                    {
                        buffer2[bufferIndex].source.Dispose();
                    }
                    catch { }
                    buffer2[bufferIndex] = new LockBitmap(b);
                    buffer2[bufferIndex].LockBits();
                    bufferIndex++;
                    //frameIndex++;
                    if (frameIndex % 50 == 0 && Program.reader.FrameCount != 0)
                        Form1.progressBar.Invoke(new Form1.EventHandle(() => Form1.progressBar.Value = (int)(((float)frameIndex / Program.reader.FrameCount) * 1000)));
                    if (bufferIndex == bufferSize)
                    {
                        whatBuffer = 0;
                        bufferingPause = true;
                        while (bufferingPause) ;
                        goto start;
                    }
                }
            }
            bufferingPause = true;
            bufferingComplete = true;
        }

        static int subIndex = 0;
        public static void RecognizeSubtitle()
        {
            while(true)
            {
                while (subtitles.Count <= subIndex)
                {
                    if(frameIndex == Program.reader.FrameCount)
                    {
                        Form1.progressBar.Invoke(new Form1.EventHandle(() => Form1.recognitionBar.Value = 1000));
                        Program.StopLearning();
                        SubtitlesWindow w = new SubtitlesWindow();
                        w.Show();
                        return;
                    }
                }

                Form1.progressBar.Invoke(new Form1.EventHandle(() => Form1.recognitionBar.Value = (int)(((double)subIndex / subtitles.Count) * 1000)));

                //Thread.Sleep(10);
                string converted = "";
                int wordStart = 0, wordEnd = 0;

                foreach (Subtitle.Line line in subtitles[subIndex].lines)
                {
                    for (int c = 0; c < line.letters.Count; c++)
                    {
                        if (line.letters[c] is Space)
                        {
                            CorrectWord(subtitles[subIndex], subtitles[subIndex].lines.IndexOf(line), wordStart, wordEnd, ref converted);
                            if (converted == null)
                                goto skipSub;
                            converted += ' ';
                            wordEnd++;
                            wordStart = wordEnd;
                            continue;
                        }
                        line.letters[c].GenerateArray();
                        string guess, secondChoice;
                        double corr = 0;
                        double err = Convert(line.letters[c], out guess, out secondChoice, out corr);
                        if (corr < 0.5) //parametrizzabile
                        {
                            line.letters[c].secondChoice = secondChoice;
                            line.letters[c].correctness = corr;
                        }
                        if(Settings.dictionaryMode ? err > Settings.maxDictionaryError || corr < Settings.minDictionaryCorrectness : err > Settings.maxError || corr < Settings.minCorrectness)
                        {
                            GuessLetter alert = new GuessLetter(subtitles[subIndex], line.letters[c], "Recognition: " + guess + "\nAverage error: " + err + "\nSecond choice: " + secondChoice + "\nFirst over second choice correctness: " + (int)(corr * 100) + "%\nFrame: " + subtitles[subIndex].startFrame + "-" + subtitles[subIndex].endFrame);
                            alert.StartPosition = FormStartPosition.CenterScreen;
                            alert.ShowDialog();

                            switch(alert.result)
                            {
                                case GuessLetter.Result.incorrect:
                                    if (alert.correction == guess)
                                        goto case GuessLetter.Result.correct;
                                    line.letters[c].value = alert.correction;
                                    Program.examples.Add(line.letters[c]);

                                    bool newNet = !Program.neuralNetwork.ContainsKey(alert.correction);
                                    if (newNet)
                                        Program.neuralNetwork.Add(alert.correction, new Network(alert.correction, 24 * 24, Settings.nnSize, Settings.nnSize));

                                    Program.AddLearningThread(guess, alert.correction);

                                    guess = alert.correction;

                                    if (newNet && Program.neuralNetwork.Count >= Settings.maxLearningThreads)
                                        Thread.Sleep(1000);
                                    break;

                                case GuessLetter.Result.correct:
                                    line.letters[c].value = guess;
                                    Program.examples.Add(line.letters[c]);
                                    Program.AddLearningThread(guess, secondChoice);
                                    break;
                                    
                                case GuessLetter.Result.notALetter:
                                    guess = "";
                                    line.letters.RemoveAt(c);
                                    c--;
                                    //Program.examples.Add(l);
                                    continue;

                                case GuessLetter.Result.skipSub:
                                    subtitles.RemoveAt(subIndex);
                                    goto skipSub;

                                case GuessLetter.Result.subChanged:
                                    goto skipSub;

                                case GuessLetter.Result.subRewrited:
                                    subtitles[subIndex].value = alert.correction;
                                    subIndex++;
                                    goto skipSub;
                            }
                        }
                        else
                        {
                            line.letters[c].value = guess;
                        }
                        if (guess != "")
                        {
                            if (IsLetter(guess))
                                wordEnd++;
                            else
                            {
                                CorrectWord(subtitles[subIndex], subtitles[subIndex].lines.IndexOf(line), wordStart, wordEnd, ref converted);
                                if (converted == null)
                                    goto skipSub;
                                wordEnd++;
                                wordStart = wordEnd;
                            }
                            converted += guess;
                        }
                    }
                    CorrectWord(subtitles[subIndex], subtitles[subIndex].lines.IndexOf(line), wordStart, wordEnd, ref converted);
                    if (converted == null)
                        goto skipSub;
                    if (converted != "" && converted[converted.Length - 1] != '\n')
                        converted += '\n';
                    wordStart = 0;
                    wordEnd = 0;
                }
                if (converted == "")
                {
                    subtitles.RemoveAt(subIndex);
                }
                else if (subIndex > 0 && converted == subtitles[subIndex - 1].value && subtitles[subIndex].startFrame - 1 <= subtitles[subIndex - 1].endFrame)
                {
                    subtitles[subIndex - 1].endFrame = subtitles[subIndex].endFrame;
                    subtitles.RemoveAt(subIndex);
                }
                else
                {
                    //Console.Write(converted);
                    subtitles[subIndex].value = converted;
                    subIndex++;
                }
                skipSub:;
            }
        }

        public static bool IsLetter(string s)
        {
            foreach (char c in s)
                if (!char.IsLetter(c))
                    return false;
            return true;
        }

        public static void CorrectWord(Subtitle sub, int line, int start, int end, ref string converted)
        {
            string word = "";
            for (int c = start; c < end; c++)
                word += sub.lines[line].letters[c].value;

            if (word.Length > 0 && !Program.FindWord(word) && !Program.FindName(word))
            {
                converted = converted.Remove(converted.Length - word.Length);
                word = "";

                Dictionary<string, Letter> alternatives = new Dictionary<string, Letter>();

                for (int c = start; c < end; c++) //provo a correggere una lettera
                {
                    if (sub.lines[line].letters[c].secondChoice != null && sub.lines[line].letters[c].secondChoice != "")
                    {

                        string newWord = word + sub.lines[line].letters[c].secondChoice;
                        for (int d = c + 1; d < end; d++)
                            newWord += sub.lines[line].letters[d].value;
                        if (Program.FindWord(newWord) || Program.FindName(newWord))
                            alternatives.Add(newWord, sub.lines[line].letters[c]);
                    }
                    word += sub.lines[line].letters[c].value;
                }
                if(alternatives.Count > 0)
                {
                    double min = 1;
                    string index = "";
                    foreach(KeyValuePair<string, Letter> alt in alternatives)
                    {
                        if(alt.Value.correctness < min)
                        {
                            min = alt.Value.correctness;
                            index = alt.Key;
                        }
                    }
                    string wrong = alternatives[index].value;
                    string corrected = alternatives[index].secondChoice;

                    alternatives[index].value = corrected;

                    Program.examples.Add(alternatives[index]);
                    Program.AddLearningThread(wrong, corrected);
                    converted += index;
                    return;
                }

                WordNotFound alert = new WordNotFound(sub, line, start, end);
                alert.ShowDialog();

                switch (alert.result)
                {
                    case WordNotFound.Result.correct:
                        string newWord = "";
                        for (int c = start; c < end; c++)
                        {
                            string corrected = alert.correction[c - start].Text;
                            newWord += corrected;
                            if (sub.lines[line].letters[c].value != corrected)
                            {
                                string wrong = sub.lines[line].letters[c].value;
                                sub.lines[line].letters[c].value = corrected;
                                Program.examples.Add(sub.lines[line].letters[c]);

                                bool newNet = !Program.neuralNetwork.ContainsKey(corrected);
                                if (newNet)
                                    Program.neuralNetwork.Add(corrected, new Network(corrected, 24 * 24, Settings.nnSize, Settings.nnSize));

                                Program.AddLearningThread(wrong, corrected);
                            }
                        }
                        converted += newWord;
                        break;
                    case WordNotFound.Result.correctWithoutLearning:
                        string n = "";
                        for (int c = start; c < end; c++)
                        {
                            string corrected = alert.correction[c - start].Text;
                            n += corrected;

                        }
                        converted += n;
                        break;
                    case WordNotFound.Result.add:
                        converted += word;
                        Program.AddToDictionary(word);
                        break;
                    case WordNotFound.Result.properName:
                        converted += word;
                        Program.AddToProperNames(word);
                        break;
                    case WordNotFound.Result.skip:
                        converted += word;
                        break;
                    case WordNotFound.Result.skipSub:
                        subtitles.RemoveAt(subIndex);
                        converted = null;
                        break;
                    case WordNotFound.Result.subChanged:
                        converted = null;
                        break;
                    case WordNotFound.Result.subRewrited:
                        sub.value = alert.subRewrited;
                        subIndex++;
                        converted = null;
                        break;
                }

            }
        }

        public static double Convert(Letter l, out string to, out string secondChoice, out double perc) //ritorna l'errore
        {
            to = "";
            secondChoice = "";
            perc = 0;
            if (Program.neuralNetwork.Count < Settings.maxLearningThreads)
                return 1;
            double higher = 0;
            double err = 0;
            //Console.WriteLine("Riconoscimento lettera");
            foreach (Network nn in Program.neuralNetwork.Values)
            {
                //bool restart = nn.learning.running;
                //nn.StopLearning();
                nn.SetLetter(l);
                double output = nn.GetOutput();
                err += output * output;
                if (output > higher)
                {
                    perc = higher;
                    secondChoice = to;

                    higher = output;
                    to = nn.value;
                }
                else if (output > perc)
                {
                    perc = output;
                    secondChoice = nn.value;
                }
                //Console.WriteLine("Caso " + nn.value + ": " + output);
                //if(restart)
                //    nn.StartLearning();
            }
            perc = higher - perc;

            err -= higher * higher;
            err += (1 - higher) * (1 - higher);
            err /= Program.neuralNetwork.Count;
            //Console.WriteLine("Risultato: " + to);
            return Math.Sqrt(err);
        }

        private static Subtitle GetSubtitleBottom()
        {
            filled = new bool[frame.Width, frame.Height];
            Subtitle subtitle = null;
            int x, y;
            for (x = 0; x < frame.Width; x += 2)
            {
                for (y = Settings.cutBottom; y < frame.Height; y += 2)
                {
                    if (!IsFilled(new Coord(x, y)) && AreSimilar(frame.GetPixel(x, y), Settings.subColor, Settings.newCharacterThreshold)) //FIX BASSA PRIORITA': ignora la colonna subito a sx nel filling 
                    {
                        Letter get = GetLetter(x, y);
                        if (get != null)
                        {
                            if (subtitle == null)
                                subtitle = new Subtitle();
                            if (get.pixels.Count < Settings.maxCharPixelSize)
                                subtitle.AddLetter(get);
                            else
                                subtitle.discardedPixels.AddRange(get.pixels);
                        }
                    }
                }
            }
            if (subtitle != null)
                ValidateSub(ref subtitle);

            return subtitle;
        }
        private static Subtitle GetSubtitleTop()
        {
            filled = new bool[frame.Width, frame.Height];
            Subtitle subtitle = null;
            int x, y;
            for (x = 0; x < frame.Width; x += 2)
            {
                for (y = 0; y < Settings.cutTop; y += 2)
                {
                    if (!IsFilled(new Coord(x, y)) && AreSimilar(frame.GetPixel(x, y), Settings.subColor, Settings.newCharacterThreshold)) //FIX BASSA PRIORITA': ignora la colonna subito a sx nel filling 
                    {
                        Letter get = GetLetter(x, y);
                        if (get != null)
                        {
                            if (subtitle == null)
                                subtitle = new Subtitle()
                                {
                                    top = true
                                };
                            subtitle.AddLetter(get);
                        }
                    }
                }
            }
            if (subtitle != null)
                ValidateSub(ref subtitle);

            return subtitle;
        }

        public static void ValidateSub(ref Subtitle subtitle)
        {
            for (int l = 0; l < subtitle.lines.Count; l++)
            {
                for (int i = 0; i < subtitle.lines[l].letters.Count; i++)
                {
                    if (subtitle.lines[l].letters[i].pixels.Count < Settings.minCharPixelSize) //|| subtitle.lines[l].letters[i].pixels.Count > Settings.maxCharPixelSize)
                    {
                        subtitle.discardedPixels.AddRange(subtitle.lines[l].letters[i].pixels);
                        subtitle.lines[l].letters.RemoveAt(i);
                        if (subtitle.lines[l].letters.Count != 0)
                            subtitle.lines[l].RecalcCoords();
                        else
                        {
                            subtitle.lines.RemoveAt(l);
                            l--;
                            goto next;
                        }
                    }
                }

                if (Settings.discardNonCenteredLines)
                {
                    while (Math.Abs(frame.Width - subtitle.lines[l].xMax - subtitle.lines[l].xMin) > Settings.nonCenteredThreshold)
                    {
                        if (subtitle.lines[l].letters.Count < 2)
                        {
                            subtitle.lines.RemoveAt(l);
                            l--;
                            break;
                        }
                        else if (frame.Width - subtitle.lines[l].xMax > subtitle.lines[l].xMin)
                        {
                            subtitle.discardedPixels.AddRange(subtitle.lines[l].letters[0].pixels);
                            subtitle.lines[l].letters.RemoveAt(0);
                            subtitle.lines[l].RecalcCoords();
                        }
                        else
                        {
                            subtitle.discardedPixels.AddRange(subtitle.lines[l].letters[subtitle.lines[l].letters.Count - 1].pixels);
                            subtitle.lines[l].letters.RemoveAt(subtitle.lines[l].letters.Count - 1);
                            subtitle.lines[l].RecalcCoords();
                        }
                    }
                    if (subtitle.lines.Count == 0)
                    {
                        subtitle = null;
                        break;
                    }
                }
                else if (Settings.discardNonPassingThroughTheCenterLines)
                {
                    if (subtitle.lines[l].xMax < frame.Width / 2 || subtitle.lines[l].xMin > frame.Width / 2)
                    {
                        subtitle.lines.RemoveAt(l);
                        if (subtitle.lines.Count == 0)
                        {
                            subtitle = null;
                            break;
                        }
                        l--;
                    }
                }
                next:;
            }
        }

        // distance between two hues:
        static double GetHueDistance(double hue1, double hue2)
        {
            double d = Math.Abs(hue1 - hue2);
            return d > 180 ? 360 - d : d;
        }
        // distance in RGB space
        static int ColorDiff(Color c1, Color c2)
        {
            return (int)Math.Sqrt((c1.R - c2.R) * (c1.R - c2.R)
                                   + (c1.G - c2.G) * (c1.G - c2.G)
                                   + (c1.B - c2.B) * (c1.B - c2.B));
        }

        public static bool AreSimilar(Color c1, Color c2, float threshold = 80)
        {
            return ColorDiff(c1, c2) < threshold;
        }
        private static bool IsValid(Coord coord)
        {
            if (coord.x < 0 || coord.y < 0 || coord.x >= frame.Width || coord.y >= frame.Height || (coord.y < Settings.cutBottom && coord.y > Settings.cutTop))
            {
                notALetterFlag = true;
                return false;
            }
            Color c1 = frame.GetPixel(coord.x, coord.y);
            Color c2 = Settings.subColor;
            bool valid = ColorDiff(c1, c2) < Settings.sameCharacterThreshold;

            if (!valid)
            {
                if (Settings.whiteAndBlack)
                    CheckEdgesWB(coord);
                else
                    CheckEdges(coord);
            }

            return valid;
        }
        static float idealHue = Settings.outSubtitleColor.GetHue();
        static double diff;
        static int tot;
        static void CheckEdges(Coord coord, int distance = 0)
        {
            if (!IsFilled(coord))
            {
                if (coord.x >= 0 && coord.y >= 0 && coord.x < frame.Width && coord.y < frame.Height && ColorDiff(frame.GetPixel(coord.x, coord.y), Settings.subColor) >= Settings.sameCharacterThreshold)
                {
                    filled[coord.x, coord.y] = true;
                    if (distance < Settings.outlineWidth)
                        foreach (Coord c in coord.Edge)
                            CheckEdges(c, distance + 1);

                    double hue = frame.GetPixel(coord.x, coord.y).GetHue();
                    diff += GetHueDistance(idealHue, hue);
                    tot++;
                }
            }
        }
        static void CheckEdgesWB(Coord coord, int distance = 0)
        {
            if (!IsFilled(coord))
            {
                if (coord.x >= 0 && coord.y >= 0 && coord.x < frame.Width && coord.y < frame.Height && ColorDiff(frame.GetPixel(coord.x, coord.y), Settings.subColor) >= Settings.sameCharacterThreshold)
                {
                    filled[coord.x, coord.y] = true;
                    if (distance < Settings.outlineWidth)
                        foreach (Coord c in coord.Edge)
                            CheckEdges(c, distance + 1);

                    Color c1 = frame.GetPixel(coord.x, coord.y);
                    byte smaller = Math.Min(c1.R, Math.Min(c1.G, c1.B));
                    c1 = Color.FromArgb(255, c1.R - smaller, c1.G - smaller, c1.B - smaller);
                    Color c2 = Color.Black;
                    diff += ColorDiff(c1, c2);
                    tot++;
                }
            }
        }

        private static Letter GetLetter(int x, int y)
        {
            newLetter = new Letter();
            notALetterFlag = false;
            Coord cStart = new Coord(x, y);
            diff = 0; tot = 0;
            Fill(cStart);
            double letter = diff / tot;
            if (notALetterFlag || letter > Settings.outlineThreshold)
                return null;
            return newLetter;
        }
        /*
        private static void Fill(Coord point)
        {
            if (IsFilled(point) || !IsValid(point))
                return;
            filled[point.x, point.y] = true;
            newLetter.AddPixel(point);

            Fill(point.Bottom);
            Fill(point.Right);
            Fill(point.Top);
            Fill(point.Left);
        }
        */
        private static void Fill(Coord p)
        {
            Queue<Coord> q = new Queue<Coord>();
            filled[p.x, p.y] = true;
            newLetter.AddPixel(p);
            q.Enqueue(p);
            while (q.Count > 0)
            {
                p = q.Dequeue();

                if (!IsFilled(p.Top) && IsValid(p.Top))
                {
                    filled[p.Top.x, p.Top.y] = true;
                    newLetter.AddPixel(p.Top);
                    q.Enqueue(p.Top);
                }
                if (!IsFilled(p.Bottom) && IsValid(p.Bottom))
                {
                    filled[p.Bottom.x, p.Bottom.y] = true;
                    newLetter.AddPixel(p.Bottom);
                    q.Enqueue(p.Bottom);
                }
                if (!IsFilled(p.Left) && IsValid(p.Left))
                {
                    filled[p.Left.x, p.Left.y] = true;
                    newLetter.AddPixel(p.Left);
                    q.Enqueue(p.Left);
                }
                if (!IsFilled(p.Right) && IsValid(p.Right))
                {
                    filled[p.Right.x, p.Right.y] = true;
                    newLetter.AddPixel(p.Right);
                    q.Enqueue(p.Right);
                }
            }
        }
        static bool IsFilled(Coord point)
        {
            return point.x < 0 || point.x >= filled.GetLength(0) || point.y < 0 || point.y >= filled.GetLength(1) || filled[point.x, point.y];
        }

        private static void SaveWhatFilled() //DEBUG
        {
            Bitmap toSave = new Bitmap(filled.GetLength(0), filled.GetLength(1));
            for (int x = 0; x < filled.GetLength(0); x++)
            {
                for (int y = 0; y < filled.GetLength(1); y++)
                {
                    toSave.SetPixel(x, y, filled[x, y] ? Color.White : Color.Black);
                }
            }
            toSave.Save("viva_la_cacca.png");
        }
        private static void SaveLetter(Letter l, string name) //DEBUG
        {
            Bitmap toSave = new Bitmap(filled.GetLength(0), filled.GetLength(1));
            /*
            for (int x = 0; x < filled.GetLength(0); x++)
                for (int y = 0; y < filled.GetLength(1); y++)
                    toSave.SetPixel(x, y, Color.Black);
            */
            foreach (Coord c in l.pixels)
                toSave.SetPixel(c.x, c.y, Color.White);

            toSave.Save(name + ".png");
        }
        public static void SaveSub(Subtitle s, Bitmap frame)
        {
            if (s.lines.Count > 0)
            {
                string path = @"sub/Sottotitolo_DA_" + s.startFrame + "_A_" + s.endFrame;
                DirectoryInfo di = Directory.CreateDirectory(path);
                for (int c = 0; c < s.lines.Count; c++)
                {
                    for (int d = 0; d < s.lines[c].letters.Count; d++)
                        SaveLetter(s.lines[c].letters[d], path + @"/riga_" + c + "_lettera_" + d);
                }

                frame.Save(path + "/frame.png");
                Console.WriteLine("Sottotitoli: " + frameIndex / 23);
            }
        }
    }
}
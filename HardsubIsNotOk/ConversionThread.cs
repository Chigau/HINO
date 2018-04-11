using System;
using AForge.Video.FFMPEG;
using System.Drawing;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Text;
using System.Linq;

namespace HardsubIsNotOk
{
    class ConversionThread
    {
        public const int MinSubPixels = 500;
        //public static string vIndex;
        public static LockBitmap frame;
        public static long frameIndex;
        static LockBitmap[] buffer, buffer1, buffer2;

        public static bool[,] filled;
        private static Letter newLetter;
        public static List<List<Subtitle>> subtitles = new List<List<Subtitle>>();
        public static List<Subtitle> waitForUser = new List<Subtitle>();
        private static bool notALetterFlag;
        static int bufferSize, whatBuffer = 0;
        static bool bufferingPause, bufferingComplete;

        public static void Go()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            Subtitle subTop = null, subBottom = null;
            Thread subRec = new Thread(RecognizeSubtitles);
            subRec.IsBackground = true;
            subRec.Start();
            subRec = new Thread(RecognizeWaitForUser);
            subRec.IsBackground = true;
            subRec.Start();

            foreach (string k in Program.videos.Keys)
            {
                subtitles.Add(new List<Subtitle>());
                bufferSize = Program.videos[k].FrameRate / 2;

                buffer1 = new LockBitmap[bufferSize];
                buffer2 = new LockBitmap[bufferSize];

                Thread bufferThread = new Thread(() => Buffering(k));
                bufferThread.IsBackground = true;
                bufferingComplete = false;
                bufferingPause = false;
                bufferThread.Start();
                while (!bufferingPause) ;

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
                            if (subTop.endFrame - subTop.startFrame >= Settings.minSubLength)
                                subtitles.Last().Add(subTop);
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
                            subtitles.Last().Add(subBottom);
                        subBottom = GetSubtitleBottom();
                        if (subBottom != null)
                            subBottom.GetStartFromBuffer(buffer);
                    }

                    while (!bufferingPause) ;
                }
            }
            Form1.progressBar.Invoke(new Form1.EventHandle(() => Form1.progressBar.Value = 1000));

            stopWatch.Stop();
            Console.WriteLine("-------------TEMPO DI ESTRAZIONE-----------");
            Console.WriteLine(stopWatch.Elapsed);
        }
        public static void Buffering(string vIndex)
        {
            Bitmap b;
            int bufferIndex;
            start:

            bufferIndex = 0;
            if (whatBuffer == 0)
            {
                while ((b = Program.videos[vIndex].ReadVideoFrame()) != null)
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
                    if (frameIndex % 50 == 0 && Program.videos[vIndex].FrameCount != 0)
                        Form1.progressBar.Invoke(new Form1.EventHandle(() => Form1.progressBar.Value = (int)(((float)frameIndex / Program.videos[vIndex].FrameCount) * 1000 / Program.videos.Count + Program.videos.Keys.ToList().IndexOf(vIndex) * 1000 / Program.videos.Count)));
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
                while ((b = Program.videos[vIndex].ReadVideoFrame()) != null)
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
                    if (frameIndex % 50 == 0 && Program.videos[vIndex].FrameCount != 0)
                        Form1.progressBar.Invoke(new Form1.EventHandle(() => Form1.progressBar.Value = (int)(((float)frameIndex / Program.videos[vIndex].FrameCount) * 1000 / Program.videos.Count + Program.videos.Keys.ToList().IndexOf(vIndex) * 1000 / Program.videos.Count)));
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

        public static void RecognizeSubtitles()
        {
            int subIndex = 0;
            int vIndex = 0;
            while (true)
            {
                while (subtitles[vIndex].Count <= subIndex)
                {
                    if (frameIndex >= Program.videos.ElementAt(vIndex).Value.FrameCount && subtitles.Count > vIndex + 1)
                    {
                        vIndex++;
                        subIndex = 0;
                        break;
                    }
                }
                WordNotFound.Result exit;
                Form1.progressBar.Invoke(new Form1.EventHandle(() => Form1.recognitionBar.Value = (int)(((float)subIndex / (subtitles[vIndex].Count + waitForUser.Count)) * 1000)));

                StringBuilder converted = new StringBuilder();
                int wordStart = 0;
                for (int lineIndex = 0; lineIndex < subtitles[vIndex][subIndex].lines.Count; lineIndex++)
                {
                    Subtitle.Line line = subtitles[vIndex][subIndex].lines[lineIndex];
                    for (int c = 0; c < line.letters.Count; c++)
                    {
                        if (line.letters[c] is Space)
                        {
                            if (!TryToCorrectWithDictionary(subtitles[vIndex][subIndex], lineIndex, wordStart, c, converted))
                            {
                                if (Settings.dictionaryMode)
                                {
                                    waitForUser.Add(subtitles[vIndex][subIndex]);
                                    //subtitles.RemoveAt(subIndex);
                                    subIndex++;
                                    goto skipSub;
                                }
                                else
                                {
                                    exit = ShowDictionaryDialog(subtitles[vIndex][subIndex], lineIndex, wordStart, c, converted);
                                    switch (exit)
                                    {
                                        case WordNotFound.Result.skipSub:
                                            subtitles[vIndex].RemoveAt(subIndex);
                                            goto skipSub;
                                        case WordNotFound.Result.subChanged:
                                            goto skipSub;
                                        case WordNotFound.Result.subRewrited:
                                            subIndex++;
                                            goto skipSub;
                                    }
                                }
                            }
                            converted.Append(' ');
                            wordStart = c + 1;
                            continue;
                        }
                        line.letters[c].GenerateArray();
                        line.letters[c].Recognize();

                        if (line.letters[c].firstOverSecondCorrectness > 0.5) //parametrizzabile
                            line.letters[c].secondChoice = null;

                        if (Settings.dictionaryMode)
                        {
                            if (line.letters[c].error > Settings.maxDictionaryError || line.letters[c].firstOverSecondCorrectness < Settings.minDictionaryCorrectness)
                            {
                                waitForUser.Add(subtitles[vIndex][subIndex]);
                                //subtitles.RemoveAt(subIndex);
                                subIndex++;
                                goto skipSub;
                            }
                        }
                        else
                        {
                            if (line.letters[c].error > Settings.maxError || line.letters[c].firstOverSecondCorrectness < Settings.minCorrectness)
                            {
                                GuessLetter.Result e = ShowCorrectLetterDialog(subtitles[vIndex][subIndex], lineIndex, c);
                                switch (e)
                                {
                                    case GuessLetter.Result.notALetter:
                                        line.letters.RemoveAt(c);
                                        c--;
                                        continue;
                                    case GuessLetter.Result.skipSub:
                                        subtitles[vIndex].RemoveAt(subIndex);
                                        goto skipSub;
                                    case GuessLetter.Result.subChanged:
                                        goto skipSub;
                                    case GuessLetter.Result.subRewrited:
                                        subIndex++;
                                        goto skipSub;

                                }
                            }
                        }

                        if (line.letters[c].value != "")
                        {
                            if (!IsLetter(line.letters[c].value))
                            {
                                if (!TryToCorrectWithDictionary(subtitles[vIndex][subIndex], lineIndex, wordStart, c, converted))
                                {
                                    if (Settings.dictionaryMode)
                                    {
                                        waitForUser.Add(subtitles[vIndex][subIndex]);
                                        //subtitles.RemoveAt(subIndex);
                                        subIndex++;
                                        goto skipSub;
                                    }
                                    else
                                    {
                                        exit = ShowDictionaryDialog(subtitles[vIndex][subIndex], lineIndex, wordStart, c, converted);
                                        switch (exit)
                                        {
                                            case WordNotFound.Result.skipSub:
                                                subtitles[vIndex].RemoveAt(subIndex);
                                                goto skipSub;
                                            case WordNotFound.Result.subChanged:
                                                goto skipSub;
                                            case WordNotFound.Result.subRewrited:
                                                subIndex++;
                                                goto skipSub;
                                        }
                                    }
                                }
                                wordStart = c + 1;
                            }
                            converted.Append(line.letters[c].value);
                        }
                    }
                    if (!TryToCorrectWithDictionary(subtitles[vIndex][subIndex], lineIndex, wordStart, line.letters.Count, converted))
                    {
                        if (Settings.dictionaryMode)
                        {
                            waitForUser.Add(subtitles[vIndex][subIndex]);
                            //subtitles.RemoveAt(subIndex);
                            subIndex++;
                            goto skipSub;
                        }
                        else
                        {
                            exit = ShowDictionaryDialog(subtitles[vIndex][subIndex], lineIndex, wordStart, line.letters.Count, converted);
                            switch (exit)
                            {
                                case WordNotFound.Result.skipSub:
                                    subtitles[vIndex].RemoveAt(subIndex);
                                    goto skipSub;
                                case WordNotFound.Result.subChanged:
                                    goto skipSub;
                                case WordNotFound.Result.subRewrited:
                                    subIndex++;
                                    goto skipSub;
                            }
                        }
                    }
                    if (converted.Length != 0 && converted[converted.Length - 1] != '\n')
                        converted.Append('\n');
                    wordStart = 0;
                }
                if (converted.Length == 0)
                {
                    subtitles[vIndex].RemoveAt(subIndex);
                }
                else if (subIndex > 0 && converted.ToString() == subtitles[vIndex][subIndex - 1].value && subtitles[vIndex][subIndex].startFrame - 1 <= subtitles[vIndex][subIndex - 1].endFrame)
                {
                    subtitles[vIndex][subIndex - 1].endFrame = subtitles[vIndex][subIndex].endFrame;
                    subtitles[vIndex].RemoveAt(subIndex);
                }
                else
                {
                    //Console.Write(converted);
                    subtitles[vIndex][subIndex].value = converted.ToString();
                    subIndex++;
                }
                skipSub:;
            }
        }
        public static void RecognizeWaitForUser() //like the recognizeSubtitles, but it works on a separated list made of the subtitles that need user intervent (for processing the main list while waiting for the user)
        {
            while (true)
            {
                while (waitForUser.Count == 0) ;
                Thread.Sleep(20);

                WordNotFound.Result exit;

                StringBuilder converted = new StringBuilder();
                int wordStart = 0;
                for (int lineIndex = 0; lineIndex < waitForUser[0].lines.Count; lineIndex++)
                {
                    Subtitle.Line line = waitForUser[0].lines[lineIndex];
                    for (int c = 0; c < line.letters.Count; c++)
                    {
                        if (line.letters[c] is Space)
                        {
                            if (!TryToCorrectWithDictionary(waitForUser[0], lineIndex, wordStart, c, converted))
                            {
                                exit = ShowDictionaryDialog(waitForUser[0], lineIndex, wordStart, c, converted);
                                switch (exit)
                                {
                                    case WordNotFound.Result.skipSub:
                                        //subtitles.Remove(waitForUser[0]);
                                        waitForUser.RemoveAt(0);
                                        goto skipSub;
                                    case WordNotFound.Result.subChanged:
                                        goto skipSub;
                                    case WordNotFound.Result.subRewrited:
                                        waitForUser.RemoveAt(0);
                                        goto skipSub;
                                }
                            }
                            converted.Append(' ');
                            wordStart = c + 1;
                            continue;
                        }
                        line.letters[c].GenerateArray();
                        line.letters[c].Recognize();

                        if (line.letters[c].firstOverSecondCorrectness > 0.5) //parametrizzabile
                            line.letters[c].secondChoice = null;
                        
                        if (line.letters[c].error > Settings.maxDictionaryError || line.letters[c].firstOverSecondCorrectness < Settings.minDictionaryCorrectness)
                        {
                            GuessLetter.Result e = ShowCorrectLetterDialog(waitForUser[0], lineIndex, c);
                            switch (e)
                            {
                                case GuessLetter.Result.notALetter:
                                    line.letters.RemoveAt(c);
                                    c--;
                                    continue;
                                case GuessLetter.Result.skipSub:
                                    //subtitles.Remove(waitForUser[0]);
                                    waitForUser.RemoveAt(0);
                                    goto skipSub;
                                case GuessLetter.Result.subChanged:
                                    goto skipSub;
                                case GuessLetter.Result.subRewrited:
                                    waitForUser.RemoveAt(0);
                                    goto skipSub;

                            }
                        }

                        if (line.letters[c].value != "")
                        {
                            if (!IsLetter(line.letters[c].value))
                            {
                                if (!TryToCorrectWithDictionary(waitForUser[0], lineIndex, wordStart, c, converted))
                                {
                                    exit = ShowDictionaryDialog(waitForUser[0], lineIndex, wordStart, c, converted);
                                    switch (exit)
                                    {
                                        case WordNotFound.Result.skipSub:
                                            //subtitles.Remove(waitForUser[0]);
                                            waitForUser.RemoveAt(0);
                                            goto skipSub;
                                        case WordNotFound.Result.subChanged:
                                            goto skipSub;
                                        case WordNotFound.Result.subRewrited:
                                            waitForUser.RemoveAt(0);
                                            goto skipSub;
                                    }
                                }
                                wordStart = c + 1;
                            }
                            converted.Append(line.letters[c].value);
                        }
                    }
                    if (!TryToCorrectWithDictionary(waitForUser[0], lineIndex, wordStart, waitForUser[0].lines.Count, converted))
                    {
                        exit = ShowDictionaryDialog(waitForUser[0], lineIndex, wordStart, waitForUser[0].lines.Count, converted);
                        switch (exit)
                        {
                            case WordNotFound.Result.skipSub:
                                //subtitles.Remove(waitForUser[0]);
                                waitForUser.RemoveAt(0);
                                goto skipSub;
                            case WordNotFound.Result.subChanged:
                                goto skipSub;
                            case WordNotFound.Result.subRewrited:
                                waitForUser.RemoveAt(0);
                                goto skipSub;
                        }
                    }
                    if (converted.Length != 0 && converted[converted.Length - 1] != '\n')
                        converted.Append('\n');
                    wordStart = 0;
                }
                //int subIndex = subtitles[vIndex].IndexOf(waitForUser[0]);
                if (converted.Length == 0)
                {
                    //subtitles.Remove(waitForUser[0]);
                    waitForUser.RemoveAt(0);
                }
                //else if (subIndex > 0 && converted.ToString() == subtitles[vIndex][subIndex - 1].value && subtitles[vIndex][subIndex].startFrame - 1 <= subtitles[vIndex][subIndex - 1].endFrame)
                //{
                //    subtitles[vIndex][subIndex - 1].endFrame = subtitles[vIndex][subIndex].endFrame;
                    //subtitles.Remove(waitForUser[0]);
                //    waitForUser[0].value = "";
                //    waitForUser.RemoveAt(0);
                //}
                else
                {
                    //Console.Write(converted);
                    waitForUser[0].value = converted.ToString();
                    waitForUser.RemoveAt(0);
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
        public static GuessLetter.Result ShowCorrectLetterDialog(Subtitle sub, int line, int letter)
        {
            Letter l = sub.lines[line].letters[letter];

            GuessLetter alert = new GuessLetter(sub, l);
            alert.StartPosition = FormStartPosition.CenterScreen;
            alert.ShowDialog();

            switch (alert.result)
            {
                case GuessLetter.Result.incorrect:
                    if (alert.correction == l.value)
                        goto case GuessLetter.Result.correct;

                    if (!Settings.learningDisabled)
                    {
                        Program.examples.Add(l);

                        bool newNet = !Program.neuralNetwork.ContainsKey(alert.correction);
                        if (newNet)
                            Program.neuralNetwork.Add(alert.correction, new Network(alert.correction, 24 * 24, Settings.nnSize, Settings.nnSize));

                        Program.AddLearningThread(l.value, alert.correction);
                        l.value = alert.correction;

                        if (newNet && Program.neuralNetwork.Count >= Settings.maxLearningThreads)
                            Thread.Sleep(1000);
                    }
                    break;

                case GuessLetter.Result.correct:
                    Program.examples.Add(l);
                    if (!Settings.learningDisabled)
                        Program.AddLearningThread(l.value, l.secondChoice);
                    break;

                case GuessLetter.Result.subRewrited:
                    sub.value = alert.correction;
                    break;
            }
            return alert.result;
        }
        public static WordNotFound.Result ShowDictionaryDialog(Subtitle sub, int line, int start, int end, StringBuilder converted)
        {
            string word = "";
            for (int c = start; c < end; c++)
                word += sub.lines[line].letters[c].value;

            WordNotFound alert = new WordNotFound(sub, line, start, end);
            alert.ShowDialog();

            switch (alert.result)
            {
                case WordNotFound.Result.incorrect:
                    string newWord = "";
                    for (int c = start; c < end; c++)
                    {
                        string corrected = alert.correction[c - start].Text;
                        newWord += corrected;
                        if (sub.lines[line].letters[c].value != corrected)
                        {
                            string wrong = sub.lines[line].letters[c].value;
                            sub.lines[line].letters[c].value = corrected;
                            if (!Settings.learningDisabled)
                            {
                                Program.examples.Add(sub.lines[line].letters[c]);

                                bool newNet = !Program.neuralNetwork.ContainsKey(corrected);
                                if (newNet)
                                    Program.neuralNetwork.Add(corrected, new Network(corrected, 24 * 24, Settings.nnSize, Settings.nnSize));

                                Program.AddLearningThread(wrong, corrected);
                            }
                        }
                    }
                    converted.Append(newWord);
                    break;
                case WordNotFound.Result.incorrectWithoutLearning:
                    string n = "";
                    for (int c = start; c < end; c++)
                    {
                        string corrected = alert.correction[c - start].Text;
                        n += corrected;

                    }
                    converted.Append(n);
                    break;
                case WordNotFound.Result.add:
                    converted.Append(word);
                    Program.AddToDictionary(word);
                    break;
                case WordNotFound.Result.properName:
                    converted.Append(word);
                    Program.AddToProperNames(word);
                    break;
                case WordNotFound.Result.dontCare:
                    converted.Append(word);
                    break;
                case WordNotFound.Result.subRewrited:
                    sub.value = alert.subRewrited;
                    break;
            }
            return alert.result;
        }
        public static bool TryToCorrectWithDictionary(Subtitle sub, int line, int start, int end, StringBuilder converted)
        {
            string word = "";
            for (int c = start; c < end; c++)
                word += sub.lines[line].letters[c].value;

            if (word.Length > 0 && !Program.FindWord(word) && !Program.FindName(word))
            {
                converted = converted.Remove(converted.Length - word.Length, word.Length);
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
                    float min = 1;
                    string index = "";
                    foreach(KeyValuePair<string, Letter> alt in alternatives)
                    {
                        if(alt.Value.firstOverSecondCorrectness < min)
                        {
                            min = alt.Value.firstOverSecondCorrectness;
                            index = alt.Key;
                        }
                    }
                    string wrong = alternatives[index].value;
                    string corrected = alternatives[index].secondChoice;

                    alternatives[index].value = corrected;

                    if (!Settings.learningDisabled)
                    {
                        Program.examples.Add(alternatives[index]);
                        Program.AddLearningThread(wrong, corrected);
                    }
                    converted.Append(index);
                    return true;
                }
                return false;
            }
            return true;
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
        static float GetHueDistance(float hue1, float hue2)
        {
            float d = Math.Abs(hue1 - hue2);
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
        static float diff;
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

                    float hue = frame.GetPixel(coord.x, coord.y).GetHue();
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
            float letter = diff / tot;
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
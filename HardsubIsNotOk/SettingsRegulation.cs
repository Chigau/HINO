using System;
using AForge.Video.FFMPEG;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Collections;

namespace HardsubIsNotOk
{
    public partial class SettingsRegulation : Form
    {
        PlayingThread p;
        TimeSpan periodTimeSpan = TimeSpan.FromSeconds(1 / Settings.frameRate);
        System.Windows.Forms.Timer refresh;
        public SettingsRegulation()
        {
            InitializeComponent();
            p = new PlayingThread(Settings.fileName);
            pictureBox2.Image = new Bitmap(p.currentFrame);

            trackBar1.Maximum = Program.reader.Height / 2;
            trackBar2.Maximum = Program.reader.Height / 2;
            trackBar1.Location = new Point(trackBar1.Location.X, pictureBox1.Location.Y + Program.reader.Height - trackBar1.Height);

            trackBar1.Value = trackBar1.Maximum * 2 - Settings.cutBottom;
            trackBar2.Value = trackBar1.Maximum - Settings.cutTop;

            trackBar3.Value = (int)Settings.sameCharacterThreshold;

            trackBar4.Maximum = (int)Settings.sameCharacterThreshold;
            trackBar4.Value = (int)Settings.newCharacterThreshold;

            trackBar5.Value = (int)Settings.outlineThreshold;

            trackBar6.Value = Settings.maxCharPixelSize;
            trackBar7.Value = Settings.minSpaceWidth;
            trackBar8.Value = Settings.minCharPixelSize;
            trackBar9.Value = Settings.outlineWidth;
            trackBar10.Value = Settings.lineDistance;

            colorSubtitle.Color = Settings.subColor;
            button7.BackColor = Settings.subColor;
            colorOutline.Color = Settings.outSubtitleColor;
            button8.BackColor = Settings.outSubtitleColor;

            checkBox1.Checked = Settings.ignoreTopSubtitles;
            checkBox2.Checked = Settings.whiteAndBlack;
            checkBox3.Checked = Settings.discardNonPassingThroughTheCenterLines;
            checkBox4.Checked = Settings.discardNonCenteredLines;
            numericUpDown1.Value = Settings.nonCenteredThreshold;

            defaultCharScale.Text = Settings.defaultCharScale.ToString();
        }


        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {

            if (!p.pause)
                button3_Click(null, null);
            if (tabControl1.SelectedIndex == 0)
            {
                button1.Enabled = true;
                button2.Enabled = false;
            }
            else if (tabControl1.SelectedIndex == 4)
            {
                button1.Enabled = false;
                button2.Enabled = true;
                UpdateStep5Image();
            }
            else
            {
                button1.Enabled = true;
                button2.Enabled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex++;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex--;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (p.pause)
            {
                refresh = new System.Windows.Forms.Timer();
                refresh.Tick += new EventHandler(NextFrame);
                refresh.Interval = (int)periodTimeSpan.TotalMilliseconds;
                refresh.Start();

                p.Play();
                button3.Text = "Pause";
            }
            else
            {
                refresh.Dispose();
                p.Stop();
                button3.Text = "Play";
                UpdateCutImage();
                UpdateStep34Image();
            }
        }
        public void NextFrame(object sender, EventArgs e)
        {
            Image toDispose = pictureBox2.Image;
            p.frameInUse = true;
            pictureBox2.Image = new Bitmap(p.currentFrame);
            p.frameInUse = false;
            toDispose.Dispose();
            textBox1.Text = TimeSpan.FromSeconds(p.frameCount / Settings.frameRate).ToString("G");
        }

        private void button11_Click(object sender, EventArgs e)
        {
            p.SetSpeed(8);
        }
        private void button6_Click(object sender, EventArgs e)
        {
            p.SetSpeed(4);
        }
        private void button5_Click(object sender, EventArgs e)
        {
            p.SetSpeed(2);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            p.SetSpeed(1);
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            UpdateCutImage();
        }
        private void trackBar2_DockChanged(object sender, EventArgs e)
        {
            UpdateCutImage();
        }
        private void trackBar2_Scroll(object sender, EventArgs e)
        {

        }
        void UpdateCutImage()
        {
            p.frameInUse = true;
            Bitmap newBit = new Bitmap(p.currentFrame);
            p.frameInUse = false;
            
            Pen pen = new Pen(Color.Red, 2);

            using (var graphics = Graphics.FromImage(newBit))
            {
                int val1 = trackBar1.Maximum * 2 - trackBar1.Value;
                graphics.DrawLine(pen, 0, val1, newBit.Width, val1);
                if (!checkBox1.Checked)
                {
                    int val2 = trackBar1.Maximum - trackBar2.Value;
                    graphics.DrawLine(pen, 0, val2, newBit.Width, val2);
                }
            }

            pictureBox1.Image = newBit;
        }

        private void trackBar2_ValueChanged(object sender, EventArgs e)
        {
            Settings.cutTop = trackBar1.Maximum - trackBar2.Value;
            UpdateCutImage();
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            Settings.cutBottom = trackBar1.Maximum * 2 - trackBar1.Value;
            UpdateCutImage();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            trackBar2.Enabled = !checkBox1.Checked;
            Settings.ignoreTopSubtitles = checkBox1.Checked;
            UpdateCutImage();
        }
        
        private void trackBar1_MouseUp(object sender, MouseEventArgs e)
        {
            UpdateStep34Image();
        }

        void UpdateStep34Image()
        {
            Bitmap newBit = new Bitmap(p.currentFrame);
            p.frameInUse = true;
            frame = new LockBitmap(new Bitmap(p.currentFrame));
            p.frameInUse = false;
            frame.LockBits();
            ConversionThread.frame = frame;

            Rectangle cropRect = new Rectangle(0, Settings.cutBottom, newBit.Width, newBit.Height - Settings.cutBottom);
            Bitmap target = new Bitmap(cropRect.Width, cropRect.Height);

            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(newBit, new Rectangle(0, 0, target.Width, target.Height),
                                 cropRect,
                                 GraphicsUnit.Pixel);
            }
            pictureBox6.Image = target;


            Subtitle bottom = GetSubtitleBottom();
            target = new Bitmap(cropRect.Width, cropRect.Height);
            for (int x = 0; x < filled.GetLength(0); x++)
                for (int y = 0; y < filled.GetLength(1); y++)
                    if (filled[x, y])
                    {
                        if(ColorDiff(frame.GetPixel(x, y), Settings.subColor) >= Settings.sameCharacterThreshold)
                            newBit.SetPixel(x, y, Color.LightPink);
                        else
                            newBit.SetPixel(x, y, Color.Red);
                    }

            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(newBit, new Rectangle(0, 0, target.Width, target.Height),
                                 cropRect,
                                 GraphicsUnit.Pixel);
            }
            pictureBox4.Image = target;

            if (bottom != null)
            {
                newBit = bottom.GetFrame();
                target = new Bitmap(cropRect.Width, cropRect.Height);
                using (Graphics g = Graphics.FromImage(target))
                {
                    g.DrawImage(newBit, new Rectangle(0, 0, target.Width, target.Height),
                                     cropRect,
                                     GraphicsUnit.Pixel);
                }
                pictureBox5.Image = target;
                pictureBox3.Image = target;
            }
            else
            {
                pictureBox5.Image = null;
                pictureBox3.Image = null;
            }

        }
        private void button12_Click(object sender, EventArgs e)
        {
            int r = 0, g = 0, b = 0, div = 0;
            Subtitle bottom = GetSubtitleBottom();
            if (bottom != null)
            {
                foreach (Subtitle.Line line in bottom.lines)
                {
                    foreach (Letter l in line.letters)
                    {
                        foreach (Coord c in l.pixels)
                        {
                            Color col = frame.GetPixel(c.x, c.y);
                            r += col.R;
                            g += col.G;
                            b += col.B;
                            div++;
                        }
                    }
                }
                if (div != 0)
                {
                    Color mean = Color.FromArgb(r / div, g / div, b / div);
                    colorSubtitle.Color = mean;
                    button7.BackColor = mean;
                    Settings.subColor = mean;
                    UpdateStep34Image();
                }
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            int r = 0, g = 0, b = 0, div = 0;
            Subtitle bottom = GetSubtitleBottom();
            if (bottom != null)
            {
                foreach (Subtitle.Line line in bottom.lines)
                {
                    foreach (Letter l in line.letters)
                    {
                        foreach (Coord c in l.outlinePixels)
                        {
                            Color col = frame.GetPixel(c.x, c.y);
                            r += col.R;
                            g += col.G;
                            b += col.B;
                            div++;
                        }
                    }
                }
                if (div != 0)
                {
                    Color mean = Color.FromArgb(r / div, g / div, b / div);
                    colorOutline.Color = mean;
                    button8.BackColor = mean;
                    Settings.outSubtitleColor = mean;
                    UpdateStep34Image();
                }
            }
        }
        void UpdateStep5Image()
        {
            Subtitle bottom = GetSubtitleBottom();
            if (bottom != null)
            {
                Bitmap newBit = bottom.GetFrame();
                Rectangle cropRect = new Rectangle(0, Settings.cutBottom, newBit.Width, newBit.Height - Settings.cutBottom);
                Bitmap target = new Bitmap(cropRect.Width, cropRect.Height);
                using (Graphics g = Graphics.FromImage(target))
                {
                    g.DrawImage(newBit, new Rectangle(0, 0, target.Width, target.Height),
                                     cropRect,
                                     GraphicsUnit.Pixel);
                }

                bottom.CalculateSpaces();
                target = new Bitmap(target);
                Pen pen = new Pen(Color.Red, 2);
                Pen spacePen = new Pen(Color.Green, 2);
                using (var graphics = Graphics.FromImage(target))
                {
                    foreach (Subtitle.Line line in bottom.lines)
                        for (int c = 0; c < line.letters.Count; c++)
                        {
                            if (c + 1 == line.letters.Count || line.letters[c + 1] is Space)
                            {
                                graphics.DrawRectangle(spacePen, Rectangle.FromLTRB(line.letters[c].xMin, line.letters[c].yMin - Settings.cutBottom, line.letters[c].xMax, line.letters[c].yMax - Settings.cutBottom));
                                c++;
                            }
                            else
                            {
                                graphics.DrawRectangle(pen, Rectangle.FromLTRB(line.letters[c].xMin, line.letters[c].yMin - Settings.cutBottom, line.letters[c].xMax, line.letters[c].yMax - Settings.cutBottom));
                            }
                        }
                }
                pictureBox7.Image = target;
            }
            else
            {
                pictureBox7.Image = null;
            }
        }
        private void button14_Click(object sender, EventArgs e)
        {
            Subtitle bottom = GetSubtitleBottom();
            if(bottom != null)
            {
                int max = 0;
                foreach(Subtitle.Line line in bottom.lines)
                    foreach(Letter l in line.letters)
                    {
                        int width = l.xMax - l.xMin;
                        int height = l.yMax - l.yMin;
                        if (width > max)
                            max = width;
                        if (height > max)
                            max = height;
                    }
                defaultCharScale.Text = ((double)23 / max).ToString();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            DialogResult pick = colorSubtitle.ShowDialog();
            button7.BackColor = colorSubtitle.Color;
            Settings.subColor = colorSubtitle.Color;
            UpdateStep34Image();
        }
        private void button8_Click(object sender, EventArgs e)
        {
            DialogResult pick = colorOutline.ShowDialog();
            button8.BackColor = colorOutline.Color;
            Settings.outSubtitleColor = colorOutline.Color;
            UpdateStep34Image();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            button9.Enabled = false;
        }
        private void button10_Click(object sender, EventArgs e)
        {
            button10.Enabled = false;
        }

        private void pictureBox6_MouseMove(object sender, MouseEventArgs e)
        {
            if (!button9.Enabled || !button10.Enabled)
            {
                Image b = pictureBox6.Image;

                double imageRatio = (double)b.Width / b.Height;

                double boxRatio = (double)pictureBox6.Width / pictureBox6.Height;

                int x, y;
                if(imageRatio > boxRatio)
                {
                    int newWidth = pictureBox6.Width;
                    int newHeight = (int)(newWidth / imageRatio);

                    x = (int)(((double)b.Width / newWidth) * e.X);
                    y = (int)(((double)b.Height / newHeight) * (e.Y - (pictureBox6.Height - newHeight) / 2));
                }
                else
                {
                    int newHeight = pictureBox6.Height;
                    int newWidth = (int)(newHeight * imageRatio);

                    x = (int)(((double)b.Width / newWidth) * (e.X - (pictureBox6.Width - newWidth) / 2));
                    y = (int)(((double)b.Height / newHeight) * e.Y);
                }

                if (x < b.Width && x >= 0 && y < b.Height && y >= 0)
                {
                    if(button9.Enabled)
                        button8.BackColor = ((Bitmap)pictureBox6.Image).GetPixel(x, y);
                    else
                        button7.BackColor = ((Bitmap)pictureBox6.Image).GetPixel(x, y);
                }
            }
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            if (!button9.Enabled)
            {
                button9.Enabled = true;
                colorSubtitle.Color = button7.BackColor;
                Settings.subColor = colorSubtitle.Color;
                UpdateStep34Image();
            }
            else if (!button10.Enabled)
            {
                button10.Enabled = true;
                colorOutline.Color = button8.BackColor;
                Settings.outSubtitleColor = colorOutline.Color;
                UpdateStep34Image();
            }
        }

        private void trackBar3_ValueChanged(object sender, EventArgs e)
        {
            sameCharacterThreshold.Text = trackBar3.Value.ToString();
            trackBar4.Maximum = trackBar3.Value;
        }
        private void trackBar4_ValueChanged(object sender, EventArgs e)
        {
            newCharacterThreshold.Text = trackBar4.Value.ToString();
        }
        private void trackBar5_ValueChanged(object sender, EventArgs e)
        {
            outlineThreshold.Text = trackBar5.Value.ToString();
        }
        private void trackBar6_ValueChanged(object sender, EventArgs e)
        {
            maxCharPixelSize.Text = trackBar6.Value.ToString();
        }
        private void trackBar7_ValueChanged(object sender, EventArgs e)
        {
            minSpaceWidth.Text = trackBar7.Value.ToString();
        }
        private void trackBar8_ValueChanged(object sender, EventArgs e)
        {
            minCharPixelSize.Text = trackBar8.Value.ToString();
        }
        private void trackBar9_ValueChanged(object sender, EventArgs e)
        {
            outlineWidth.Text = trackBar9.Value.ToString();
        }
        private void trackBar10_ValueChanged(object sender, EventArgs e)
        {
            lineDistance.Text = trackBar10.Value.ToString();
        }

        private void sameCharacterThreshold_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Settings.sameCharacterThreshold = float.Parse(sameCharacterThreshold.Text);
                if (p.frameCount != 0)
                    UpdateStep34Image();
            }
            catch (FormatException ex) { }
        }

        private void newCharacterThreshold_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Settings.newCharacterThreshold = float.Parse(newCharacterThreshold.Text);
                if (p.frameCount != 0)
                    UpdateStep34Image();
            }
            catch (FormatException ex) { }
        }

        private void outlineThreshold_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Settings.outlineThreshold = float.Parse(outlineThreshold.Text);
                if (p.frameCount != 0)
                    UpdateStep34Image();
            }
            catch (FormatException ex) { }
        }

        private void minCharPixelSize_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Settings.minCharPixelSize = int.Parse(minCharPixelSize.Text);
                if (p.frameCount != 0)
                    UpdateStep34Image();
            }
            catch (FormatException ex) { }
        }

        private void maxCharPixelSize_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Settings.maxCharPixelSize = int.Parse(maxCharPixelSize.Text);
                if (p.frameCount != 0)
                    UpdateStep34Image();
            }
            catch (FormatException ex) { }
        }

        private void minSpaceWidth_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Settings.minSpaceWidth = int.Parse(minSpaceWidth.Text);
                if (p.frameCount != 0)
                    UpdateStep5Image();
            }
            catch (FormatException ex) { }
        }

        private void lineDistance_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Settings.lineDistance = int.Parse(lineDistance.Text);
                if (p.frameCount != 0)
                    UpdateStep5Image();
            }
            catch (FormatException ex) { }
        }

        private void outlineWidth_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Settings.outlineWidth = int.Parse(outlineWidth.Text);
                if (p.frameCount != 0)
                    UpdateStep34Image();
            }
            catch (FormatException ex) { }
        }
        private void defaultCharScale_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Settings.defaultCharScale = double.Parse(defaultCharScale.Text);
            }
            catch (FormatException ex) { }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            Settings.whiteAndBlack = checkBox2.Checked;
            UpdateStep34Image();
        }
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            Settings.discardNonPassingThroughTheCenterLines = checkBox3.Checked;
        }
        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            Settings.discardNonCenteredLines = checkBox4.Checked;
        }
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Settings.nonCenteredThreshold = (int)numericUpDown1.Value;
        }

        //da qui in giù le funzioni sono copiate dal Conversion Thread
        LockBitmap frame;
        bool[,] filled;
        bool notALetterFlag;
        Letter newLetter;

        private Subtitle GetSubtitleBottom()
        {
            filled = new bool[frame.Width, frame.Height];
            Subtitle subtitle = null;
            int x, y;
            for (x = 0; x < frame.Width; x += 2)
            {
                for (y = Settings.cutBottom; y < frame.Height; y += 2)
                {
                    if (!IsFilled(new Coord(x, y)) && AreSimilar(frame.GetPixel(x, y), Settings.subColor, Settings.newCharacterThreshold))
                    {
                        Letter get = GetLetter(x, y);
                        if (get != null)
                        {
                            if (subtitle == null)
                                subtitle = new Subtitle();
                            if(get.pixels.Count < Settings.maxCharPixelSize)
                                subtitle.AddLetter(get);
                        }
                    }
                }
            }
            if (subtitle != null)
                foreach (Subtitle.Line l in subtitle.lines)
                {
                    for (int i = 0; i < l.letters.Count; i++)
                    {
                        if (l.letters[i].pixels.Count < Settings.minCharPixelSize) //|| l.letters[i].pixels.Count > Settings.maxCharPixelSize)
                            l.letters.RemoveAt(i);
                    }
                }
            return subtitle;
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
        bool IsValid(Coord coord)
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

        float idealHue = Settings.outSubtitleColor.GetHue();
        double diff; int tot;
        void CheckEdges(Coord coord, int distance = 0)
        {
            if (!IsFilled(coord))
            {
                if (coord.x >= 0 && coord.y >= 0 && coord.x < frame.Width && coord.y < frame.Height && ColorDiff(frame.GetPixel(coord.x, coord.y), Settings.subColor) >= Settings.sameCharacterThreshold)
                {
                    filled[coord.x, coord.y] = true;
                    newLetter.outlinePixels.Add(coord);
                    if (distance < Settings.outlineWidth)
                        foreach (Coord c in coord.Edge)
                            CheckEdges(c, distance + 1);

                    double hue = frame.GetPixel(coord.x, coord.y).GetHue();
                    diff += GetHueDistance(idealHue, hue);
                    tot++;
                }
            }
        }
        void CheckEdgesWB(Coord coord, int distance = 0)
        {
            if (!IsFilled(coord))
            {
                if (coord.x >= 0 && coord.y >= 0 && coord.x < frame.Width && coord.y < frame.Height && ColorDiff(frame.GetPixel(coord.x, coord.y), Settings.subColor) >= Settings.sameCharacterThreshold)
                {
                    filled[coord.x, coord.y] = true;
                    newLetter.outlinePixels.Add(coord);
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

        private Letter GetLetter(int x, int y)
        {
            newLetter = new Letter();
            newLetter.outlinePixels = new HashSet<Coord>();
            notALetterFlag = false;
            Coord cStart = new Coord(x, y);
            diff = 0; tot = 0;
            Fill(cStart);
            double letter = diff / tot;
            if (notALetterFlag || letter > Settings.outlineThreshold)
                return null;
            return newLetter;
        }
        private void Fill(Coord p)
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
        bool IsFilled(Coord point)
        {
            return point.x < 0 || point.x >= filled.GetLength(0) || point.y < 0 || point.y >= filled.GetLength(1) || filled[point.x, point.y];
        }
    }

    public class PlayingThread
    {
        public Image currentFrame;
        public int frameCount = 0;
        public bool pause = true, frameInUse = false;

        VideoFileReader reader = new VideoFileReader();
        public Double frameDuration;
        public PlayingThread(String filename)
        {
            frameDuration = 1 / Settings.frameRate;
            reader.Open(filename);
            currentFrame = reader.ReadVideoFrame();
        }

        public void Play()
        {
            pause = false;
            Thread playing = new Thread(Buffering);
            playing.Start();
        }
        public void Stop()
        {
            pause = true;
        }
        protected void Buffering()
        {
            Stopwatch s = new Stopwatch();
            s.Start();

            while (!pause)
            {
                Image toDispose = currentFrame;
                currentFrame = reader.ReadVideoFrame();

                frameCount++;
                while (frameInUse) ;
                toDispose.Dispose();
                while (s.Elapsed.TotalSeconds < frameDuration) ;
                s.Restart();
            }
        }
        public void SetSpeed(int val)
        {
            frameDuration = 1 / (Settings.frameRate * val);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HardsubIsNotOk
{
    public partial class WordNotFound : Form
    {
        public Result result = Result.none;
        public List<TextBox> correction = new List<TextBox>();
        public string subRewrited;
        public Subtitle sub;
        public int line, start, end;
        public enum Result
        {
            incorrect = 0,
            incorrectWithoutLearning = 1,
            add = 2,
            dontCare = 3,
            skipSub = 4,
            subChanged = 5,
            subRewrited = 6,
            properName = 7,
            none = 8,
        }

        private void button1_Click(object sender, EventArgs e)
        {
            result = Result.dontCare;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            result = Result.incorrect;
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            result = Result.add;
            Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Settings.dictionaryMode = false;
            result = Result.incorrect;
            Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            result = Result.properName;
            Close();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            result = Result.incorrectWithoutLearning;
            Close();
        }

        public WordNotFound(Subtitle sub, int line, int start, int end)
        {
            string word = "";
            InitializeComponent();
            int Y = label3.Location.Y + 50;
            int X = label3.Location.X;
            Font labelFont = new Font("Tahoma", 15, FontStyle.Bold);
            Font textboxFont = new Font("Tahoma", 12);


            for (int c = start; c < end; c++)
            {
                word += sub.lines[line].letters[c].value;
                PictureBox p = new PictureBox();
                p.Location = new Point(X + 20, Y + 40 * (c - start));
                p.Image = sub.lines[line].letters[c].ArrayToBitmap();
                p.Size = p.Image.Size;
                Controls.Add(p);

                Label l = new Label();
                l.Location = new Point(X + 140, Y + 40 * (c - start));
                l.Text = sub.lines[line].letters[c].value;
                if(sub.lines[line].letters[c].secondChoice != null)
                    l.Text = sub.lines[line].letters[c].value + " (" + sub.lines[line].letters[c].secondChoice + ")";
                else
                    l.Text = sub.lines[line].letters[c].value;
                l.Font = labelFont;
                Controls.Add(l);

                TextBox t = new TextBox();
                t.Location = new Point(X + 250, Y + 40 * (c - start));
                t.Text = sub.lines[line].letters[c].value;
                t.Font = textboxFont;
                Controls.Add(t);
                correction.Add(t);
            }
            int newHeight = Y + 40 * (end - start) + 200;
            Size = new Size(Size.Width, newHeight > 500 ? newHeight : 500);
            textBox2.Text = word;
            this.sub = sub;
            this.line = line;
            this.start = start;
            this.end = end;


            button5.Enabled = Settings.dictionaryMode;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int yMin = Math.Min(sub.lines[line].letters[start].yMin, sub.lines[line].letters[end - 1].yMin);
            int yMax = Math.Max(sub.lines[line].letters[start].yMax, sub.lines[line].letters[end - 1].yMax);

            FramePreview prev = new FramePreview(sub, sub.lines[line].letters[start].xMin - 2, yMin - 2, sub.lines[line].letters[end - 1].xMax + 2, yMax + 2);
            prev.ShowDialog();
            if (prev.exitStatus == GuessLetter.Result.skipSub)
            {
                result = Result.skipSub;
                Close();
            }
            else if (prev.exitStatus == GuessLetter.Result.subChanged)
            {
                result = Result.subChanged;
                Close();
            }
            else if (prev.exitStatus == GuessLetter.Result.subRewrited)
            {
                subRewrited = prev.corrected;
                result = Result.subRewrited;
                Close();
            }
        }
    }
}

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
    public partial class FramePreview : Form
    {
        public string corrected;
        public GuessLetter.Result exitStatus = GuessLetter.Result.none;
        bool errorRemoval = false;
        public Dictionary<Coord, Letter> toRemove;
        Subtitle sub;
        public FramePreview(Subtitle sub, int l, int t, int r, int b)
        {
            InitializeComponent();
            Bitmap image = new Bitmap(sub.GetFrame());
            Pen pen = new Pen(Color.Red, 2);
            Pen borderPen = new Pen(Color.Green, 2);

            using (var graphics = Graphics.FromImage(image))
            {
                graphics.DrawRectangle(pen, Rectangle.FromLTRB(l, t, r, b));

                graphics.DrawRectangle(borderPen, Rectangle.FromLTRB(0, 0, image.Width, Settings.cutTop));
                graphics.DrawRectangle(borderPen, Rectangle.FromLTRB(0, Settings.cutBottom, image.Width, image.Height));
            }
            preview.Image = image;
            label1.Text = "Frame: " + sub.startFrame + "-" + sub.endFrame;
            this.sub = sub;
        }
        public FramePreview(Bitmap image)
        {
            InitializeComponent();
            preview.Image = image;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            exitStatus = GuessLetter.Result.skipSub;
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            errorRemoval = !errorRemoval;
            if (errorRemoval)
            {
                toRemove = new Dictionary<Coord, Letter>();
                button3.Text = "Confirm";
                preview.SizeMode = PictureBoxSizeMode.StretchImage;
            }
            else
            {
                foreach (KeyValuePair<Coord, Letter> p in toRemove)
                {
                    sub.discardedPixels.Add(p.Key);
                    p.Value.pixels.Remove(p.Key);
                }
                for (int line = 0; line < sub.lines.Count; line++)
                {
                    for (int c = 0; c < sub.lines[line].letters.Count; c++)
                        if (sub.lines[line].letters[c].pixels.Count == 0)
                        {
                            sub.lines[line].letters.RemoveAt(c);
                            c--;
                        }
                        else
                        {
                            sub.lines[line].letters[c].RecalcCoords();
                        }
                    if (sub.lines[line].letters.Count == 0)
                        sub.lines.RemoveAt(line);

                    exitStatus = GuessLetter.Result.subChanged;
                    Close();
                }
                sub.CalculateSpaces();
            }
        }

        private void preview_Click(object sender, EventArgs e)
        {
            if (errorRemoval)
            {
                MouseEventArgs me = (MouseEventArgs)e;
                Image b = preview.Image;
                int x = b.Width * me.X / preview.Width;
                int y = b.Height * me.Y / preview.Height;
                Coord c = new Coord(x, y);
                foreach (Subtitle.Line line in sub.lines)
                    foreach(Letter l in line.letters)
                    {
                        if (l.pixels.Contains(c))
                        {
                            HashSet<Coord> filled = new HashSet<Coord>();
                            Fill(l, c, ref filled);
                            if (!toRemove.ContainsKey(c))
                            {
                                foreach (Coord f in filled)
                                {
                                    ((Bitmap)preview.Image).SetPixel(f.x, f.y, Color.Blue);
                                    toRemove.Add(f, l);
                                }
                            }
                            else
                            {
                                foreach (Coord f in filled)
                                {
                                    ((Bitmap)preview.Image).SetPixel(f.x, f.y, Color.Black);
                                    toRemove.Remove(f);
                                }
                            }
                            preview.Refresh();
                        }
                    }

            }
        }
        public void Fill(Letter l, Coord c, ref HashSet<Coord> filled)
        {
            if (!l.pixels.Contains(c))
                return;
            if (filled.Add(c))
            {
                Fill(l, c.Top, ref filled);
                Fill(l, c.Bottom, ref filled);
                Fill(l, c.Right, ref filled);
                Fill(l, c.Left, ref filled);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (textBox1.Visible)
            {
                corrected = textBox1.Text + '\n';
                exitStatus = GuessLetter.Result.subRewrited;
                Close();
            }
            else
            {
                textBox1.Visible = true;
                button4.Text = "Confirm";
            }
        }
    }
}

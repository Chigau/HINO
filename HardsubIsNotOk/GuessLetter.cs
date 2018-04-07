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
    public partial class GuessLetter : Form
    {
        public Result result = Result.none;
        public string correction = "";
        public Subtitle sub;
        public Letter lett;
        public enum Result
        {
            correct = 0,
            notALetter = 1,
            incorrect = 2,
            skipSub = 3,
            subChanged = 4,
            subRewrited = 5,
            none = 6,
        }
        public GuessLetter(Subtitle sub, Letter lett)
        {
            string message = 
                "Recognition: " + lett.value + 
                "\nAverage error: " + lett.error + 
                "\nSecond choice: " + lett.secondChoice + 
                "\nFirst over second choice correctness: " + (int)(lett.firstOverSecondCorrectness * 100) + 
                "%\nFrame: " + sub.startFrame + "-" + sub.endFrame;

            InitializeComponent();
            letterBox.Image = lett.ArrayToBitmap();
            this.sub = sub;
            this.lett = lett;
            label2.Text = message;
            button5.Enabled = !Settings.dictionaryMode;
        }

        private void GuessLetter_Load(object sender, EventArgs e)
        {
            ActiveControl = letter;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            result = Result.correct;
            Close();
        }
        

        private void button1_Click(object sender, EventArgs e)
        {
            result = Result.notALetter;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (letter.Text != "")
            {
                result = Result.incorrect;
                correction = letter.Text;
                Close();
            }
        }

        private void letter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                button2_Click(null, null);

        }

        private void button4_Click(object sender, EventArgs e)
        {
            FramePreview prev = new FramePreview(sub, lett.xMin - 2, lett.yMin - 2, lett.xMax + 2, lett.yMax + 2);
            prev.ShowDialog();
            if(prev.exitStatus == Result.subRewrited)
            {
                correction = prev.corrected;
                result = prev.exitStatus;
                Close();
            }
            else if(prev.exitStatus == Result.skipSub || prev.exitStatus == Result.subChanged)
            {
                result = prev.exitStatus;
                Close();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Settings.dictionaryMode = true;
            result = Result.correct;
            Close();
        }
    }
}

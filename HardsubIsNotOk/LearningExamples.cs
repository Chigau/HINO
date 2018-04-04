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
    public partial class LearningExamples : Form
    {
        private readonly ContextMenuStrip strip;

        Dictionary<string, List<Letter>> examples = new Dictionary<string, List<Letter>>();
        Dictionary<string, ImageList> images = new Dictionary<string, ImageList>();

        public LearningExamples()
        {
            InitializeComponent();

            var toolStripMenuItem1 = new ToolStripMenuItem { Text = "Duplicate" };
            toolStripMenuItem1.Click += toolStripMenuItem1_Click;
            var toolStripMenuItem2 = new ToolStripMenuItem { Text = "Delete" };
            toolStripMenuItem2.Click += toolStripMenuItem2_Click;

            strip = new ContextMenuStrip();
            strip.Items.AddRange(new ToolStripItem[] { toolStripMenuItem1, toolStripMenuItem2 });
            exampleList.MouseClick += exampleList_MouseClick;

            foreach (Letter l in Program.examples)
            {
                letterList.Sorted = true;
                if (!examples.Keys.Contains(l.value))
                {
                    examples.Add(l.value, new List<Letter>());
                    images.Add(l.value, new ImageList());
                    letterList.Items.Add(l.value);
                }
                examples[l.value].Add(l);
                images[l.value].Images.Add(l.ArrayToBitmap());
            }
        }
        public void Repaint()
        {
            string i = (string)letterList.SelectedItem;

            examples = new Dictionary<string, List<Letter>>();
            images = new Dictionary<string, ImageList>();
            exampleList.Clear();
            letterList.Items.Clear();

            foreach (Letter l in Program.examples)
            {
                letterList.Sorted = true;
                if (!examples.Keys.Contains(l.value))
                {
                    examples.Add(l.value, new List<Letter>());
                    images.Add(l.value, new ImageList());
                    letterList.Items.Add(l.value);
                }
                examples[l.value].Add(l);
                images[l.value].Images.Add(l.ArrayToBitmap());
            }
            if (images.Keys.Contains(i))
                letterList.SelectedItem = i;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string i = (string)letterList.SelectedItem;
            exampleList.Clear();
            exampleList.LargeImageList = images[i];
            for (int c = 0; c < examples[i].Count; c++)
            {
                ListViewItem item = new ListViewItem();
                item.ImageIndex = c;
                exampleList.Items.Add(item);
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string l = (string)letterList.SelectedItem;
            int i = exampleList.FocusedItem.Index;
            Letter duplicated = new Letter(examples[l][i]);
            Program.examples.Add(duplicated);

            Repaint();

        }
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            string l = (string)letterList.SelectedItem;
            int i = exampleList.FocusedItem.Index;
            Program.examples.Remove(examples[l][i]);

            Repaint();
        }

        private void exampleList_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (exampleList.FocusedItem.Bounds.Contains(e.Location) == true)
                {
                    strip.Show(Cursor.Position);
                }
            }
        }
    }
}

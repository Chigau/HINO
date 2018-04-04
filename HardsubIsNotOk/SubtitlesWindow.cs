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
    public partial class SubtitlesWindow : Form
    {
        public SubtitlesWindow()
        {
            InitializeComponent();
            foreach(Subtitle s in ConversionThread.subtitles)
                if (s.value != "")
                {
                    TimeSpan start = TimeSpan.FromSeconds(s.startFrame / Settings.frameRate);
                    TimeSpan end = TimeSpan.FromSeconds(s.endFrame / Settings.frameRate);
                    SubtitlesTable.Items.Add(new ListViewItem(new[] { s.value, start.ToString("g"), end.ToString("g") }));
                }
        }

        private void aggiornaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SubtitlesTable.Items.Clear();
            foreach (Subtitle s in ConversionThread.subtitles)
                if (s.value != "")
                {
                    TimeSpan start = TimeSpan.FromSeconds(s.startFrame / Settings.frameRate);
                    TimeSpan end = TimeSpan.FromSeconds(s.endFrame / Settings.frameRate);
                    SubtitlesTable.Items.Add(new ListViewItem(new[] { s.value, start.ToString("g"), end.ToString("g") }));
                }
        }
    }
}

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
            for (int c = 0; c < ConversionThread.subtitles.Count; c++)
            {
                foreach (Subtitle s in ConversionThread.subtitles[c])
                {
                    SubtitlesTable.Groups.Add(new ListViewGroup(Program.videos.Keys.ToList()[c]));
                    if (s.value != "")
                    {
                        TimeSpan start = TimeSpan.FromSeconds(s.startFrame / Settings.frameRate);
                        TimeSpan end = TimeSpan.FromSeconds(s.endFrame / Settings.frameRate);
                        SubtitlesTable.Items.Add(new ListViewItem(new[] { s.value, start.ToString("g"), end.ToString("g") }, SubtitlesTable.Groups[c]));
                    }
                }
            }
        }

        private void aggiornaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SubtitlesTable.Items.Clear();
            for (int c = 0; c < ConversionThread.subtitles.Count; c++)
            {
                foreach (Subtitle s in ConversionThread.subtitles[c])
                {
                    SubtitlesTable.Groups.Add(new ListViewGroup(Program.videos.Keys.ToList()[c]));
                    if (s.value != "")
                    {
                        TimeSpan start = TimeSpan.FromSeconds(s.startFrame / Settings.frameRate);
                        TimeSpan end = TimeSpan.FromSeconds(s.endFrame / Settings.frameRate);
                        SubtitlesTable.Items.Add(new ListViewItem(new[] { s.value, start.ToString("g"), end.ToString("g") }, SubtitlesTable.Groups[c]));
                    }
                }
            }
        }
    }
}

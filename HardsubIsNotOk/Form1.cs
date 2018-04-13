using AForge.Video.FFMPEG;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace HardsubIsNotOk
{
    public partial class Form1 : Form
    {
        public static ListView info;
        public static ProgressBar progressBar, recognitionBar;
        public Form1()
        {
            InitializeComponent();
        }

        private void opzioniToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void caricaFileDesempioToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void apriVideoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "Video files|*.MKV;*.AVI;*.MP4;|Everything (*.*)|*.*",
                FilterIndex = 1,
                Multiselect = true
            };
            
            DialogResult result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                foreach(string file in openFileDialog.FileNames)
                    AddVideo(file);
            }

        }
        void AddVideo(string path)
        {
            if (Program.videos.ContainsKey(path))
            {
                MessageBox.Show("Video '" + path + "' is duplicated. It will be removed.", "Duplicated video", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            VideoFileReader reader = new VideoFileReader();
            reader.Open(path);

            listBox1.Items.Add(path);
            Program.videos.Add(path, reader);

            if (label1.Visible)
            {
                if (Settings.cutTop == -1)
                {
                    Settings.cutTop = reader.Height / 6;
                    Settings.cutBottom = reader.Height - Settings.cutTop;
                }
                start.Enabled = true;
                button1.Enabled = true;
                listBox1.SelectedIndex = 0;
                label1.Visible = false;

                Settings.saveSubsPath = Path.GetDirectoryName(path);
            }
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            info.Items.Clear();
            if (listBox1.SelectedItem == null)
                return;

            string index = listBox1.SelectedItem.ToString();
            
            // video attributes
            info.Items.Add(new ListViewItem(new[] { "File name:", index }, info.Groups[0]));
            info.Items.Add(new ListViewItem(new[] { "Width:", Program.videos[index].Width + "px" }, info.Groups[0]));
            info.Items.Add(new ListViewItem(new[] { "Height:", Program.videos[index].Height + "px" }, info.Groups[0]));
            if (Program.videos[index].FrameRate != 0)
                info.Items.Add(new ListViewItem(new[] { "Fps:", Program.videos[index].FrameRate + "" }, info.Groups[0]));
            if (Program.videos[index].FrameCount != 0)
                info.Items.Add(new ListViewItem(new[] { "Frame count:", Program.videos[index].FrameCount + "" }, info.Groups[0]));
            info.Items.Add(new ListViewItem(new[] { "Codec:", Program.videos[index].CodecName }, info.Groups[0]));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            info = listView1;
            progressBar = extraction;
            recognitionBar = recognition;
        }

        Thread conversion;
        private void start_Click(object sender, EventArgs e)
        {
            if (conversion == null || !conversion.IsAlive)
            {
                start.Text = "Abort";
                button1.Enabled = false;
                ThreadStart t = new ThreadStart(ConversionThread.Go);
                conversion = new Thread(t);
                conversion.IsBackground = true;
                conversion.Start();
            }
            else
            {
                start.Text = "Start";
                button1.Enabled = true;
                extraction.Value = 0;
                conversion.Abort();
            }
        }
        
        private void salvaStatoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                FileName = "New_Settings",
                DefaultExt = ".ASTOLFO",
                Filter = "HINO Settings File|*.ASTOLFO;|Everything (*.*)|*.*",
                FilterIndex = 1
            };

            DialogResult result = saveFileDialog.ShowDialog();
            if(result == DialogResult.OK)
            {
                Enabled = false;
                StreamWriter file = new StreamWriter(saveFileDialog.FileName);

                foreach(KeyValuePair<string, Network> nn in Program.neuralNetwork)
                {
                    file.Write(nn.Value.Serialize());
                    file.WriteLine("\\" + nn.Key);
                }
                file.WriteLine(">examples");
                foreach (Letter s in Program.examples)
                {
                    file.WriteLine(s.value);
                    string toSave = "";
                    foreach (Coord d in s.pixels)
                        toSave += d.x + "," + d.y + ";";
                    file.WriteLine(toSave);
                }
                file.WriteLine(">settings");
                file.WriteLine(Settings.cutBottom);
                file.WriteLine(Settings.cutTop);
                file.WriteLine(Settings.ignoreTopSubtitles);

                file.WriteLine(ColorTranslator.ToHtml(Settings.subColor));
                file.WriteLine(ColorTranslator.ToHtml(Settings.outSubtitleColor));

                file.WriteLine(Settings.newCharacterThreshold);
                file.WriteLine(Settings.sameCharacterThreshold);
                file.WriteLine(Settings.outlineThreshold);
                file.WriteLine(Settings.outlineWidth);

                file.WriteLine(Settings.minCharPixelSize);
                file.WriteLine(Settings.maxCharPixelSize);
                file.WriteLine(Settings.minSpaceWidth);
                file.WriteLine(Settings.charDistance);
                file.WriteLine(Settings.lineDistance);
                file.WriteLine(Settings.defaultCharScale);

                file.WriteLine(Settings.whiteAndBlack);
                file.WriteLine(Settings.discardNonPassingThroughTheCenterLines);
                file.WriteLine(Settings.discardNonCenteredLines);
                file.WriteLine(Settings.nonCenteredThreshold);
                file.Close();
                Enabled = true;
            }
        }

        private void caricaStatoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                DefaultExt = ".ASTOLFO",
                Filter = "HINO Settings File|*.ASTOLFO;|Everything (*.*)|*.*",
                FilterIndex = 1,
                Multiselect = false
            };

            DialogResult result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                Enabled = false;

                string line;
                List<string> buffer = new List<string>();

                Program.neuralNetwork = new Dictionary<string, Network>();
                Program.examples = new List<Letter>();

                StreamReader file = new StreamReader(openFileDialog.FileName);
                while ((line = file.ReadLine()) != ">examples")
                {
                    if (line[0] != '\\')
                        buffer.Add(line);
                    else
                    {
                        string value = line.Remove(0, 1);

                        Network loaded = new Network(value, 24 * 24, Settings.nnSize, Settings.nnSize);
                        loaded.Load(buffer.ToArray());
                        Program.neuralNetwork.Add(value, loaded);
                        buffer = new List<string>();
                    }
                }

                while ((line = file.ReadLine()) != ">settings")
                {
                    
                    Letter loaded = new Letter();
                    loaded.value = line;
                    line = file.ReadLine();

                    int c = 0;
                    while (c < line.Length)
                    {
                        String toParse = "";
                        for (; line[c] != ','; c++)
                            toParse += line[c];
                        int x = int.Parse(toParse);

                        toParse = "";
                        for (c++; line[c] != ';'; c++)
                            toParse += line[c];
                        int y = int.Parse(toParse);

                        loaded.AddPixel(new Coord(x, y));
                        c++;
                    }
                    loaded.GenerateArray();

                    Program.examples.Add(loaded);
                }

                Settings.cutBottom = int.Parse(file.ReadLine());
                Settings.cutTop = int.Parse(file.ReadLine());
                Settings.ignoreTopSubtitles = bool.Parse(file.ReadLine());

                Settings.subColor = ColorTranslator.FromHtml(file.ReadLine());
                Settings.outSubtitleColor = ColorTranslator.FromHtml(file.ReadLine());

                Settings.newCharacterThreshold = int.Parse(file.ReadLine());
                Settings.sameCharacterThreshold = float.Parse(file.ReadLine());
                Settings.outlineThreshold = float.Parse(file.ReadLine());
                Settings.outlineWidth = int.Parse(file.ReadLine());

                Settings.minCharPixelSize = int.Parse(file.ReadLine());
                Settings.maxCharPixelSize = int.Parse(file.ReadLine());
                Settings.minSpaceWidth = int.Parse(file.ReadLine());
                Settings.charDistance = int.Parse(file.ReadLine());
                Settings.lineDistance = int.Parse(file.ReadLine());
                Settings.defaultCharScale = float.Parse(file.ReadLine());

                Settings.whiteAndBlack = bool.Parse(file.ReadLine());
                Settings.discardNonPassingThroughTheCenterLines = bool.Parse(file.ReadLine());
                Settings.discardNonCenteredLines = bool.Parse(file.ReadLine());
                Settings.nonCenteredThreshold = int.Parse(file.ReadLine());

                file.Close();
                Enabled = true;
            }
        }

        private void salvaSottotitoliToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            /*
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                FileName = "New_Subtitles",
                DefaultExt = ".str",
                Filter = "Subtitles file|*.str;",
                FilterIndex = 1
            };

            DialogResult result = saveFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {*/
                Enabled = false;

                /*
                int index = 0;
                foreach(Subtitle s in ConversionThread.waitForUser)
                {
                    while (index < ConversionThread.subtitles.Count && ConversionThread.subtitles[index].startFrame < s.startFrame)
                        index++;
                    ConversionThread.subtitles.Insert(index, s);
                }
                */
                int z = 0;
                foreach (List<Subtitle> subtitles in ConversionThread.subtitles)
                {
                    string toSave = "";
                    for (int c = 0; c < subtitles.Count; c++)
                    {
                        Subtitle sub = subtitles[c];
                        if (sub.value == "")
                        {
                            subtitles.RemoveAt(c);
                            c--;
                            continue;
                        }

                        int d;
                        for (d = c - 1; d > 0 && subtitles[c].top != subtitles[d].top; d--) ;

                        if (d >= 0 && sub.startFrame < subtitles[d].endFrame)
                            sub.startFrame = subtitles[d].endFrame;

                        toSave += c + "\n";
                        TimeSpan time = TimeSpan.FromSeconds(sub.startFrame / Settings.frameRate);
                        toSave += time.ToString("g");
                        toSave += " --> ";
                        time = TimeSpan.FromSeconds(sub.endFrame / Settings.frameRate);
                        toSave += time.ToString("g");
                        toSave += sub.top ? '\n' + @"{\a6}" + sub.value + '\n' : '\n' + sub.value + '\n';
                    }
                    File.WriteAllText(Settings.saveSubsPath + '/' + Path.GetFileName(Program.videos.Keys.ToList()[z]) + ".srt", toSave);
                    z++;
                }
                Enabled = true;
            //}
        }
        private void saveSubtitlesToolStripMenuItem1_Click(object sender, EventArgs e)
        {

            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.SelectedPath = Settings.saveSubsPath;

            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                Settings.saveSubsPath = dialog.SelectedPath;
            }
        }

        private void listaEsempiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LearningExamples win = new LearningExamples();
            win.Show();
        }

        private void salvaLeModificheAlDizionarioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Enabled = false;
            //Program.dictionary.Sort();
            File.WriteAllLines(Settings.dictionaryPath, Program.dictionary);
            File.WriteAllLines(Settings.properNamesDictionaryPath, Program.namesDictionary);
            Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SettingsRegulation set = new SettingsRegulation();
            set.Show();
        }

        private void apriFinestraSottotitoliToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SubtitlesWindow sub = new SubtitlesWindow();
            sub.Show();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsWindow win = new SettingsWindow();
            win.ShowDialog();
        }

        private void listBox1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])(e.Data.GetData(DataFormats.FileDrop, false));
            foreach (string path in files)
                AddVideo(path);
        }

        private void listBox1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void listBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Delete && listBox1.SelectedItem != null)
            {
                string index = (string)listBox1.SelectedItem;
                Program.videos.Remove(index);
                listBox1.Items.RemoveAt(listBox1.SelectedIndex);
            }
        }

        public delegate void EventHandle();
    }
}

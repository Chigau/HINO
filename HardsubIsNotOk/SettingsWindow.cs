using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HardsubIsNotOk
{
    public partial class SettingsWindow : Form
    {
        public SettingsWindow()
        {
            InitializeComponent();
            textBox2.Text = Settings.dictionaryPath;
            textBox1.Text = Settings.properNamesDictionaryPath;
            textBox3.Text = Settings.frameRate.ToString();
            checkBox1.Checked = Settings.dictionaryMode;

            textBox4.Text = Settings.maxError.ToString();
            textBox5.Text = (Settings.minCorrectness * 100).ToString();

            textBox7.Text = Settings.maxDictionaryError.ToString();
            textBox6.Text = (Settings.minDictionaryCorrectness * 100).ToString();

            textBox9.Text = Settings.maxLearningThreads.ToString();
            checkBox2.Checked = Settings.learningDisabled;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (Settings.dictionaryPath != textBox2.Text)
                {
                    Settings.dictionaryPath = textBox2.Text;
                    if (File.Exists(Settings.dictionaryPath))
                    {
                        Program.dictionary = new List<string>(File.ReadAllLines(Settings.dictionaryPath));
                        Program.dictionary.Sort();
                    }
                    else
                    {
                        File.Create(Settings.dictionaryPath).Dispose();
                        Program.dictionary = new List<string>();
                    }
                }
                if (Settings.properNamesDictionaryPath != textBox1.Text)
                {
                    Settings.properNamesDictionaryPath = textBox1.Text;
                    if (File.Exists(Settings.properNamesDictionaryPath))
                    {
                        Program.namesDictionary = new List<string>(File.ReadAllLines(Settings.properNamesDictionaryPath));
                        Program.namesDictionary.Sort();
                    }
                    else
                    {
                        File.Create(Settings.properNamesDictionaryPath).Dispose();
                        Program.namesDictionary = new List<string>();
                    }
                }
                Settings.frameRate = float.Parse(textBox3.Text);
                Settings.dictionaryMode = checkBox1.Checked;
                Settings.maxError = float.Parse(textBox4.Text);
                Settings.minCorrectness = float.Parse(textBox5.Text) / 100;
                Settings.maxDictionaryError = float.Parse(textBox7.Text);
                Settings.minDictionaryCorrectness = float.Parse(textBox6.Text) / 100;
                Settings.maxLearningThreads = int.Parse(textBox9.Text);
                Settings.learningDisabled = checkBox2.Checked;

                StreamWriter file = new StreamWriter("settings.txt");
                file.WriteLine(Settings.dictionaryPath);
                file.WriteLine(Settings.properNamesDictionaryPath);
                file.WriteLine(Settings.frameRate);
                file.WriteLine(Settings.maxError);
                file.WriteLine(Settings.minCorrectness);
                file.WriteLine(Settings.maxDictionaryError);
                file.WriteLine(Settings.minDictionaryCorrectness);
                file.WriteLine(Settings.maxLearningThreads);
                file.Close();
                Close();
            }
            catch(FormatException ex)
            {
                MessageBox.Show(ex.Message, "Format Exception", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}

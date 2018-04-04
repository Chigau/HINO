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
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Settings.dictionaryPath = textBox2.Text;
                Settings.properNamesDictionaryPath = textBox1.Text;
                Settings.frameRate = double.Parse(textBox3.Text);
                Settings.dictionaryMode = checkBox1.Checked;

                Settings.maxError = double.Parse(textBox4.Text);
                Settings.minCorrectness = double.Parse(textBox5.Text) / 100;

                Settings.maxDictionaryError = double.Parse(textBox7.Text);
                Settings.minDictionaryCorrectness = double.Parse(textBox6.Text) / 100;
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

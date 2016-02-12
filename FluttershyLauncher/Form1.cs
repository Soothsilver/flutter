using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StarSwirlLauncher
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string directory = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            string pathtorealgame = System.IO.Path.Combine(directory, "Fluttershy's Hearthswarming Adventure.exe");
            string res = this.comboBox1.Text;
            if (res.Contains(' ')) res = res.Substring(0, res.IndexOf(' '));
            string parameters = res + " " + (this.checkBox1.Checked ? "fullscreen" : "windowed") + " " + (this.checkBox2.Checked ? "donottrack" : "trackatwill");
            System.Diagnostics.Process.Start(pathtorealgame, parameters);
            this.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.comboBox1.SelectedIndex = 0;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}

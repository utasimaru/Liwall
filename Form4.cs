using System;
using System.IO;
using System.Windows.Forms;

namespace Liwall
{
    public partial class Form4 : Form
    {
        Form5 f5;
        public Form4()
        {
            InitializeComponent();
            var fullpaths = Directory.GetFiles(Directory.GetCurrentDirectory()+"/DLLs", "*.dll", System.IO.SearchOption.AllDirectories);
            foreach (string path in fullpaths)
            {
                comboBox1.Items.Add(Path.GetFileName(path));
            }
            f5 = new Form5();
            f5.Show();
        }
        private void FrameRateTextBox_TextChanged(object sender, EventArgs e)
        {
            int.TryParse(FrameRateTextBox.Text, out int value);
            if(value>0)Program.framerate = 1000 / value;
        }
        private void LoadButton_Click(object sender, EventArgs e)
        {
            //this.Visible = false;
            if (comboBox1.SelectedIndex>-1)f5.DLLLoadPlay("DLLs/"+comboBox1.Text);
        }
        private void Form4_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.drawing = false;
        }
    }
}

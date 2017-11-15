using System;
using System.IO;
using System.Windows.Forms;

namespace САПР5
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Form2 f = new Form2();

        private void Form1_Load(object sender, EventArgs e)
        {
            richTextBox1.Text = File.ReadAllText(textBox1.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog FileDialog = new OpenFileDialog();
            button3.Enabled = false;
            FileDialog.Filter = "Файл .mv|*.mv";
            if (FileDialog.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = FileDialog.FileName;
                richTextBox1.Text = File.ReadAllText(textBox1.Text);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            f.FillTable();
            f.ShowDialog();
            button3.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form3 ff = new Form3(f);
            ff.Analysis(richTextBox1.Text);
            ff.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            button3.Enabled = false;
            File.WriteAllText(@textBox1.Text, richTextBox1.Text);
        }
    }
}

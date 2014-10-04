using System;
using System.Windows.Forms;
using System.Resources;
using System.IO;
using Cooperativeness.OBA.Word.ResourceGenerator;

namespace Cooperativeness.OBA.Word.ResourceTools
{
    public partial class FormResource : Form
    {
        public FormResource()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            label1.Text = "";
            var dialog = new OpenFileDialog();
            dialog.Filter = "资源文件|*.xml";
            dialog.Multiselect = false;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.textBox1.Text = dialog.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string resourceXml = this.textBox1.Text.Trim();
            if (string.IsNullOrEmpty(resourceXml)) return;
            ResourceAdmin.Generate(resourceXml);
            label1.Text = "OK.";
        }

    }
}

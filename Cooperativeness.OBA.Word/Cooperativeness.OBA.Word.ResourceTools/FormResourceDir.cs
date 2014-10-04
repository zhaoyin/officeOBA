using System;
using System.Windows.Forms;
using Cooperativeness.OBA.Word.ResourceGenerator;

namespace Cooperativeness.OBA.Word.ResourceTools
{
    public partial class FormResourceDir : Form
    {
        public FormResourceDir()
        {
            InitializeComponent();
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            string sourceDir = txtSource.Text.Trim();
            if(string.IsNullOrEmpty(sourceDir)) return;
            string targetDir = txtTarget.Text.Trim();
            if(string.IsNullOrEmpty(targetDir)) return;
            ResourceAdmin.Generator(sourceDir,targetDir);
            lblResult.Text = "OK!";
        }
    }
}

using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Cooperativeness.FileTransfer;
using Cooperativeness.OBA.Word.AddIns.FrameWork;
using OfficeWord = Microsoft.Office.Interop.Word;

namespace Cooperativeness.OBA.Word.DocumentScene.Plugin.Forms
{
    public partial class FilesForm : Form
    {
        private IBundleContext bundleContext = null;
        public FilesForm(IBundleContext context, OfficeWord.Application app)
        {
            InitializeComponent();
            bundleContext = context;
            //client.Upload(@"C:\Documents and Settings\zhaoyin3\桌面\Process Monitor 助力Windows用户态程序高效排错.docx");
            string[] files = SceneContext.Instance.FileServer.GetFiles("", "*.*.txt",false);
            FileGrid.Rows.Clear();
            if (files != null && files.Length > 0)
            {
                foreach (var file in files)
                {
                    FileGrid.Rows.Add(file.Replace(".txt",""));
                }
            }
        }

        private string _fileName;
        public string LocalFileName
        {
            get { return _fileName; }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            try
            {
                int selectRow = FileGrid.SelectedCells[0].RowIndex;
                string value2 = FileGrid.Rows[selectRow].Cells[0].Value.ToString();
                string destFile = bundleContext.Bundle.Location + @"\documents\" + value2;
                if (SceneContext.Instance.FileServer.Download(value2, destFile))
                {
                    _fileName = destFile;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                this.DialogResult=DialogResult.OK;
                this.Close();
            }
            
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

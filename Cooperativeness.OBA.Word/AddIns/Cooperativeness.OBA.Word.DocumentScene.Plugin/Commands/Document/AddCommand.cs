using System.IO;
using System.Windows.Forms;
using Cooperativeness.OBA.Word.DocumentScene.Plugin.Framework;
using Cooperativeness.OBA.Word.Tools;
using OfficeWord = Microsoft.Office.Interop.Word;
using Cooperativeness.OBA.Word.Ribbon;
using Cooperativeness.OBA.Word.Ribbon.Command;
using Cooperativeness.OBA.Word.Ribbon.Model;

namespace Cooperativeness.OBA.Word.DocumentScene.Plugin.Commands.Document
{
    public class AddCommand : RibbonButtonCommand
    {
        public override string GetLabel()
        {
            return SceneContext.Instance.GetString(this.XRibbonElement.Id);
        }

        protected override void OnButtonAction(XRibbonButton xRibbonButton, RibbonEventArgs e)
        {
            //Commands.DocumentAdmin.Execute(SceneContext.Instance);
            OfficeWord.Application app = SceneContext.Instance.WordAppAdmin.Application;
            OfficeWord.Document doc = app.ActiveDocument;
            if (doc != null)
            {
                doc.Save();
                var entry = new SecretEntry()
                {
                    LoginEntity = null,
                    BizFileName = doc.FullName,
                    BizFileId = "",
                };
                SceneContext.Instance.SecretDataAdmin.Set(doc,entry);
                string filename = doc.FullName;
                string tempFileName = Path.Combine(Logger.TemporaryDirectory, doc.Name);
                File.Copy(filename, tempFileName);
                if (SceneContext.Instance.FileServer.Upload(tempFileName, doc.Name))
                {
                    MessageBox.Show("文件新增成功!");
                }
                File.Delete(tempFileName);
            }
        }

        public override bool GetVisible()
        {
            return SceneContext.Instance.IsLogin;
        }

        public override RibbonSizeMode GetSizeMode()
        {
            return RibbonSizeMode.SizeLarge;
        }

        public override string GetSupertip()
        {
            return GetLabel();
        }
    }
}

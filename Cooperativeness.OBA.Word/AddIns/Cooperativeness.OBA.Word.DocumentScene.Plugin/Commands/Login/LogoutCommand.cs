using System;
using System.IO;
using System.Windows.Forms;
using Cooperativeness.OBA.Word.Ribbon;
using Cooperativeness.OBA.Word.Ribbon.Command;
using Cooperativeness.OBA.Word.Ribbon.Model;
using Cooperativeness.OBA.Word.Tools;
using OfficeWord = Microsoft.Office.Interop.Word;

namespace Cooperativeness.OBA.Word.DocumentScene.Plugin.Commands.Login
{
    public class LogoutCommand : RibbonButtonCommand
    {
        public override string GetLabel()
        {
            return SceneContext.Instance.GetString(this.XRibbonElement.Id);
        }

        protected override void OnButtonAction(XRibbonButton xRibbonButton, RibbonEventArgs e)
        {
            SceneContext.Instance.Logout();
            xRibbonButton.Invalidate(); //刷新
            //Commands.DocumentAdmin.Execute(SceneContext.Instance);
            OfficeWord.Document doc = SceneContext.Instance.WordAppAdmin.ActiveDocument;
            if (doc != null)
            {
                if (SceneContext.Instance.SecretDataAdmin.Get(doc) == null)
                {
                    //MessageBox.Show("该文档不是通过文档工具打开的文档,无法使用此功能!");
                }
                else
                {
                    SceneContext.Instance.SecretDataAdmin.Remove(doc);
                    doc.Save();
                    string filename = doc.FullName;
                    string tempFileName = Path.Combine(Logger.TemporaryDirectory, doc.Name);
                    File.Copy(filename, tempFileName);
                    if (SceneContext.Instance.FileServer.Upload(tempFileName, doc.Name))
                    {
                        MessageBox.Show("文件保存成功!");
                    }
                    // 缺省参数
                    object unknown = Type.Missing;
                    doc.Close(ref unknown, ref unknown, ref unknown);
                    File.Delete(tempFileName);
                    File.Delete(filename);
                }
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

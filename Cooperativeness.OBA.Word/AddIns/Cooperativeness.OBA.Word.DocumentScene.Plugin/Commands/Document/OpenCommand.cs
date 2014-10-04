using System.IO;
using System.Windows.Forms;
using Cooperativeness.OBA.Word.DocumentScene.Plugin.Forms;
using Cooperativeness.OBA.Word.DocumentScene.Plugin.Framework;
using Cooperativeness.OBA.Word.Ribbon;
using Cooperativeness.OBA.Word.Ribbon.Command;
using Cooperativeness.OBA.Word.Ribbon.Model;
using OfficeWord = Microsoft.Office.Interop.Word;
using Cooperativeness.OBA.Word.AddIns.FrameWork;

namespace Cooperativeness.OBA.Word.DocumentScene.Plugin.Commands.Document
{
    public class OpenCommand:RibbonButtonCommand
    {
        private IBundleContext context = SceneContext.Instance.BundleContext;
        public override string GetLabel()
        {
            return SceneContext.Instance.GetString(this.XRibbonElement.Id);
        }

        protected override void OnButtonAction(XRibbonButton xRibbonButton, RibbonEventArgs e)
        {
            OfficeWord.Application app = SceneContext.Instance.WordAppAdmin.Application;

            var file = new FilesForm(context,app);
            if (file.ShowDialog() == DialogResult.OK)
            {
                if(string.IsNullOrEmpty(file.LocalFileName)) return;
                // 关闭当前的文档
                object missing = System.Reflection.Missing.Value;

                var newFile = new FileInfo(file.LocalFileName);
                if (newFile.Exists)
                {
                    // 注入用户数据
                    //CreateCustomXml(openFileName, loginObj);

                    // 打开该文件
                    object fileNameObj = file.LocalFileName;
                    object visible = true;
                    app.ScreenUpdating = false;
                    OfficeWord.Document wDoc = app.Documents.Open(ref fileNameObj, ref missing, ref missing
                                                                    , ref missing, ref missing, ref missing
                                                                    , ref missing, ref missing, ref missing
                                                                    , ref missing, ref missing, ref visible
                                                                    , ref missing, ref missing, ref missing
                                                                    , ref missing);
                    app.ScreenUpdating = true;
                    // 记录当前文档数据
                    var entry = new SecretEntry()
                    {
                        LoginEntity = null,
                        BizFileName = file.LocalFileName,
                        BizFileId = "",
                    };
                    SceneContext.Instance.SecretDataAdmin.Set(wDoc,entry);

                    //wDoc.ActiveWindow.View.Type = Microsoft.Office.Interop.Word.WdViewType.wdPrintView;
                    // 记录当前文档数据
                    //SecretEntry entry = new SecretEntry()
                    //{
                    //    LoginEntity = loginEntity,
                    //    BizFileName = fileInfo.FullName,
                    //    BizFileId = fileInfo.FileId,
                    //    AuthAdmin = secretEntry.AuthAdmin,
                    //};
                    //context.SecretDataAdmin.Set(wDoc, entry);

                    // 获取数据
                    //string xml = wDoc.SelectPartXmlByNamespace("---namespace----");

                    //object saveChange = false;
                    //doc.Close(ref saveChange, ref nullobj, ref nullobj);
                }
            }



            //var dialog = new F.OpenFileDialog();
            //dialog.InitialDirectory = @"C:\";
            //dialog.Filter = "所有Word文档|*.docx;*.docm;*.dotx;*.doc;*.dot;*.htm;*.html|Word文档|*.docx|启用宏的Word文档|*.docm|Xml文件|*.xml|Word97-2003文档|*.doc";
            //dialog.FilterIndex = 1;
            //if (dialog.ShowDialog() == F.DialogResult.OK)
            //{
            //    string fileName = dialog.FileName;
            //    //// 检查文件是否已打开
            //    //string fileName = context.WordAppAdmin.Application.ActiveDocument.Name;
            //    //if (fileName.Equals(dialog.FileName))
            //    //{
            //    //    MessageBox.Show(string.Format("文件 {0}已打开.",fileName));
            //    //    return false;
            //    //}
            
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


using System;
using System.IO;
using Cooperativeness.OBA.Word.Ribbon;
using Cooperativeness.OBA.Word.Ribbon.Command;
using Cooperativeness.OBA.Word.Ribbon.Model;
using Microsoft.Office.Tools;

namespace Cooperativeness.OBA.Word.DocumentScene.Plugin.Commands.Document
{
    public class CloseCommand:RibbonButtonCommand
    {
        public override string GetLabel()
        {
            return SceneContext.Instance.GetString(this.XRibbonElement.Id);
        }

        protected override void OnButtonAction(XRibbonButton xRibbonButton, RibbonEventArgs e)
        {
            // 缺省参数
            object unknown = Type.Missing;
            //doc.Close(ref saveChange, ref nullobj, ref nullobj);
            var doc = SceneContext.Instance.WordAppAdmin.ActiveDocument;
            string docName="";
            if (doc != null)
            {
                docName = doc.FullName;
                SceneContext.Instance.SecretDataAdmin.Remove(doc);
                object saveChanges = false;
                doc.Close(ref saveChanges, ref unknown, ref unknown);
            }
            //删除此文件
            if(!string.IsNullOrEmpty(docName))  File.Delete(docName);

            var panes = SceneContext.Instance.CustomTaskPanesAdmin.CustomTaskPanes;
            int panesCount = panes.Count;
            if (panesCount > 0)
            {
                CustomTaskPane taskPane = panes[0];
                if (taskPane.Visible)
                {
                    taskPane.Visible = false;
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

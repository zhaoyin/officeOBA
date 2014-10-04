
using System;
using Cooperativeness.OBA.Word.DocumentScene.Plugin.Control;
using Cooperativeness.OBA.Word.Ribbon;
using Cooperativeness.OBA.Word.Ribbon.Command;
using Cooperativeness.OBA.Word.Ribbon.Model;
using Microsoft.Office.Tools;
using Cooperativeness.OBA.Word.Tools;

namespace Cooperativeness.OBA.Word.DocumentScene.Plugin.Commands.Document
{
    public class VersionCommand : RibbonButtonCommand
    {
        private static readonly Logger Log = new Logger(typeof(VersionCommand));
        public override string GetLabel()
        {
            return SceneContext.Instance.GetString(this.XRibbonElement.Id);
        }

        protected override void OnButtonAction(XRibbonButton xRibbonButton, RibbonEventArgs e)
        {
            Log.Debug("---VersionCommand--Begin:" + DateTime.Now.ToLongTimeString());
            CustomTaskPaneCollection panes = SceneContext.Instance.CustomTaskPanesAdmin.CustomTaskPanes;
            int panesCount = panes.Count;
            Log.Debug("---VersionCommand--01:" + DateTime.Now.ToLongTimeString());
            if (panesCount == 0)
            {
                var pane=new CustomPane();
                Log.Debug("---VersionCommand--02:" + DateTime.Now.ToLongTimeString());
                var host = new PaneDecorator(pane);
                Log.Debug("---VersionCommand--03:" + DateTime.Now.ToLongTimeString());
                panes.Add(host, "版本信息");
                Log.Debug("---VersionCommand--04:" + DateTime.Now.ToLongTimeString());
            }
            CustomTaskPane taskPane = panes[0];
            taskPane.VisibleChanged += new EventHandler(taskPane_VisibleChanged);
            taskPane.Width = 430;
            taskPane.Visible = true;
        }

        void taskPane_VisibleChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
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


using System;
using System.IO;
using Cooperativeness.OBA.Word.Ribbon;
using Cooperativeness.OBA.Word.Ribbon.Command;
using Cooperativeness.OBA.Word.Ribbon.Model;
using System.Security.AccessControl;

namespace Cooperativeness.OBA.Word.DocumentScene.Plugin.Commands.Login
{
    public class LoginCommand : RibbonButtonCommand
    {
        public override string GetLabel()
        {
            return SceneContext.Instance.GetString(this.XRibbonElement.Id);
        }

        protected override void OnButtonAction(XRibbonButton xRibbonButton, RibbonEventArgs e)
        {
            if (SceneContext.Instance.Login())
            {
                xRibbonButton.Invalidate();
            }
            string path = Path.Combine(SceneContext.Instance.BundleContext.Bundle.Location, "Documents");
            if (!Directory.Exists(path))
            {
                //DirectorySecurity security=new DirectorySecurity();
                //security.AddAccessRule(new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, InheritanceFlags.ContainerInherit, PropagationFlags.InheritOnly, AccessControlType.Allow));
                Directory.CreateDirectory(path);
            }
        }

        public override bool GetVisible()
        {
            return !SceneContext.Instance.IsLogin;
        }

        public override RibbonSizeMode GetSizeMode()
        {
            return RibbonSizeMode.SizeLarge;
        }

        public override string GetSupertip()
        {
            return GetLabel();
        }

        public override bool GetEnabled()
        {
            return true;
        }
    }
}


using Cooperativeness.OBA.Word.Ribbon.Command;

namespace Cooperativeness.OBA.Word.DocumentScene.Plugin.Commands.Document
{
    public class DocumentAdminGroupCommand : RibbonGroupCommand
    {
        public override string GetLabel()
        {
            return SceneContext.Instance.GetString(this.XRibbonElement.Id);
        }

        public override bool GetVisible()
        {
            return SceneContext.Instance.IsLogin;
        }

        public override string GetSupertip()
        {
            return GetLabel();
        }
    }
}


using Cooperativeness.OBA.Word.Ribbon.Command;

namespace Cooperativeness.OBA.Word.DocumentScene.Plugin.Commands.Login
{
    public class LoginAdminGroupCommand : RibbonGroupCommand
    {
        public override string GetLabel()
        {
            return SceneContext.Instance.GetString(this.XRibbonElement.Id);
        }

        public override bool GetVisible()
        {
            return true;
        }

        public override string GetSupertip()
        {
            return GetLabel();
        }
    }
}


using Cooperativeness.OBA.Word.Ribbon;
using Cooperativeness.OBA.Word.Ribbon.Command;

namespace Cooperativeness.OBA.Word.DocumentScene.Plugin.Commands.Test
{
    public class TestMenuCommand : RibbonMenuCommand
    {
        public override string GetLabel()
        {
            return SceneContext.Instance.GetString(this.XRibbonElement.Id);
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

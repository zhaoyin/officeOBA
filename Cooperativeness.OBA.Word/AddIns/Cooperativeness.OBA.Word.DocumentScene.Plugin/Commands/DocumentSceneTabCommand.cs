using Cooperativeness.OBA.Word.Ribbon.Command;

namespace Cooperativeness.OBA.Word.DocumentScene.Plugin.Commands
{
    /// <summary>
    /// 定义场景设计页签命令对象
    /// </summary>
    public class DocumentSceneTabCommand : RibbonTabCommand
    {
        public override string GetLabel()
        {
            return SceneContext.Instance.GetString(this.XRibbonElement.Id);
        }
    }
}

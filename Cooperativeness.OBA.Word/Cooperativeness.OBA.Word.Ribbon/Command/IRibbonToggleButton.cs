
namespace Cooperativeness.OBA.Word.Ribbon.Command
{
    public interface IRibbonToggleButtonCommand : IRibbonButtonCommand, IRibbonAction
    {
        /// <summary>
        /// 获取当前按钮是否按下的状态
        /// </summary>
        /// <returns></returns>
        bool GetPressed();
    }
}

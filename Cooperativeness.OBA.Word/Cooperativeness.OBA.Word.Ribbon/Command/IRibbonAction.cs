using Cooperativeness.OBA.Word.Ribbon.Model;

namespace Cooperativeness.OBA.Word.Ribbon.Command
{
    /// <summary>
    /// 定义功能区按钮动作
    /// </summary>
    public interface IRibbonAction
    {
        /// <summary>
        /// 处理按钮点击操作
        /// </summary>
        /// <param name="elment"></param>
        /// <param name="e"></param>
        void OnAction(RibbonElement elment, RibbonEventArgs e);
    }
}

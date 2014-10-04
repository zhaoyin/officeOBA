
namespace Cooperativeness.OBA.Word.Ribbon.Command
{
    /// <summary>
    /// 定义功能区分割按钮的命令接口契约
    /// </summary>
    public interface IRibbonSeparatorCommand
    {
        /// <summary>
        /// 获取当前分割按钮可视状态
        /// </summary>
        /// <returns></returns>
        bool GetVisible();
    }
}

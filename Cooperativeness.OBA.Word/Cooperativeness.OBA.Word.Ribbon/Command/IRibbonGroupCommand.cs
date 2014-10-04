
namespace Cooperativeness.OBA.Word.Ribbon.Command
{
    /// <summary>
    /// 定义功能区分组命令接口契约
    /// </summary>
    public interface IRibbonGroupCommand
    {
        /// <summary>
        /// 获取快捷键提示信息
        /// </summary>
        /// <returns></returns>
        string GetKeytip();

        /// <summary>
        /// 获取分组显示名称
        /// </summary>
        /// <returns></returns>
        string GetLabel();

        /// <summary>
        /// 获取屏幕提示信息
        /// </summary>
        /// <returns></returns>
        string GetScreentip();

        /// <summary>
        /// 获取上级提示信息
        /// </summary>
        /// <returns></returns>
        string GetSupertip();

        /// <summary>
        /// 获取当前分组可视状态
        /// </summary>
        /// <returns></returns>
        bool GetVisible();
    }
}

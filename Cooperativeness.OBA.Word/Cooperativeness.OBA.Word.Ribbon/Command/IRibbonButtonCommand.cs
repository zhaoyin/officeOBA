
namespace Cooperativeness.OBA.Word.Ribbon.Command
{
    /// <summary>
    /// 定义功能区按钮命令接口契约
    /// </summary>
    public interface IRibbonButtonCommand : IRibbonAction
    {
        /// <summary>
        /// 获取按钮可用状态
        /// </summary>
        /// <returns></returns>
        bool GetEnabled();

        /// <summary>
        /// 获取按钮的可视状态
        /// </summary>
        /// <returns></returns>
        bool GetVisible();

        /// <summary>
        /// 获取按钮的描述信息
        /// </summary>
        /// <returns></returns>
        string GetDescription();

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
        /// 获取按钮的大小模式
        /// </summary>
        /// <returns></returns>
        RibbonSizeMode GetSizeMode();

        /// <summary>
        /// 获取显示名称的显示状态
        /// </summary>
        /// <returns></returns>
        bool GetShowLabel();

        /// <summary>
        /// 获取按钮的显示状态
        /// </summary>
        /// <returns></returns>
        bool GetShowImage();
    }
}

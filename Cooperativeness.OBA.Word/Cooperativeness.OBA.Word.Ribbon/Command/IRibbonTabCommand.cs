
namespace Cooperativeness.OBA.Word.Ribbon.Command
{
    /// <summary>
    /// 定义功能区页签命令接口契约
    /// </summary>
    public interface IRibbonTabCommand
    {
        /// <summary>
        /// 获取按钮可视状态
        /// </summary>
        bool GetVisible();

        /// <summary>
        /// 获取按键提示信息
        /// </summary>
        string GetKeytip();

        /// <summary>
        /// 获取显示名称
        /// </summary>
        string GetLabel();

        /// <summary>
        /// 获取用户数据
        /// </summary>
        /// <returns></returns>
        string GetTag();
    }
}

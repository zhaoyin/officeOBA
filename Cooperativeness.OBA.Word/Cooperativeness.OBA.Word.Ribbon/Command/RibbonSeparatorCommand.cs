
namespace Cooperativeness.OBA.Word.Ribbon.Command
{
    /// <summary>
    /// 定义分隔符按钮命令对象
    /// </summary>
    public class RibbonSeparatorCommand : IRibbonSeparatorCommand
    {
        #region IRibbonSeparatorCommand 成员

        public virtual bool GetVisible()
        {
            return true;
        }

        #endregion
    }
}

using Cooperativeness.OBA.Word.Ribbon.Model;

namespace Cooperativeness.OBA.Word.Ribbon.Command
{
    /// <summary>
    /// 定义抽象的分组命令对象
    /// </summary>
    public abstract class RibbonGroupCommand : IRibbonGroupCommand
    {
        #region 属性
        /// <summary>
        /// 获取或设置功能区分组元素
        /// </summary>
        public XRibbonGroup XRibbonElement { get; internal set; }

        #endregion

        #region IRibbonGroupCommand 成员

        public virtual string GetKeytip()
        {
            return string.Empty;
        }

        public abstract string GetLabel();

        public virtual string GetScreentip()
        {
            return string.Empty;
        }

        public virtual string GetSupertip()
        {
            return string.Empty;
        }

        public virtual bool GetVisible()
        {
            return true;
        }

        #endregion
    }
}

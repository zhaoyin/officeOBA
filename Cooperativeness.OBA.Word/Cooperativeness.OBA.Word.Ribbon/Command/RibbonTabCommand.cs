using Cooperativeness.OBA.Word.Ribbon.Model;

namespace Cooperativeness.OBA.Word.Ribbon.Command
{
    /// <summary>
    /// 定义抽象的页签命令对象
    /// </summary>
    public abstract class RibbonTabCommand:IRibbonTabCommand
    {
        #region 属性
        /// <summary>
        /// 获取或设置功能区页签元素
        /// </summary>
        public XRibbonTab XRibbonElement { get; internal set; }

        #endregion

        #region IRibbonTabCommand 成员

        public virtual bool GetVisible()
        {
            return true;
        }

        public virtual string GetKeytip()
        {
            return string.Empty;
        }

        public abstract string GetLabel();

        public virtual string GetTag()
        {
            return string.Empty;
        }

        #endregion
    }
}

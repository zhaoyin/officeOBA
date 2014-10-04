using Cooperativeness.OBA.Word.Ribbon.Model;

namespace Cooperativeness.OBA.Word.Ribbon.Command
{
    /// <summary>
    /// 定义抽象的菜单命令对象
    /// </summary>
    public abstract class RibbonMenuCommand : IRibbonMenuCommand
    {
        #region 属性
        /// <summary>
        /// 获取或设置功能区下拉按钮元素
        /// </summary>
        public XRibbonMenu XRibbonElement { get; internal set; }

        #endregion

        #region IRibbonMenuCommand 成员

        public virtual bool GetEnabled()
        {
            return true;
        }

        public virtual bool GetVisible()
        {
            return true;
        }

        public virtual string GetDescription()
        {
            return string.Empty;
        }

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

        public virtual RibbonSizeMode GetSizeMode()
        {
            return RibbonSizeMode.SizeLarge;
        }

        public virtual bool GetShowLabel()
        {
            return true;
        }

        public virtual bool GetShowImage()
        {
            return true;
        }

        #endregion
    }
}

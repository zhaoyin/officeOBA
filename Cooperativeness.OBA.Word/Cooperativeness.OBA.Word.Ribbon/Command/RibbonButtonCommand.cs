using Cooperativeness.OBA.Word.Ribbon.Model;

namespace Cooperativeness.OBA.Word.Ribbon.Command
{
    /// <summary>
    /// 定义抽象的按钮命令对象
    /// </summary>
    public abstract class RibbonButtonCommand : IRibbonButtonCommand
    {
        #region 属性
        /// <summary>
        /// 获取或设置功能区按钮元素
        /// </summary>
        public XRibbonButton XRibbonElement { get; internal set; }

        #endregion

        #region IRibbonButtonCommand 成员

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
            return RibbonSizeMode.SizeRegular;
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

        #region IRibbonAction 成员

        public void OnAction(RibbonElement elment, RibbonEventArgs e)
        {
            if (e == null) return;
            var xRibbonButton = (XRibbonButton)elment;
            if (xRibbonButton == null) return;

            this.OnButtonAction(xRibbonButton, e);
        }

        protected abstract void OnButtonAction(XRibbonButton xRibbonButton, RibbonEventArgs e);

        #endregion
    }
}

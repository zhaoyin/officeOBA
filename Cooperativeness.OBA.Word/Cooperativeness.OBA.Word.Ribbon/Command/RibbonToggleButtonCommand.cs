using Cooperativeness.OBA.Word.Ribbon.Model;

namespace Cooperativeness.OBA.Word.Ribbon.Command
{
    /// <summary>
    /// 定义抽象的切换按钮命令对象
    /// </summary>
    public abstract class RibbonToggleButtonCommand : IRibbonToggleButtonCommand
    {
        #region 属性
        /// <summary>
        /// 获取或设置功能区开关按钮元素
        /// </summary>
        public XRibbonToggleButton XRibbonElement { get; internal set; }

        #endregion

        #region IRibbonToggleButtonCommand 成员

        public virtual bool GetPressed()
        {
            return false;
        }

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
            var args = (RibbonToggleButtonEventArgs)e;
            if (e == null) return;
            var xRibbonToggleButton = (XRibbonToggleButton)elment;
            if (xRibbonToggleButton == null) return;


            this.OnToggleButtonAction(xRibbonToggleButton, args);
        }

        protected abstract void OnToggleButtonAction(XRibbonToggleButton xRibbonToggleButton, RibbonToggleButtonEventArgs e);

        #endregion
    }
}

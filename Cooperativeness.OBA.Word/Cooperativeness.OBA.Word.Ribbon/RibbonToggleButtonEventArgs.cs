using Office = Microsoft.Office.Core;

namespace Cooperativeness.OBA.Word.Ribbon
{
    /// <summary>
    /// 定义切换按钮参数事件对象
    /// </summary>
    public class RibbonToggleButtonEventArgs : RibbonEventArgs
    {
        #region 字段
        private bool pressed;

        #endregion

        #region 构造函数
        public RibbonToggleButtonEventArgs(Office.IRibbonControl ribbonControl,bool isPressed)
            : base(ribbonControl)
        {
            this.pressed = isPressed;
        }

        public RibbonToggleButtonEventArgs(Office.IRibbonControl ribbonControl, bool cancel,bool isPressed)
            : base(ribbonControl,cancel)
        {
            this.pressed = isPressed;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取一个值，用来指示当前按钮是否处于按下状态
        /// </summary>
        public bool Pressed
        {
            get { return this.pressed; }
        }

        #endregion
    }
}

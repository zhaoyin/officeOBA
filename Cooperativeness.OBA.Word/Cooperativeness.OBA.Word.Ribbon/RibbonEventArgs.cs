using System.ComponentModel;
using Office = Microsoft.Office.Core;

namespace Cooperativeness.OBA.Word.Ribbon
{
    /// <summary>
    /// 定义功能区参数对象
    /// </summary>
    public class RibbonEventArgs : CancelEventArgs
    {
        #region 字段
        private Office.IRibbonControl ribbonControl;

        #endregion

        #region 构造函数
        public RibbonEventArgs(Office.IRibbonControl ribbonControl)
            : base()
        {
            this.ribbonControl = ribbonControl;
        }

        public RibbonEventArgs(Office.IRibbonControl ribbonControl, bool cancel)
            : base(cancel)
        {
            this.ribbonControl = ribbonControl;
        }

        #endregion

        #region 属性
        /// <summary>
        /// 获取功能区按钮对象
        /// </summary>
        public Office.IRibbonControl RibbonControl
        {
            get { return this.ribbonControl; }
        }

        public object Context
        {
            get { return this.ribbonControl.Context; }
        }

        public string Id
        {
            get { return this.ribbonControl.Id; }
        }

        public string Tag
        {
            get { return this.ribbonControl.Tag; }
        }

        #endregion
    }
}

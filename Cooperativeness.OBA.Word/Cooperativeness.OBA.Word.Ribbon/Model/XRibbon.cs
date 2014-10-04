using System.Collections.Generic;
using System.Xml.Linq;
using Cooperativeness.OBA.Word.AddIns.FrameWork;
using Office = Microsoft.Office.Core;

namespace Cooperativeness.OBA.Word.Ribbon.Model
{
    /// <summary>
    /// 定义功能区元素对象
    /// </summary>
    public class XRibbon : CompositeRibbonElement
    {
        #region 字段
        private Office.IRibbonUI ribbonUI;

        #endregion

        #region 构造方法
        public XRibbon()
        {
        }

        public XRibbon(params RibbonElement[] childElements)
            : base(childElements)
        {
        }

        public XRibbon(IEnumerable<RibbonElement> childElements)
            : base(childElements)
        {
        }

        #endregion

        #region 属性
        /// <summary>
        /// 获取当前功能区所有的页签对象列表
        /// </summary>
        public XRibbonTabs Tabs
        {
            get
            {
                return base.GetElement<XRibbonTabs>();
            }
        }

        /// <summary>
        /// 获取或设置功能区UI对象
        /// </summary>
        internal Office.IRibbonUI RibbonUI
        {
            get { return this.ribbonUI; }
            set
            {
                if (this.ribbonUI != null)
                    return;
                this.ribbonUI = value;
            }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 根据XML对象设置当前元素的属性值
        /// </summary>
        /// <param name="element"></param>
        public override bool InitProperties(XElement element, IBundle bundle)
        {
            return true;
        }

        /// <summary>
        /// 序列化处理
        /// </summary>
        /// <returns></returns>
        protected override XElement OnSerialize()
        {
            try
            {
                string xml = "<ribbon></ribbon>";
                XElement element = XElement.Parse(xml);

                return element;
            }
            catch { }

            return null;
        }

        /// <summary>
        /// 关闭操作
        /// </summary>
        public override void Close()
        {
            if (this.HasChildren)
            {
                foreach (RibbonElement childElement in this.ChildElements)
                {
                    childElement.Close();
                }
            }
            
        }
        #endregion
    }
}

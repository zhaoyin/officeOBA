using System.Collections.Generic;
using System.Xml.Linq;
using Cooperativeness.OBA.Word.AddIns.FrameWork;

namespace Cooperativeness.OBA.Word.Ribbon.Model
{
    /// <summary>
    /// 定义功能区页签集合元素对象
    /// </summary>
    public class XRibbonTabs : CompositeRibbonElement
    {
        #region 构造方法
        public XRibbonTabs()
        {
        }

        public XRibbonTabs(params RibbonElement[] childElements)
            : base(childElements)
        {
        }

        public XRibbonTabs(IEnumerable<RibbonElement> childElements)
            : base(childElements)
        {
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
                string xml = "<tabs></tabs>";
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
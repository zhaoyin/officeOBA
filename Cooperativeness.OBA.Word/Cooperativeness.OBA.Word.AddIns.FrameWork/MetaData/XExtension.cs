using System.Collections.Generic;
using System.Xml.Linq;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.MetaData
{
    /// <summary>
    /// 定义清单插件的扩展元素
    /// </summary>
    internal class XExtension: MetaCompositeElement
    {
        #region 字段
        // 属性名
        private string[] _attributeNames = { "Point"};

        #endregion

        #region 构造方法
        public XExtension()
        {
        }

        public XExtension(params MetaElement[] childElements)
            : base(childElements)
        {
        }

        public XExtension(IEnumerable<MetaElement> childElements)
            : base(childElements)
        {
        }

        #endregion

        #region 属性
        /// <summary>
        /// 获取或设置Bundle扩展的扩展点
        /// </summary>
        public StringValue Point
        {
            get
            {
                return this.Attributes[0] as StringValue;
            }
            internal set
            {
                this.Attributes[0] = value;
            }
        }

        /// <summary>
        /// 获取属性名集合
        /// </summary>
        internal override string[] AttributeNames
        {
            get
            {
                return this._attributeNames;
            }
        }
        #endregion

        #region 方法
        public override MetaElement CloneElement(bool deep)
        {
            return base.CloneImp<XExtension>(deep);
        }

        /// <summary>
        /// 根据XML对象设置当前元素的属性值
        /// </summary>
        /// <param name="element"></param>
        protected override void SetProperties(XElement element)
        {
            // 执行基类设置属性方法
            base.SetProperties(element);
            // 设置当前属性值
            this.Point = element.AttibuteStringValue("Point");
        }
        #endregion
    }
}

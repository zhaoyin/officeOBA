using System.Collections.Generic;
using System.Xml.Linq;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.MetaData
{
    /// <summary>
    /// 定义清单插件的扩展点元素
    /// </summary>
    internal class XExtensionPoint : MetaCompositeElement
    {
        #region 字段
        private string[] _attributeNames = { "Point", "Schema" };

        #endregion

        #region 构造方法
        public XExtensionPoint()
        {
        }

        public XExtensionPoint(params MetaElement[] childElements)
            : base(childElements)
        {
        }

        public XExtensionPoint(IEnumerable<MetaElement> childElements)
            : base(childElements)
        {
        }

        #endregion

        #region 属性
        /// <summary>
        /// 获取或设置Bundle的扩展点
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
        /// 获取或设置扩展点内容的Schema 
        /// </summary>
        public StringValue Schema
        {
            get
            {
                return this.Attributes[1] as StringValue;
            }
            internal set
            {
                this.Attributes[1] = value;
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
            return base.CloneImp<XExtensionPoint>(deep);
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
            this.Schema = element.AttibuteStringValue("Schema");
        }
        #endregion
    }
}

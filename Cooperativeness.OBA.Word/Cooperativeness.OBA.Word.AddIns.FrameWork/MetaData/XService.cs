using System.Xml.Linq;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.MetaData
{
    /// <summary>
    /// 定义清单插件的服务元素
    /// </summary>
    internal class XService : MetaLeafElement
    {
        #region 字段
        private string[] _attributeNames = { "Type", "Service","Properties" };

        #endregion

        #region 属性
        /// <summary>
        /// 获取或设置Bundle的扩展点
        /// </summary>
        public StringValue Type
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
        public StringValue Service
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
        /// 获取或设置扩展点内容的Schema 
        /// </summary>
        public StringValue Properties
        {
            get
            {
                return this.Attributes[2] as StringValue;
            }
            internal set
            {
                this.Attributes[2] = value;
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
            return base.CloneImp<XService>(deep);
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
            this.Type = element.AttibuteStringValue("Type");
            this.Service = element.AttibuteStringValue("Service");
            this.Properties = element.AttibuteStringValue("Properties");
        }
        #endregion
    }
}

using System.Xml.Linq;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.MetaData
{
    /// <summary>
    /// 定义插件清单标识的激活器元素
    /// </summary>
    internal class XActivator : MetaLeafElement
    {
        #region 字段
        private string[] _attributeNames = { "Type", "Policy" };

        #endregion

        #region 属性
        /// <summary>
        /// 获取或设置插件激活器类型全名称
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
        /// 获取或设置插件激活器激活策略
        /// </summary>
        public EnumValue<ActivatorPolicy> Policy
        {
            get
            {
                return this.Attributes[1] as EnumValue<ActivatorPolicy>;
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
        /// <summary>
        /// 拷贝当前元素
        /// </summary>
        /// <param name="deep"></param>
        /// <returns></returns>
        public override MetaElement CloneElement(bool deep)
        {
            return base.CloneImp<XActivator>(deep);
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
            this.Policy = element.AttributeEnumValue<ActivatorPolicy>("Policy");
        }
        #endregion
    }
}

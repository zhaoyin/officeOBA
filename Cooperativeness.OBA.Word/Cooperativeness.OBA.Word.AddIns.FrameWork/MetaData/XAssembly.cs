using System.Xml.Linq;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.MetaData
{
    /// <summary>
    /// 定义插件清单运行时依赖组件元素
    /// </summary>
    internal class XAssembly : MetaLeafElement
    {
        #region 字段
        // 属性名
        private string[] _attributeNames = { "Path", "Share" };

        #endregion

        #region 属性
        /// <summary>
        /// 获取或设置依赖Assembly的路径，可以使相对路径
        /// 可以是绝对路径，必选熟悉
        /// </summary>
        public StringValue Path
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
        /// 获取或设置Bundle依赖的程序集是否可以被其它插件共享 
        /// </summary>
        public BoolValue Share
        {
            get
            {
                return this.Attributes[1] as BoolValue;
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
            return base.CloneImp<XAssembly>(deep);
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
            this.Path = element.AttibuteStringValue("Path");
            this.Share = element.AttributeBoolValue("Share");
        }
        #endregion
    }
}
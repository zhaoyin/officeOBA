using System.Xml.Linq;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.MetaData
{
    /// <summary>
    /// 定义插件清单运行时依赖其他插件及其内部组件元素
    /// </summary>
    internal class XDependency : MetaLeafElement
    {
        #region 字段
        // 属性名
        private string[] _attributeNames = 
                                { 
                                    "BundleSymbolicName", "BundleVersion"
                                    , "AssemblyName","AssemblyVersion"
                                    ,"Culture","PublicKeyToken"
                                    ,"Resolution"
                                };
        #endregion

        #region 属性
        /// <summary>
        /// 获取或设置依赖插件的唯一标示名
        /// </summary>
        public StringValue BundleSymbolicName
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
        /// 获取或设置依赖插件的版本信息
        /// </summary>
        public StringValue BundleVersion
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
        /// 获取或设置依赖插件的程序集名
        /// </summary>
        public StringValue AssemblyName
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
        /// 获取或设置依赖插件的程序集版本信息
        /// </summary>
        public StringValue AssemblyVersion
        {
            get
            {
                return this.Attributes[3] as StringValue;
            }
            internal set
            {
                this.Attributes[3] = value;
            }
        }

        /// <summary>
        /// 获取或设置依赖插件的程序集本地语言
        /// </summary>
        public StringValue Culture
        {
            get
            {
                return this.Attributes[4] as StringValue;
            }
            internal set
            {
                this.Attributes[4] = value;
            }
        }

        /// <summary>
        /// 获取或设置依赖插件的程序集公钥令牌
        /// </summary>
        public StringValue PublicKeyToken
        {
            get
            {
                return this.Attributes[5] as StringValue;
            }
            internal set
            {
                this.Attributes[5] = value;
            }
        }

        /// <summary>
        /// 获取或设置依赖插件的解析方式
        /// </summary>
        public EnumValue<ResolutionMode> Resolution
        {
            get
            {
                return this.Attributes[6] as EnumValue<ResolutionMode>;
            }
            internal set
            {
                this.Attributes[6] = value;
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
            return base.CloneImp<XDependency>(deep);
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
            this.BundleSymbolicName = element.AttibuteStringValue("BundleSymbolicName");
            this.BundleVersion = element.AttibuteStringValue("BundleVersion");
            this.AssemblyName = element.AttibuteStringValue("AssemblyName");
            this.AssemblyVersion = element.AttibuteStringValue("AssemblyVersion");
            this.Culture = element.AttibuteStringValue("Culture");
            this.PublicKeyToken = element.AttibuteStringValue("PublicKeyToken");
            this.Resolution = element.AttributeEnumValue<ResolutionMode>("Resolution", ResolutionMode.Mandatory);
        }
        #endregion
    }
}

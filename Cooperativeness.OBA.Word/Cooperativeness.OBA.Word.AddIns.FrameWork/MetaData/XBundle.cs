using System.Collections.Generic;
using System.Xml.Linq;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.MetaData
{
    /// <summary>
    /// 定义插件清单元素
    /// </summary>
    internal sealed class XBundle : MetaCompositeElement
    {
        #region 字段
        // 属性名
        private string[] _attributeNames = 
            { 
                "xmlns", "SymbolicName", "Version"
                ,"Name", "StartLevel"
                ,"HostBundleSymbolicName", "HostBundleVersion"
            };

        #endregion

        #region 构造方法
        public XBundle()
        {
        }

        public XBundle(params MetaElement[] childElements)
            : base(childElements)
        {
        }

        public XBundle(IEnumerable<MetaElement> childElements)
            : base(childElements)
        {
        }

        #endregion

        #region 属性
        /// <summary>
        /// 获取或设置Bundle元素的标示名称
        /// </summary>
        public StringValue XmlNamespace
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
        /// 获取或设置Bundle元素的标示名称
        /// </summary>
        public StringValue SymbolicName
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
        /// 获取或设置Bundle的版本信息
        /// </summary>
        public StringValue Version
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
        /// 获取或设置Bundle的名称，可选
        /// </summary>
        public StringValue Name
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
        /// 获取或设置Bundle的启动级别
        /// </summary>
        public IntValue StartLevel
        {
            get
            {
                return this.Attributes[4] as IntValue;
            }
            internal set
            {
                this.Attributes[4] = value;
            }
        }

        /// <summary>
        /// 获取或设置Bundle的宿主标示名称
        /// </summary>
        public StringValue HostBundleSymbolicName
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
        /// 获取或设置Bundle的宿主的版本信息
        /// </summary>
        public StringValue HostBundleVersion
        {
            get
            {
                return this.Attributes[6] as StringValue;
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

        /// <summary>
        /// 获取或设置Bundle的激活器
        /// </summary>
        public XActivator Activator
        {
            get
            {
                return base.GetElement<XActivator>();
            }
            internal set
            {
                base.SetElement<XActivator>(value);
            }
        }

        /// <summary>
        /// 获取插件运行时相关的元数据
        /// </summary>
        public XRuntime Runtime
        {
            get
            {
                return base.GetElement<XRuntime>();
            }
            internal set
            {
                base.SetElement<XRuntime>(value);
            }
        }

        /// <summary>
        /// 获取插件的所有扩展点
        /// </summary>
        public XExtensionPoint[] ExtensionPoints
        {
            get
            {
                return base.GetElements<XExtensionPoint>();
            }
        }

        /// <summary>
        /// 获取插件的所有扩展
        /// </summary>
        public XExtension[] Extensions
        {
            get
            {
                return base.GetElements<XExtension>();
            }
        }

        /// <summary>
        /// 获取插件注册的服务集合元数据
        /// </summary>
        public XServices Services
        {
            get
            {
                return base.GetElement<XServices>();
            }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 重载拷贝方法
        /// </summary>
        /// <param name="deep"></param>
        /// <returns></returns>
        public override MetaElement CloneElement(bool deep)
        {
            return base.CloneImp<XBundle>(deep);
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
            this.XmlNamespace = element.AttibuteStringValue("xmlns");
            this.SymbolicName = element.AttibuteStringValue("SymbolicName");
            this.Version = element.AttibuteStringValue("Version");
            this.Name = element.AttibuteStringValue("Name");
            this.StartLevel = element.AttributeIntValue("StartLevel", 256);
            this.HostBundleSymbolicName = element.AttibuteStringValue("HostBundleSymbolicName");
            this.HostBundleVersion = element.AttibuteStringValue("HostBundleVersion");
        }
        #endregion
    }
}

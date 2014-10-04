using System.Collections.Generic;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.MetaData
{
    /// <summary>
    /// 定义插件清单运行时元素
    /// </summary>
    internal class XRuntime : MetaCompositeElement
    {
        #region 构造方法
        public XRuntime()
        {
        }

        public XRuntime(params MetaElement[] childElements)
            : base(childElements)
        {
        }

        public XRuntime(IEnumerable<MetaElement> childElements)
            : base(childElements)
        {
        }

        #endregion

        #region 属性
        /// <summary>
        /// 获取或设置运行时依赖的其他插件或插件的程序集
        /// </summary>
        public XDependency[] Dependencies
        {
            get
            {
                return base.GetElements<XDependency>();
            }
        }

        /// <summary>
        /// 获取或设置运行时依赖的程序集
        /// </summary>
        public XAssembly[] Assemblies
        {
            get
            {
                return base.GetElements<XAssembly>();
            }
        }
        #endregion

        #region 方法
        public override MetaElement CloneElement(bool deep)
        {
            return base.CloneImp<XRuntime>(deep);
        }

        #endregion
    }
}

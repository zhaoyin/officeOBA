using System.Collections.Generic;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.MetaData
{
    /// <summary>
    /// 定义插件清单的服务集合元素
    /// </summary>
    internal sealed class XServices : MetaCompositeElement
    {
        #region 构造方法
        public XServices()
        {
        }

        public XServices(params MetaElement[] childElements)
            : base(childElements)
        {
        }

        public XServices(IEnumerable<MetaElement> childElements)
            : base(childElements)
        {
        }

        #endregion

        #region 属性
        /// <summary>
        /// 获取所有的待注册的服务
        /// </summary>
        public XService[] Services
        {
            get
            {
                return base.GetElements<XService>();
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
            return base.CloneImp<XServices>(deep);
        }
        #endregion
    }
}

using System;
using Cooperativeness.OBA.Word.AddIns.FrameWork;
using Cooperativeness.OBA.Word.Ribbon.Model;
using System.Xml.Linq;
using Cooperativeness.OBA.Word.Ribbon.Resolver;

namespace Cooperativeness.OBA.Word.Ribbon
{
    /// <summary>
    /// 定义功能区管理器
    /// </summary>
    public class RibbonAdminImpl : IRibbonAdmin
    {
        #region 字段
        private RibbonUi ribbonExtensiblity;
        internal XRibbon xRibbon;
        private IBundleContext bundleContext;
        private IExtensionPoint point;
        internal RibbonRepository ribbonRepository;

        #endregion

        #region 构造函数
        public RibbonAdminImpl()
        {
            ribbonRepository = new RibbonRepository();
        }

        #endregion

        #region 方法
        /// <summary>
        /// 创建功能区
        /// </summary>
        /// <returns></returns>
        private XRibbon CreateRibbon()
        {
            if (xRibbon != null) return xRibbon;
            // 创建功能区对象
            xRibbon = new XRibbon();
            // 创建功能区页签集合对象
            var xTabs = new XRibbonTabs();
            xRibbon.Append(xTabs);

            return xRibbon;
        }

        /// <summary>
        /// 检查有效性
        /// </summary>
        /// <returns></returns>
        private void CheckValid()
        {
            if (xRibbon == null)
                throw new InvalidOperationException("Not initialize.");
        }

        /// <summary>
        /// 根据用户指定的类型和唯一标示获取功能区元素对象
        /// </summary>
        /// <param name="tabId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        internal T Find<T>(string id) where T : RibbonElement
        {
            return this.ribbonRepository.Find<T>(id);
        }


        /// <summary>
        /// 缓存功能区元素对象
        /// </summary>
        /// <param name="id"></param>
        /// <param name="element"></param>
        internal void CacheRibbonElement(string id, RibbonElement element)
        {
            this.ribbonRepository.Add(id, element);
        }

        /// <summary>
        /// 检查指定的分组标示是否在指定的页签中
        /// </summary>
        /// <param name="tabId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        internal bool GroupInTab(string tabId, string id)
        {
            return this.ribbonRepository.GroupInTab(tabId, id);
        }

        /// <summary>
        /// 检查指定的分组标示是否在指定的页签中
        /// </summary>
        /// <param name="tabId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        internal bool MenuInGroup(string groupId, string id)
        {
            return this.ribbonRepository.MenuInGroup(groupId, id);
        }

        /// <summary>
        /// 检查是否存在指定的类型
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal bool Contains<T>(string id) where T : RibbonElement
        {
            return this.ribbonRepository.Contains<T>(id);
        }

        /// <summary>
        /// 获取功能区扩展对象
        /// </summary>
        public object RibbonUi
        {
            get 
            {
                CheckValid();
                if (ribbonExtensiblity == null)
                {
                    ribbonExtensiblity=new RibbonUi(this);
                }
                return ribbonExtensiblity;
            }
        }

        /// <summary>
        /// 初始化功能区管理器
        /// </summary>
        /// <param name="context"></param>
        /// <param name="extensinPoint"></param>
        public void Initialize(IBundleContext context, IExtensionPoint extensinPoint)
        {
            this.point = extensinPoint;
            this.bundleContext = context;
            this.xRibbon = CreateRibbon();
        }

        /// <summary>
        /// 添加扩展对象
        /// </summary>
        /// <param name="extension"></param>
        public void AddExtension(IExtension extension)
        {
            CheckValid();
            if (extension.Data != null)
            {
                foreach (XElement xElement in extension.Data)
                {
                    IRibbonResolver ribbonResolver = BaseRibbonResolver.CreateResolver("ribbon", this);
                    ribbonResolver.Resolve(xElement, extension.Owner);
                    break;
                }
            }
        }

        /// <summary>
        /// 刷新整个功能区
        /// </summary>
        public void Invalidate()
        {
            if (this.xRibbon != null)
            {
                this.xRibbon.Invalidate();
            }
        }

        /// <summary>
        /// 根据功能区按钮标识，刷新指定功能区按钮
        /// </summary>
        /// <param name="ControlID"></param>
        public void InvalidateControl(string ControlID)
        {
            if (this.xRibbon != null)
            {
                this.xRibbon.InvalidateControl(ControlID);
            }
        }

        /// <summary>
        /// 关闭功能区管理器
        /// </summary>
        public void Close()
        {
            this.xRibbon.Close();
        }
        #endregion
    }
}

using System;
using Cooperativeness.OBA.Word.AddIns.FrameWork;
using Cooperativeness.OBA.Word.Ribbon.Model;
using System.Xml.Linq;

namespace Cooperativeness.OBA.Word.Ribbon.Resolver
{
    /// <summary>
    /// 定义功能区按钮解析器
    /// </summary>
    internal class RibbonSingleResolver<T> : BaseRibbonResolver where T : LeafRibbonElement
    {
        public RibbonSingleResolver(RibbonAdminImpl ribbonAdmin)
            : base(ribbonAdmin)
        {
        }

        /// <summary>
        /// 解析操作
        /// </summary>
        /// <param name="element"></param>
        /// <param name="bundle"></param>
        /// <returns></returns>
        protected override void OnResolver(XElement element, IBundle bundle)
        {
            // 首先获取当前页签的唯一标示
            string id = element.AttibuteStringValue("id");
            if (string.IsNullOrEmpty(id)) return;
            // 获取上级容器的唯一标示
            string containerId = element.Parent.AttibuteStringValue("id");
            // 检查缓存中是否存在
            if (ribbonAdmin.Contains<T>(id)) return;
            // 创建指定类型的对象
            T xRibbonElement = Activator.CreateInstance<T>();
            bool success = xRibbonElement.InitProperties(element, bundle);
            if (!success) return;
            // 将当前元素的容器
            CompositeRibbonElement xRibbonContainer = ribbonAdmin.Find<CompositeRibbonElement>(containerId);
            xRibbonContainer.Append(xRibbonElement);
            // 添加到缓存
            ribbonAdmin.CacheRibbonElement(id, xRibbonElement);
        }
    }
}

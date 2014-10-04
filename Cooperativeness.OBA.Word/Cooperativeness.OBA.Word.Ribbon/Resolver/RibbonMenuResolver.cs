using System.Collections.Generic;
using System.Linq;
using Cooperativeness.OBA.Word.AddIns.FrameWork;
using Cooperativeness.OBA.Word.Ribbon.Model;
using System.Xml.Linq;

namespace Cooperativeness.OBA.Word.Ribbon.Resolver
{
    /// <summary>
    /// 定义功能区菜单解析器
    /// </summary>
    internal class RibbonMenuResolver : BaseRibbonResolver
    {
        private string[] supportNestedElements = new string[]
                        {
                            "button","toggleButton","menu"
                        };

        public RibbonMenuResolver(RibbonAdminImpl ribbonAdmin)
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
            // 获取上级页签的唯一标示
            string ContainerId = element.Parent.AttibuteStringValue("id");
            // 尝试从缓存中加载
            XRibbonMenu xRibbonMenu = ribbonAdmin.Find<XRibbonMenu>(id);
            // 将当前菜单的容器对象
            CompositeRibbonElement xRibbonContainer = ribbonAdmin.Find<CompositeRibbonElement>(ContainerId);
            if (xRibbonMenu != null)
            {
                if (xRibbonContainer != xRibbonMenu.Parent) 
                    return;
            }
            else
            {
                // 创建分组对象
                xRibbonMenu = new XRibbonMenu();
                bool success = xRibbonMenu.InitProperties(element, bundle);
                if (!success) return;
                // 将当前菜单添加到容器中
                xRibbonContainer.Append(xRibbonMenu);
                // 添加到缓存
                ribbonAdmin.CacheRibbonElement(id, xRibbonMenu);
            }
            // 获取所有的元素列表
            IEnumerable<XElement> xChildElements = element.Elements();
            if (xChildElements == null && xChildElements.Count() == 0) return;
            // 遍历所有元素
            foreach (XElement xChildElement in xChildElements)
            {
                if (SupportNestedType(xChildElement.Name.LocalName))
                {
                    IRibbonResolver resolver = CreateResolver(xChildElement.Name.LocalName, ribbonAdmin);
                    if (resolver != null) resolver.Resolve(xChildElement, bundle);
                }
            }
        }

        /// <summary>
        /// 支持嵌套的类型
        /// </summary>
        /// <returns></returns>
        private bool SupportNestedType(string name)
        {
            if (supportNestedElements.Contains(name))
                return true;
            return false;
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Cooperativeness.OBA.Word.AddIns.FrameWork;
using Cooperativeness.OBA.Word.Ribbon.Model;

namespace Cooperativeness.OBA.Word.Ribbon.Resolver
{
    /// <summary>
    /// 定义功能区分组解析器对象
    /// </summary>
    internal class RibbonGroupResolver : BaseRibbonResolver
    {
        private string[] supportNestedElements = new string[]
                        {
                            "ribbon","tabs","tab"
                        };

        public RibbonGroupResolver(RibbonAdminImpl ribbonAdmin)
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
            string tabId = element.Parent.AttibuteStringValue("id");
            // 尝试从缓存中加载
            XRibbonGroup xRibbonGroup = ribbonAdmin.Find<XRibbonGroup>(id);
            if (xRibbonGroup != null)
            {
                if (!ribbonAdmin.GroupInTab(tabId, id)) return;
            }
            else
            {
                // 创建分组对象
                xRibbonGroup = new XRibbonGroup();
                bool success = xRibbonGroup.InitProperties(element, bundle);
                if (!success) return;
                // 将当前分组添加到页签中
                XRibbonTab xRibbonTab = ribbonAdmin.Find<XRibbonTab>(tabId);
                xRibbonTab.Append(xRibbonGroup);
                // 添加到缓存
                ribbonAdmin.CacheRibbonElement(id, xRibbonGroup);
            }
            // 获取所有的元素列表
            IEnumerable<XElement> xChildElements = element.Elements();
            if (xChildElements == null && xChildElements.Count() == 0) return;
            // 遍历所有元素
            foreach (XElement xChildElement in xChildElements)
            {
                if (UnSupportNestedType(xChildElement.Name.LocalName)) continue;
                IRibbonResolver resolver = CreateResolver(xChildElement.Name.LocalName, ribbonAdmin);
                if (resolver != null) resolver.Resolve(xChildElement, bundle);
            }
        }

        /// <summary>
        /// 不支持支持嵌套的类型
        /// </summary>
        /// <returns></returns>
        private bool UnSupportNestedType(string name)
        {
            if (supportNestedElements.Contains(name))
                return true;
            return false;
        }
    }
}

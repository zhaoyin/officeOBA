using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Cooperativeness.OBA.Word.AddIns.FrameWork;

namespace Cooperativeness.OBA.Word.Ribbon.Resolver
{
    internal class RibbonResolver : BaseRibbonResolver
    {
        public RibbonResolver(RibbonAdminImpl ribbonAdmin)
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
            // 检查是否符合规范，并起始于ribbon节点
            if (!element.Name.LocalName.Equals("ribbon"))
                return;
            // 检查ribbon子节点，有且仅有一个tabs节点
            IEnumerable<XElement> xTabsElements = element.Elements(xns + "tabs");
            if (xTabsElements == null && xTabsElements.Count() != 1)
                return;
            XElement xTabsElement = xTabsElements.FirstOrDefault();
            // 获取所有的tab节点
            IEnumerable<XElement> xTabs = xTabsElement.Elements(xns + "tab");
            if (xTabs == null || xTabs.Count() == 0)
                return;
            // 遍历所有的tab节点
            foreach (XElement xTab in xTabs)
            {
                // 根据元素类型解析器
                IRibbonResolver tabResolver = CreateResolver(xTab.Name.LocalName, ribbonAdmin);
                // 解析Tab元素
                if (tabResolver != null) tabResolver.Resolve(xTab, bundle);
            }
        }
    }
}

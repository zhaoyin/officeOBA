using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Cooperativeness.OBA.Word.AddIns.FrameWork;
using Cooperativeness.OBA.Word.Ribbon.Model;

namespace Cooperativeness.OBA.Word.Ribbon.Resolver
{
    internal class RibbonTabResolver : BaseRibbonResolver
    {
        public RibbonTabResolver(RibbonAdminImpl ribbonAdmin)
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
            // 尝试从缓存中加载
            XRibbonTab xRibbonTab = ribbonAdmin.Find<XRibbonTab>(id);
            if (xRibbonTab == null)
            {
                // 创建页签对象
                xRibbonTab = new XRibbonTab();
                bool success = xRibbonTab.InitProperties(element, bundle);
                if (!success) return;
                // 将当前页签添加到页签集合中
                ribbonAdmin.xRibbon.Tabs.Append(xRibbonTab);
                // 添加到缓存
                ribbonAdmin.CacheRibbonElement(id, xRibbonTab);
            }
            // 获取所有的分组元素列表
            IEnumerable<XElement> xGroups = element.Elements(xns + "group");
            if (xGroups == null && xGroups.Count() == 0) return;
            // 遍历所有分组
            foreach (XElement xGroup in xGroups)
            {
                IRibbonResolver groupResolver = CreateResolver(xGroup.Name.LocalName, ribbonAdmin);
                if (groupResolver != null) groupResolver.Resolve(xGroup, bundle);
            }
        }
    }
}

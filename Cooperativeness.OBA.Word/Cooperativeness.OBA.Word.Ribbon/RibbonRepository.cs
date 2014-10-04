using System.Collections.Specialized;
using Cooperativeness.OBA.Word.Ribbon.Model;
using Cooperativeness.OBA.Word.Tools;

namespace Cooperativeness.OBA.Word.Ribbon
{
    /// <summary>
    /// 定义功能区仓库
    /// </summary>
    public class RibbonRepository
    {
        private static readonly Logger Log=new Logger(typeof(RibbonRepository));
        #region 字段
        private OrderedDictionary ribbonsById;
        private OrderedDictionary groupsByTab;
        private OrderedDictionary menusByGroup;
        #endregion

        #region 构造函数
        public RibbonRepository()
        {
            ribbonsById = new OrderedDictionary();
            groupsByTab = new OrderedDictionary();
            menusByGroup = new OrderedDictionary();
        }

        #endregion

        #region 方法
        /// <summary>
        /// 添加功能区元素
        /// </summary>
        /// <param name="id"></param>
        /// <param name="element"></param>
        public void Add(string id, RibbonElement element)
        {
            if (string.IsNullOrEmpty(id)) return;
            if (!ribbonsById.Contains(id))
            {
                // 添加到全局的功能区缓存中
                ribbonsById.Add(id, element);
                // 检查是否是分组元素
                var xRibbonGroup = element as XRibbonGroup;
                if (xRibbonGroup != null)
                {
                    var xRibbonTab = xRibbonGroup.Parent as XRibbonTab;
                    OrderedDictionary groups ;
                    if (xRibbonTab == null)
                    {
                        Log.Debug("Add方法xRibbonTab为空！");
                        return;
                    }

                    if (groupsByTab.Contains(xRibbonTab.Id))
                    {
                        groups = groupsByTab[xRibbonTab.Id] as OrderedDictionary;
                    }
                    else
                    {
                        groups = new OrderedDictionary();
                        groupsByTab.Add(xRibbonTab.Id, groups);
                    }
                    if (groups == null)
                    {
                        Log.Debug("Add方法Groups为空！");
                        return;
                    }
                    groups.Add(id, element);
                    return;
                }
                // 检查是否是菜单元素
                var xRibbonMenu = element as XRibbonMenu;
                if (xRibbonMenu != null)
                {
                    xRibbonGroup = xRibbonMenu.Parent as XRibbonGroup;
                    OrderedDictionary menus;
                    if (xRibbonGroup == null)
                    {
                        Log.Debug("xRibbonGroup为空！");
                        return;
                    }
                    if (menusByGroup.Contains(xRibbonGroup.Id))
                    {
                        menus = menusByGroup[xRibbonGroup.Id] as OrderedDictionary;
                    }
                    else
                    {
                        menus = new OrderedDictionary();
                        menusByGroup.Add(xRibbonGroup.Id, menus);
                    }
                    if (menus == null)
                    {
                        Log.Debug("menus为空！");
                        return;
                    }
                    menus.Add(id, element);
                }
            }
        }

        /// <summary>
        /// 根据用户指定的类型和唯一标示获取功能区元素对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T Find<T>(string id) where T:RibbonElement
        {
            if (!string.IsNullOrEmpty(id) && ribbonsById.Contains(id))
            {
                return ribbonsById[id] as T;
            }

            return default(T);
        }

        /// <summary>
        /// 检查指定的分组标示是否在指定的页签中
        /// </summary>
        /// <param name="tabId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool GroupInTab(string tabId, string id)
        {
            if (groupsByTab.Contains(tabId))
            {
                var groups = groupsByTab[tabId] as OrderedDictionary;
                if (groups == null)
                {
                    Log.Debug("GroupInTab方法groups为空！");
                    return false;
                }
                if (groups.Contains(id)) return true;
            }
            return false;
        }

        /// <summary>
        /// 检查指定的分组标示是否在指定的页签中
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool MenuInGroup(string groupId, string id)
        {
            if (menusByGroup.Contains(groupId))
            {
                var menus = menusByGroup[groupId] as OrderedDictionary;
                if (menus == null)
                {
                    Log.Debug("GroupInTab方法groups为空！");
                    return false;
                } 
                if (menus.Contains(id)) return true;
            }
            return false;
        }

        /// <summary>
        /// 检查是否存在指定的类型
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Contains<T>(string id) where T:RibbonElement
        {
            if (string.IsNullOrEmpty(id))
                return false;
            if (ribbonsById.Contains(id))
            {
                var local = ribbonsById[id] as T;
                if (local != null) return true;
            }

            return false;
        }

        #endregion
    }
}

using System.Xml.Linq;
using Cooperativeness.OBA.Word.AddIns.FrameWork;
using Cooperativeness.OBA.Word.Ribbon.Model;

namespace Cooperativeness.OBA.Word.Ribbon.Resolver
{
    internal abstract class BaseRibbonResolver : IRibbonResolver
    {
        #region 字段
        protected RibbonAdminImpl ribbonAdmin;
        protected XNamespace xns = "urn:xplugin-bundle-manifest-2.0";

        #endregion

        #region 构造函数
        public BaseRibbonResolver(RibbonAdminImpl ribbonAdmin)
        {
            this.ribbonAdmin = ribbonAdmin;
        }

        #endregion

        #region 方法
        /// <summary>
        /// 创建功能区解析器
        /// </summary>
        /// <param name="elementName"></param>
        /// <param name="ribbonAdmin"></param>
        /// <returns></returns>
        public static IRibbonResolver CreateResolver(string elementName, RibbonAdminImpl ribbonAdmin)
        {
            switch (elementName)
            {
                case "ribbon":
                    return new RibbonResolver(ribbonAdmin);
                case "tab":
                    return new RibbonTabResolver(ribbonAdmin);
                case "group":
                    return new RibbonGroupResolver(ribbonAdmin);
                case "button":
                    return new RibbonSingleResolver<XRibbonButton>(ribbonAdmin);
                case "toggleButton":
                    return new RibbonSingleResolver<XRibbonToggleButton>(ribbonAdmin);
                case "separator":
                    return new RibbonSingleResolver<XRibbonSeparator>(ribbonAdmin);
                case "menu":
                    return new RibbonMenuResolver(ribbonAdmin);
                default :
                    return null;
            }

        }

        #endregion

        #region IRibbonResolver 成员
        /// <summary>
        /// 解析操作
        /// </summary>
        /// <param name="element"></param>
        /// <param name="bundle"></param>
        /// <returns></returns>
        public void Resolve(XElement element, IBundle bundle)
        {
            this.OnResolver(element, bundle);
        }

        protected abstract void OnResolver(XElement element, IBundle bundle);

        #endregion
    }
}

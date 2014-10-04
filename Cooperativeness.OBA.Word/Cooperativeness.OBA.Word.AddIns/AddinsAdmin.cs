using System;
using Cooperativeness.OBA.Word.AddIns.FrameWork;
using Cooperativeness.OBA.Word.Ribbon;
using Office = Microsoft.Office.Core;

namespace Cooperativeness.OBA.Word.AddIns
{
    /// <summary>
    /// 定义插件管理
    /// </summary>
    public class AddinsAdmin
    {
        #region 字段
        //private static AddinsAdmin instance;
        private BundleStarter bundleStarter;
        private IRibbonAdmin ribbonAdmin;

        #endregion

        #region 构造函数
        public AddinsAdmin() { }

        #endregion

        #region 属性
        ///// <summary>
        ///// 定义插件管理当前实例
        ///// </summary>
        //public static AddinsAdmin Instance
        //{
        //    get
        //    {
        //        if (instance == null)
        //            instance = new AddinsAdmin();
        //        return instance;
        //    }
        //}

        /// <summary>
        /// 获取功能区对象
        /// </summary>
        public Office.IRibbonExtensibility RibbonUi
        {
            get
            {
                CheckValid();
                return InternalGetRibbon();
            }
        }

        /// <summary>
        /// 获取插件启动器
        /// </summary>
        public BundleStarter BundleStarter
        {
            get { return this.bundleStarter; }
        }

        #endregion

        #region 方法
        /// <summary>
        /// 启动插件管理器
        /// </summary>
        public void Start()
        {
            if (bundleStarter == null)
            {
                bundleStarter = new BundleStarter();
                bundleStarter.Start();
            }
        }

        /// <summary>
        /// 关闭插件管理器
        /// </summary>
        /// <returns></returns>
        public void Shutdown()
        {
            if (bundleStarter != null)
            {
                bundleStarter.Shutdown();
            }
        }

        /// <summary>
        /// 检查有效性
        /// </summary>
        private void CheckValid()
        {
            if (bundleStarter == null)
                throw new InvalidOperationException("AddIns not initialize.");
        }

        /// <summary>
        /// 获取功能区对象
        /// </summary>
        /// <returns></returns>
        private Office.IRibbonExtensibility InternalGetRibbon()
        {
            // 获取功能区管理服务
            if (ribbonAdmin == null)
            {
                
                ribbonAdmin = bundleStarter.GetService<IRibbonAdmin>();
            }
            // 获取功能区扩展对象
            if (ribbonAdmin != null)
            {
                var ribbon = ribbonAdmin.RibbonUi as Office.IRibbonExtensibility;
                return ribbon;
            }
            return null;
        }
        #endregion
    }
}

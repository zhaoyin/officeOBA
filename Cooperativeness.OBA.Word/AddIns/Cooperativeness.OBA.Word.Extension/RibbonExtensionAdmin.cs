
using Cooperativeness.OBA.Word.AddIns.FrameWork;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Events;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Service;
using Cooperativeness.OBA.Word.Ribbon;

namespace Cooperativeness.OBA.Word.Extension
{
    /// <summary>
    /// 定义功能区扩展管理器
    /// </summary>
    internal class RibbonExtensionAdmin
    {
        #region 字段
        private static RibbonExtensionAdmin instance;
        private IBundleContext context;
        private IRibbonAdmin ribbonAdmin;
        private IServiceRegistration ribbonAdminService;
        private IServiceRegistration ribbonExtensibilityService;

        #endregion

        #region 构造函数
        private RibbonExtensionAdmin()
        {
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取功能区扩展管理器的单实例对象
        /// </summary>
        public static RibbonExtensionAdmin Instance
        {
            get
            {
                if (instance == null)
                    instance = new RibbonExtensionAdmin();
                return instance;
            }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 初始化操作
        /// </summary>
        /// <param name="context"></param>
        public void Initialize(IBundleContext context)
        {
            this.context = context;
            context.ExtensionChanged += OnExtensionChanged;
            context.ExtensionPointChanged += OnExtensionPointChanged;
        }

        /// <summary>
        /// 关闭功能区扩展管理器
        /// </summary>
        public void Close()
        {
            context.ExtensionChanged -= OnExtensionChanged;
            context.ExtensionPointChanged -= OnExtensionPointChanged;
            ribbonAdminService.Unregister();
            ribbonExtensibilityService.Unregister();
        }

        /// <summary>
        /// 功能区扩展点变化处理逻辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnExtensionPointChanged(object sender, ExtensionPointEventArgs e)
        {
            if (e.Point.Equals("Word.Robin"))
            {
                if (ribbonAdmin == null)
                {
                    ribbonAdmin = new RibbonAdminImpl();
                    ribbonAdmin.Initialize(context, e.ExtensionPoint);

                    ribbonAdminService = Activator.Register(new string[] { typeof(IRibbonAdmin).FullName }, ribbonAdmin, null);
                    ribbonExtensibilityService = Activator.Register(new string[] { "Microsoft.Office.Core.IRibbonExtensibility" }, ribbonAdmin.RibbonUi, null);

                }
            }
        }

        /// <summary>
        /// 功能区扩展变化处理逻辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnExtensionChanged(object sender, ExtensionEventArgs e)
        {
            if (e.Point.Equals("Word.Robin"))
            {
                if (ribbonAdmin != null)
                {
                    ribbonAdmin.AddExtension(e.Extension);
                }
            }
        }

        
        #endregion
    }
}


using Cooperativeness.OBA.Word.AddIns.FrameWork;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Service;

namespace Cooperativeness.OBA.Word.AddIns
{
    internal class ServiceAdmin
    {
        #region 字段
        private ThisAddIn addIn;
        private IServiceRegistration wordApplicationService;
        private IServiceRegistration _customTaskPaneService;
        private bool isActive = false;
        private BundleStarter bundleStarter;

        #endregion

        #region 构造函数
        public ServiceAdmin(ThisAddIn addIn, BundleStarter bundleStarter)
        {
            this.addIn = addIn;
            this.bundleStarter = bundleStarter;
        }

        #endregion

        #region 方法
        /// <summary>
        /// 启动Word插件服务管理
        /// </summary>
        public void Start()
        {
            if (!isActive)
            {
                wordApplicationService = bundleStarter.RegisterService(
                        new string[] { "Microsoft.Office.Interop.Word.Application" }, addIn.Application, null);
                _customTaskPaneService = bundleStarter.RegisterService(
                        new string[] { "Microsoft.Office.Tools.CustomTaskPaneCollection" }, addIn.CustomTaskPanes, null);
                isActive = true;
            }
        }

        /// <summary>
        /// 停止Word插件服务管理
        /// </summary>
        public void Stop()
        {
            if (isActive)
            {
                if (wordApplicationService != null)
                    wordApplicationService.Unregister();
                if (_customTaskPaneService != null)
                    _customTaskPaneService.Unregister();
                isActive = false;
            }
        }
        #endregion
    }
}

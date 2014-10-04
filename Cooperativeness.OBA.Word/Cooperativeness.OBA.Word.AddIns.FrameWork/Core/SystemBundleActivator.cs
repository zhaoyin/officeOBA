using System;
using System.Collections.Generic;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Service;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Core
{
    /// <summary>
    /// 定义系统插件激活器
    /// </summary>
    internal class SystemBundleActivator : IBundleActivator
    {
        #region 字段
        private BundleContextImpl context;
        private SystemBundle bundle;
        private Framework framework;
        private IServiceRegistration packageAdmin;
        private IServiceRegistration startLevel;

        #endregion

        #region 方法
        /// <summary>
        /// 启动操作
        /// </summary>
        /// <param name="context"></param>
        public void Start(IBundleContext context)
        {
            this.context = context as BundleContextImpl;
            bundle = (SystemBundle)context.Bundle;
            framework = bundle.Framework;

            if (framework.packageAdmin != null)
                packageAdmin = Register(new String[] {ConfigConstant.BUNDLE_PACKAGEADMIN_NAME}, framework.packageAdmin, null);
            if (framework.startLevelManager != null)
                startLevel = Register(new String[] { ConfigConstant.BUNDLE_STARTLEVEL_NAME }, framework.startLevelManager, null);
        }

        /// <summary>
        /// 停止操作
        /// </summary>
        /// <param name="context"></param>
        public void Stop(IBundleContext context)
        {
            if (packageAdmin != null)
                packageAdmin.Unregister();
            if (startLevel != null)
                startLevel.Unregister();

            framework = null;
            bundle = null;
            this.context = null;
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="names"></param>
        /// <param name="service"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        private IServiceRegistration Register(string[] names, object service, IDictionary<string, object> properties)
        {
            if (properties == null)
                properties = new Dictionary<string, object>();
            properties.Add(ConfigConstant.SERVICE_RANKING, int.MaxValue);
            properties.Add(ConfigConstant.SERVICE_PID, bundle.BundleId + "." + service.GetType().FullName); //$NON-NLS-1$
            return context.RegisterService(names, service, properties);
        }

        #endregion
    }
}

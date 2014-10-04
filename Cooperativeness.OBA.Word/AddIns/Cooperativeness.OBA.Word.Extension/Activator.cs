using System.Collections.Generic;
using Cooperativeness.OBA.Word.AddIns.FrameWork;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Service;

namespace Cooperativeness.OBA.Word.Extension
{
    public class Activator : IBundleActivator
    {
        #region 字段
        private static IBundleContext bundleContext;
        private static IBundle bundle;

        #endregion

        #region 属性
        /// <summary>
        /// 获取当前插件的上下文
        /// </summary>
        internal static IBundleContext Context
        {
            get { return bundleContext; }
        }

        #endregion

        #region 方法
        /// <summary>
        /// 激活器启动
        /// </summary>
        /// <param name="context"></param>
        public void Start(IBundleContext context)
        {
            // 获取上下文对象
            bundleContext = context;
            bundle = context.Bundle;
            // 初始化功能区管理器
            RibbonExtensionAdmin.Instance.Initialize(context);
        }

        /// <summary>
        /// 激活器停止
        /// </summary>
        /// <param name="context"></param>
        public void Stop(IBundleContext context)
        {
            context = null;
            RibbonExtensionAdmin.Instance.Close();
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="names"></param>
        /// <param name="service"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        internal static IServiceRegistration Register(string[] names, object service, IDictionary<string, object> properties)
        {
            if (properties == null)
                properties = new Dictionary<string, object>();
            properties.Add(ConfigConstant.SERVICE_RANKING, int.MaxValue);
            properties.Add(ConfigConstant.SERVICE_PID, bundle.BundleId + "." + service.GetType().FullName); //$NON-NLS-1$
            return bundleContext.RegisterService(names, service, properties);
        }
        #endregion
    }
}

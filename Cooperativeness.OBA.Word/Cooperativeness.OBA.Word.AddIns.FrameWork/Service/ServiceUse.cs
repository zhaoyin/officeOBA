using System;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Core;
using Cooperativeness.OBA.Word.Tools;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Service
{
    /// <summary>
    /// 定义服务使用统计管理器对象
    /// </summary>
    internal class ServiceUse
    {
        private static readonly Logger Log = new Logger(typeof(ServiceUse));
        #region 字段
        // 服务工厂
        private IServiceFactory factory;
        // 服务使用相关的插件上下文
        private BundleContextImpl context;
        // 服务注册者
        private ServiceRegistrationImpl registration;
        // 缓存的服务对象
        private object cachedService;
        // 使用计数
        private int useCount;

        #endregion

        #region 构造函数
        public ServiceUse(BundleContextImpl context, ServiceRegistrationImpl registration)
        {
            this.useCount = 0;
            Object service = registration.ServiceObject;
            if (service is IServiceFactory)
            {
                this.factory = (IServiceFactory)service;
                this.cachedService = null;
            }
            else
            {
                this.factory = null;
                this.cachedService = service;
            }
            this.context = context;
            this.registration = registration;
        }

        #endregion

        #region 方法
        /// <summary>
        /// 获取服务对象
        /// </summary>
        /// <returns></returns>
        internal object GetService()
        {
            if ((useCount > 0) || (factory == null))
            {
                useCount++;
                return cachedService;
            }

            object service;
            try
            {
                service = factory.GetService(context.GetBundleImpl(), registration);
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
                return null;
            }

            if (service == null)
            {
                return null;
            }

            //string[] clazzes = registration.getClasses();
            //String invalidService = ServiceRegistry.checkServiceClass(clazzes, service);
            //if (invalidService != null) {
            //    return null;
            //}

            this.cachedService = service;
            useCount++;

            return service;
        }

        /// <summary>
        /// 释放服务对象
        /// </summary>
        /// <returns></returns>
        internal bool UngetService()
        {
            if (useCount == 0)
            {
                return true;
            }

            useCount--;
            if (useCount > 0)
            {
                return false;
            }

            if (factory == null)
            {
                return true;
            }

            object service = cachedService;
            cachedService = null;

            try
            {
                factory.UngetService(context.GetBundleImpl(), registration, service);
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
            }

            return true;
        }

        /// <summary>
        /// 释放服务
        /// </summary>
        internal void ReleaseService()
        {
            if ((useCount == 0) || (factory == null))
            {
                return;
            }
            object service = cachedService;
            cachedService = null;
            useCount = 0;

            try
            {
                factory.UngetService(context.GetBundleImpl(), registration, service);
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
            }
        }

        #endregion
    }
}

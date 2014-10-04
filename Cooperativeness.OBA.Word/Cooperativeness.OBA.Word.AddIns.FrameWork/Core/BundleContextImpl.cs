using System;
using System.Collections;
using System.Collections.Generic;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Events;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Service;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Core
{
    /// <summary>
    /// 定义插件上下文具体实现类
    /// </summary>
    internal class BundleContextImpl : IBundleContext
    {
        #region 属性
        private Framework framework;
        private BundleHost bundle;
        private object contextLock = new object();
        private volatile bool valid = false;
        private Hashtable servicesInUse;
        protected IBundleActivator activator;

        #endregion

        #region 构造函数
        public BundleContextImpl(BundleHost bundle)
        {
            this.bundle = bundle;
            this.framework = bundle.Framework;
            this.valid = true;
            lock (contextLock)
            {
                servicesInUse = null;
            }
            activator = null;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取当前插件的框架对象
        /// </summary>
        public Framework Framework
        {
            get { return this.framework; }
        }

        /// <summary>
        /// 获取当前上下文所属的宿主插件对象
        /// </summary>
        public IBundle Bundle
        {
            get { return GetBundleImpl(); }
        }

        /// <summary>
        /// 获取所有的已安装的插件
        /// </summary>
        public IBundle[] Bundles
        {
            get { return framework.getAllBundles(); }
        }

        #endregion

        #region 方法
        /// <summary>
        /// 获取实际的插件对象
        /// </summary>
        /// <returns></returns>
        public AbstractBundle GetBundleImpl()
        {
            return this.bundle;
        }

        /// <summary>
        /// 创建过滤器对象
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IFilter createFilter(String filter)
        {
            return FilterImpl.NewInstance(filter);
        }

        /// <summary>
        /// 返回当前正在被上下文使用的服务的服务注册者到服务使用的映射
        /// </summary>
        /// <returns></returns>
        public Hashtable GetServicesInUseMap()
        {
            lock (contextLock)
            {
                return servicesInUse;
            }
        }

        /// <summary>
        /// 启动操作
        /// </summary>
        public void Start()
        {
            activator = bundle.LoadBundleActivator();

            if (activator != null)
            {
                try
                {
                    StartActivator(activator);
                }
                catch (BundleException be)
                {
                    activator = null;
                    throw be;
                }
            }
        }

        /// <summary>
        /// 启动激活器
        /// </summary>
        /// <param name="bundleActivator"></param>
        internal void StartActivator(IBundleActivator bundleActivator)
        {
            try
            {
                if (bundleActivator != null)
                {
                    bundleActivator.Start(this);
                }
            }
            catch (Exception ex)
            {
                string clazz = null;
                clazz = bundleActivator.GetType().FullName;

                throw new BundleException(string.Format("Activator Start Exception. [Clazz:{0}] - [Method:{1}] -[Bundle:{2}] ", new object[] { clazz, "start", string.IsNullOrEmpty(bundle.SymbolicName) ? "" + bundle.BundleId : bundle.SymbolicName }), ex, BundleExceptionType.ACTIVATOR_ERROR); //$NON-NLS-1$ //$NON-NLS-2$ 
            }
        }

        /// <summary>
        /// 停止激活器
        /// </summary>
        public void Stop()
        {
            try
            {
                if (activator != null)
                {
                    activator.Stop(this);
                }
            }
            catch (Exception ex)
            {
                string clazz = null;
                clazz = activator.GetType().FullName;
                throw new BundleException(string.Format("Activator Start Exception. [Clazz:{0}] - [Method:{1}] -[Bundle:{2}] ", new object[] { clazz, "Stop", string.IsNullOrEmpty(bundle.SymbolicName) ? "" + bundle.BundleId : bundle.SymbolicName }), ex, BundleExceptionType.ACTIVATOR_ERROR); //$NON-NLS-1$ //$NON-NLS-2$ 
            }
            finally
            {
                activator = null;
            }
        }

        /// <summary>
        /// 关闭上下文
        /// </summary>
        public void Close()
        {
            valid = false; /* invalidate context */
            ServiceRegistry registry = framework.ServiceRegistry;
            /* service's registered by the bundle, if any, are unregistered. */
            registry.UnregisterServices(this);
            /* service's used by the bundle, if any, are released. */
            registry.ReleaseServicesInUse(this);
            lock (contextLock)
            {
                servicesInUse = null;
            }
        }

        /// <summary>
        /// 检查上下文是否有效，如果无效会抛出非法状态异常
        /// </summary>
        public void CheckValid()
        {
            if (!IsValid())
            {
                throw new IllegalStateException("Context invalid exception. ");
            }
        }

        /// <summary>
        /// 检查上下文是否处于有效状态
        /// </summary>
        /// <returns></returns>
        protected bool IsValid()
        {
            return valid;
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="clazzes"></param>
        /// <param name="service"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public IServiceRegistration RegisterService(string[] clazzes, object service, IDictionary<string, object> properties)
        {
            CheckValid();

            return framework.ServiceRegistry.RegisterService(this, clazzes, service, properties);
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="clazz"></param>
        /// <param name="service"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public IServiceRegistration RegisterService(string clazz, object service, IDictionary<string, object> properties)
        {
            string[] clazzes = new string[] { clazz };

            return RegisterService(clazzes, service, properties);
        }

        /// <summary>
        /// 根据服务类型和过滤属性获取服务引用
        /// </summary>
        /// <param name="clazz"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IServiceReference[] GetServiceReferences(string clazz, string filter)
        {
            CheckValid();
            return framework.ServiceRegistry.GetServiceReferences(this, clazz, filter, false);
        }

        /// <summary>
        /// 根据服务类型和过滤属性获取服务引用
        /// </summary>
        /// <param name="clazz"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IServiceReference[] GetAllServiceReferences(string clazz, string filter)
        {
            CheckValid();
            return framework.ServiceRegistry.GetServiceReferences(this, clazz, filter, true);
        }

        /// <summary>
        /// 根据服务类型获取服务引用
        /// </summary>
        /// <param name="clazz"></param>
        /// <returns></returns>
        public IServiceReference GetServiceReference(string clazz)
        {
            CheckValid();

            return framework.ServiceRegistry.GetServiceReference(this, clazz);
        }

        /// <summary>
        /// 根据服务引用获取服务对象
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        public object GetService(IServiceReference reference)
        {
            CheckValid();
            if (reference == null)
                throw new ArgumentNullException("A null service reference is not allowed."); //$NON-NLS-1$
            lock (contextLock)
            {
                if (servicesInUse == null)
                    // Cannot predict how many services a bundle will use, start with a small table.
                    servicesInUse = new Hashtable();
            }

            return framework.ServiceRegistry.GetService(this, (ServiceReferenceImpl)reference);
        }

        /// <summary>
        /// 获取指定类型的服务对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reference"></param>
        /// <returns></returns>
        public T GetService<T>(IServiceReference reference)
        {
            object service = GetService(reference);
            return (T)service;
        }

        /// <summary>
        /// 注销服务
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        public bool UngetService(IServiceReference reference)
        {
            CheckValid();

            return framework.ServiceRegistry.UngetService(this, (ServiceReferenceImpl)reference);
        }

        /// <summary>
        /// 注册解析程序集的钩子
        /// </summary>
        /// <param name="hook"></param>
        public void RegisterAssemblyResolverHook(IAssemblyResolverHook hook)
        {
             framework.AssemblyResolver.RegisterAssemblyResolverHook(hook);
        }
        #endregion

        #region 事件
        /// <summary>
        /// 扩展变化事件
        /// </summary>
        public event EventHandler<ExtensionEventArgs> ExtensionChanged
        {
            add { this.framework.ExtensionChanged += value; }
            remove { this.framework.ExtensionChanged -= value; }
        }

        /// <summary>
        /// 扩展点变化事件
        /// </summary>
        public event EventHandler<ExtensionPointEventArgs> ExtensionPointChanged
        {
            add { this.framework.ExtensionPointChanged += value; }
            remove { this.framework.ExtensionPointChanged -= value; }
        }
        #endregion
    }
}

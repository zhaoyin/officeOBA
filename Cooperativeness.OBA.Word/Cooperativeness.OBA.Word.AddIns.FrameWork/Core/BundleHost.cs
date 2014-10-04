using System;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Loader;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Service;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Core
{
    /// <summary>
    /// 定义宿主插件对象
    /// </summary>
    internal class BundleHost : AbstractBundle
    {
        #region 字段
        /** 当前插件和所有片段插件的上下文对象 */
        protected BundleContextImpl context;
        /** 片段插件列表 */
        protected BundleFragment[] fragments;
        /** 插件锁 */
        protected object fragmentLock = new object();
        /** 插件加载器代理 */
        protected BundleLoaderProxy proxy;

        #endregion

        #region 构造函数
        public BundleHost(IBundleData bundledata, Framework framework)
            : base(bundledata, framework)
        {
            context = null;
            fragments = null;
        }

        #endregion

        #region 属性
        /// <summary>
        /// 获取插件及所有插件片段的上下文对象
        /// </summary>
        internal override BundleContextImpl BundleContext
        {
            get
            {
                if (context == null)
                {
                    // only create the context if we are starting, active or stopping
                    // this is so that SCR can get the context for lazy-start bundles
                    if ((state & (BundleState.Starting | BundleState.Active | BundleState.Stopping)) != 0)
                        context = CreateContext();
                }
                return (context);
            }
        }

        /// <summary>
        /// 获取片段插件列表
        /// </summary>
        internal override BundleFragment[] Fragments
        {
            get
            {
                lock (fragmentLock)
                {
                    if (fragments == null)
                        return null;
                    BundleFragment[] result = new BundleFragment[fragments.Length];
                    Array.Copy(fragments, result, result.Length);
                    return result;
                }
            }
        }

        /// <summary>
        /// 获取插件加载器
        /// </summary>
        internal override IBundleLoader BundleLoader
        {
            get { return this.GetBundleLoader(); }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 创建插件上下文对象
        /// </summary>
        /// <returns></returns>
        protected BundleContextImpl CreateContext()
        {
            return new BundleContextImpl(this);
        }

        /// <summary>
        /// 附加片段插件
        /// </summary>
        /// <param name="fragment"></param>
        internal void AttachFragment(BundleFragment fragment)
        {
            if (fragments == null)
            {
                fragments = new BundleFragment[] { fragment };
            }
            else
            {
                bool inserted = false;
                // We must keep our fragments ordered by bundle ID; or 
                // install order.
                BundleFragment[] newFragments = new BundleFragment[fragments.Length + 1];
                for (int i = 0; i < fragments.Length; i++)
                {
                    if (fragment == fragments[i])
                        return; // this fragment is already attached
                    if (!inserted && fragment.BundleId < fragments[i].BundleId)
                    {
                        newFragments[i] = fragment;
                        inserted = true;
                    }
                    newFragments[inserted ? i + 1 : i] = fragments[i];
                }
                if (!inserted)
                    newFragments[newFragments.Length - 1] = fragment;
                fragments = newFragments;
            }
        }

        /// <summary>
        /// 获取当前插件使用的所有服务引用列表
        /// </summary>
        /// <returns></returns>
        public override IServiceReference[] GetServicesInUse()
        {
            if (context == null)
            {
                return null;
            }

            return context.Framework.ServiceRegistry.GetServicesInUse(context);
        }

        /// <summary>
        /// 获取所有插件注册的服务的服务引用列表
        /// </summary>
        /// <returns></returns>
        public override IServiceReference[] GetRegisteredServices()
        {
            if (context == null)
            {
                return null;
            }

            return context.Framework.ServiceRegistry.GetRegisteredServices(context);
        }

        /// <summary>
        /// 加载插件
        /// </summary>
        internal override void Load()
        {
            proxy = null;
        }

        protected IBundleLoader GetBundleLoader()
        {
            BundleLoaderProxy curProxy = GetLoaderProxy();
            return curProxy == null ? null : curProxy.BundleLoader;
        }

        /// <summary>
        /// 获取加载器代理
        /// </summary>
        /// <returns></returns>
        public BundleLoaderProxy GetLoaderProxy()
        {
            if (proxy != null)
                return proxy;
            proxy = new BundleLoaderProxy(this);
            return proxy;
        }

        /// <summary>
        /// 获取类加载器
        /// </summary>
        /// <returns></returns>
        public IClassLoader GetClassLoader()
        {
            BundleLoaderProxy curProxy = GetLoaderProxy();
            IBundleLoader loader = curProxy == null ? null : curProxy.BundleLoader;
            IClassLoader bcl = loader == null ? null : curProxy.CreateBundleLoader();
            return bcl;
        }

        /// <summary>
        /// 加载指定的类名的类型
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public override Type LoadClass(string className)
        {
            IBundleLoader loader = this.BundleLoader;
            if (loader == null)
                throw new Exception();
            try
            {
                return loader.LoadClass(className);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="loadMode"></param>
        /// <returns></returns>
        public override object LoadResource(string resourceName, ResourceLoadMode loadMode)
        {
            IBundleLoader loader = this.BundleLoader;
            if (loader == null)
                return null;
            object result = loader.LoadResource(resourceName, loadMode);
            return result;
        }

        /// <summary>
        /// 启动插件处理器
        /// </summary>
        /// <param name="option"></param>
        protected override void StartWorker(BundleOptions option)
        {
            if (!framework.IsActive || state == BundleState.Active)
                return;
            if (state == BundleState.Installed)
            {
                if (!framework.packageAdmin.ResolveBundles(new IBundle[] { this }))
                    throw new BundleException("插件解析失败", BundleExceptionType.RESOLVE_ERROR);
            }
            if (option == BundleOptions.Transient)
            {
                if (state == BundleState.Resolved)
                {
                    // 设置当前状态为待启动状态
                    state = BundleState.Starting;
                    // 设置加载器状态为懒加载待启动状态
                    AbstractClassLoader loader = (AbstractClassLoader)this.BundleLoader;
                    loader.SetLazyTrigger();
                    // 完成状态变化
                    CompleteStateChange();
                    // 解析扩展包
                    framework.packageAdmin.ResolveExtension(this);
                }
                return;
            }
            state = BundleState.Starting;
            // 发布启动事件
            try
            {
                this.BundleContext.Start();
                if (framework.IsActive)
                {
                    state = BundleState.Active;
                    framework.packageAdmin.ResolveExtension(this);
                    CompleteStateChange();
                }
            }
            catch (Exception ex)
            {
                state = BundleState.Stopping;
                context.Close();
                context = null;
                state = BundleState.Resolved;
                // 发布事件
                throw ex;
            }
        }

        /// <summary>
        /// 停止插件处理器
        /// </summary>
        /// <param name="options"></param>
        protected override void StopWorker(BundleOptions option)
        {
            if (framework.IsActive)
            {
                if ((state & (BundleState.Stopping | BundleState.Resolved | BundleState.Installed))
                    != BundleState.None)
                {
                    return;
                }
                state = BundleState.Stopping;
                // 发布事件
                try
                {
                    if (context != null)
                        context.Stop();
                }
                finally
                {
                    if (context != null)
                    {
                        context.Close();
                        context = null;
                    }
                    state = BundleState.Resolved;
                    // 发布事件
                }
            }
        }
        #endregion
    }
}

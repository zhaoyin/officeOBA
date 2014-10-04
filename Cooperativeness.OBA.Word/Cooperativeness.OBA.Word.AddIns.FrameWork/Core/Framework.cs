using System;
using System.Collections;
using System.Linq;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Adaptor;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Events;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Loader;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Package;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Service;
using Cooperativeness.OBA.Word.Tools;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Core
{
    /// <summary>
    /// 定义插件框架
    /// </summary>
    public class Framework
    {
        private static readonly Logger Log = new Logger(typeof(Framework));
        #region 字段
        /** 框架适配器. */
	    internal IFrameworkAdaptor adaptor;
	    /** 框架属性对象. */
	    internal BundleProperty properties;
        /** 插件仓库 */
        private BundleRepository bundles;
	    /** 指示框架是否已启动 */
	    internal bool active;
	    /** 启动级别管理器 */
	    internal StartLevelManager startLevelManager;
        /** 扩展管理器 */
        private ExtensionAdmin extensionAdmin;
	    /** 服务注册中心 */
	    internal ServiceRegistry serviceRegistry;
        /** 包管理器 */
        internal IPackageAdmin packageAdmin;
        /** 插件启动级别 */
	    internal static readonly int BUNDLEEVENT = 1;
	    /** 系统框架对象 */
        internal SystemBundle systemBundle;
        /** 程序集解析器对象 */
        internal IAssemblyResolving assemblyResolver;
        /** 扩展变化事件 */
        internal event EventHandler<ExtensionEventArgs> ExtensionChanged;
        /** 扩展点变化事件 */
        internal event EventHandler<ExtensionPointEventArgs> ExtensionPointChanged;

        #endregion

        #region 构造函数
        internal Framework(IFrameworkAdaptor adaptor)
        {
            this.Initialize(adaptor);
        }

        #endregion

        #region 属性
        /// <summary>
        /// 获取一个值用来指示当前框架是否已激活
        /// </summary>
        public bool IsActive 
        {
            get { return this.active; } 
        }

        /// <summary>
        /// 获取插件框架适配器
        /// </summary>
        internal IFrameworkAdaptor Adaptor
        {
            get { return adaptor; }
        }

        /// <summary>
        /// 获取服务注册中心
        /// </summary>
        /// <returns></returns>
        internal ServiceRegistry ServiceRegistry
        {
            get { return serviceRegistry; }
        }

        /// <summary>
        /// 获取插件仓库
        /// </summary>
        /// <returns></returns>
        internal BundleRepository Bundles
        {
            get { return (bundles); }
        }

        /// <summary>
        /// 获取扩展管理器
        /// </summary>
        internal ExtensionAdmin ExtensionAdmin
        {
            get { return this.extensionAdmin; }
        }

        /// <summary>
        /// 获取组件解析器
        /// </summary>
        internal AssemblyResolvingImpl AssemblyResolver
        {
            get { return (AssemblyResolvingImpl)this.assemblyResolver; }
        }

        #endregion

        #region 方法
        /// <summary>
        /// 初始化插件框架
        /// </summary>
        /// <param name="adaptor"></param>
        internal void Initialize(IFrameworkAdaptor adaptor)
        {
            this.adaptor = adaptor;
            active = false;
            /* 初始化适配器 */
            adaptor.Initialize();
            try
            {
                adaptor.InitializeStorage();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
            /* 初始化框架属性 */
            InitializeProperties(adaptor.Properties);
            /* 初始化包管理器 */
            packageAdmin = new PackageAdminImpl(this);
            /* 初始化扩展管理器 */
            extensionAdmin = new ExtensionAdmin(this);
            /* 初始化程序集解析器 */
            assemblyResolver = new AssemblyResolvingImpl(this);
            /* 初始化启动级别管理器 */
            startLevelManager = new StartLevelManager(this);
            /* 创建服务注册中心 */
            serviceRegistry = new ServiceRegistry(this);
            /* 创建系统插件 */
            this.CreateSystemBundle();
            /* 为安装的插件创建插件对象. */
            IBundleData[] bundleDatas = adaptor.InstalledBundles.ToArray();
            bundles = new BundleRepository(this);
            /* 添加系统插件到插件仓库 */
            bundles.Add(systemBundle);
            if (bundleDatas != null)
            {
                for (int i = 0; i < bundleDatas.Length; i++)
                {
                    try
                    {
                        AbstractBundle bundle = AbstractBundle.CreateBundle(bundleDatas[i], this, true);
                        bundles.Add(bundle);
                    }
                    catch (BundleException be)
                    {
                        Log.Debug(be);
                        // This is not a fatal error. Publish the framework event.
                        //publishFrameworkEvent(FrameworkEvent.ERROR, systemBundle, be);
                    }
                }
            }
        }

        /// <summary>
        /// 初始化框架属性对象
        /// </summary>
        /// <param name="property"></param>
        private void InitializeProperties(BundleProperty properties)
        {
            this.properties = properties;
        }

        /// <summary>
        /// 创建系统插件实体对象
        /// </summary>
        private void CreateSystemBundle()
        {
            try
            {
                systemBundle = new SystemBundle(this);
                systemBundle.BundleData.SetBundle(systemBundle);
            }
            catch (BundleException e)
            {
                throw new Exception(e.Message, e);
            }
        }

        /// <summary>
        /// 关闭矿建,销毁框架实例
        /// </summary>
        public void Close()
        {
            if (adaptor == null)
                return;
            if (active) Shutdown();
            lock (bundles)
            {
                IList allBundles = bundles.GetBundles();
                int size = allBundles.Count;
                for (int i = 0; i < size; i++)
                {
                    AbstractBundle bundle = (AbstractBundle)allBundles[i];
                    bundle.Close();
                }
                bundles.RemoveAllBundles();
            }
            serviceRegistry = null;
            adaptor = null;
            assemblyResolver.Stop();
            assemblyResolver = null;
        }

        /// <summary>
        /// 启动框架
        /// </summary>
        public void Launch()
        {
            /* 如果框架已经启动,直接返回 */
            if (active)
            {
                return;
            }
            /* mark framework as started */
            active = true;
            assemblyResolver.Start();
            systemBundle.Resume();
        }

        /// <summary>
        /// 关闭框架
        /// </summary>
        public void Shutdown()
        {
            /* Return if framework already stopped */
            if (!active) return;
            systemBundle.State = BundleState.Stopping;
            systemBundle.BundleContext.Stop();
            /* 停止所有以运行的插件 */
            StopAllBundles();
            /* mark framework as stopped */
            active = false;
            
        }

        /// <summary>
        /// 停止插件插件
        /// </summary>
        /// <param name="bundle"></param>
        /// <param name="options"></param>
        private void StopBundle(IBundle bundle, BundleOptions options)
        {
            try
            {
                bundle.Stop(options);
            }
            catch (BundleException e)
            {
                Log.Debug(e);
                if ((bundle.State & BundleState.Resolved) != 0)
                {
                    Log.Debug("(bundle.State & BundleState.Resolved) != 0");
                    // 记录日志
                }
            }
        }

        /// <summary>
        /// 停止所有
        /// </summary>
        /// <param name="bundles"></param>
        private void StopAllBundles()
        {
            IList allBundles = bundles.GetBundles();
            foreach (AbstractBundle bundle in allBundles)
            {
                if (bundle.IsFragment) continue;
                StopBundle(bundle, BundleOptions.General);
            }
        }

        /// <summary>
        /// 根据插件标示获取插件对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal AbstractBundle GetBundle(long id)
        {
            lock (bundles)
            {
                return bundles.GetBundle(id);
            }
        }

        /// <summary>
        /// 获取系统上下文对象
        /// </summary>
        /// <returns></returns>
        internal BundleContextImpl GetSystemBundleContext()
        {
            if (systemBundle == null)
                return null;
            return systemBundle.BundleContext;
        }

        /// <summary>
        /// 根据插件版本号和插件唯一表示名获取插件
        /// </summary>
        /// <param name="symbolicName"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        internal AbstractBundle getBundleBySymbolicName(string symbolicName, Version version)
        {
            lock (bundles)
            {
                return bundles.GetBundle(symbolicName, version);
            }
        }

        /// <summary>
        /// 获取所有的插件
        /// </summary>
        /// <returns></returns>
        internal AbstractBundle[] getAllBundles()
        {
            lock (bundles)
            {
                IList allBundles = bundles.GetBundles();
                int size = allBundles.Count;
                if (size == 0)
                {
                    return (null);
                }
                AbstractBundle[] bundlelist = new AbstractBundle[size];
                allBundles.CopyTo(bundlelist, 0);
                return (bundlelist);
            }
        }


	    /// <summary>
	    /// 根据位置获取插件对象
	    /// </summary>
	    /// <param name="location"></param>
	    /// <returns></returns>
        internal AbstractBundle GetBundleByLocation(string location)
        {
            lock (bundles)
            {
                // this is not optimized; do not think it will get called
                // that much.
                string finalLocation = location;

                IList allBundles = bundles.GetBundles();
                int size = allBundles.Count;
                for (int i = 0; i < size; i++)
                {
                    AbstractBundle bundle = (AbstractBundle)allBundles[i];
                    if (finalLocation.Equals(bundle.Location))
                    {
                        return (bundle);
                    }
                }
                return (null);
            }
        }

	    /// <summary>
	    /// 根据标示名获取插件对象列表
	    /// </summary>
	    /// <param name="symbolicName"></param>
	    /// <returns></returns>
        internal AbstractBundle[] getBundleBySymbolicName(string symbolicName)
        {
            lock (bundles)
            {
                return bundles.GetBundles(symbolicName);
            }
        }

        /// <summary>
        /// 发布扩展变化事件
        /// </summary>
        /// <param name="action"></param>
        /// <param name="extension"></param>
        internal void PublishExtensionEvent(object sender, ExtensionEventArgs e)
        {
            if (this.ExtensionChanged != null)
            {
                this.ExtensionChanged.Invoke(sender, e);
            }
        }

        /// <summary>
        /// 发布扩展点变化事件
        /// </summary>
        /// <param name="action"></param>
        /// <param name="extension"></param>
        internal void PublishExtensionPointEvent(object sender,ExtensionPointEventArgs e)
        {
            if (this.ExtensionPointChanged != null)
            {
                this.ExtensionPointChanged.Invoke(sender, e);
            }
        }
        #endregion
    }
}

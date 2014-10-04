using System.Collections.Generic;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Adaptor;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Core;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Package;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Service;
using Cooperativeness.OBA.Word.Tools;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork
{
    /// <summary>
    /// 定义插件启动器
    /// </summary>
    public class BundleStarter
    {
        private static readonly Logger Log = new Logger(typeof(BundleStarter));
        #region 字段
        
        //private static BundleStarter instance;
        private object lockObj = new object();
        private IFrameworkAdaptor adaptor;
        private IBundleContext context;
        private Framework framework;
        private bool running;

        
        #endregion

        #region 构造函数
        public BundleStarter()
        {
            running = false;
        }

        //static BundleStarter()
        //{
        //    lockObj = new object();
        //}

        #endregion

        #region 属性
        ///// <summary>
        ///// 获取插件启动器实例
        ///// </summary>
        //public static BundleStarter Instance
        //{
        //    get
        //    {
        //        lock (lockObj)
        //        {
        //            if (instance == null)
        //                instance = new BundleStarter();
        //            return instance;
        //        }
        //    }
        //}

        /// <summary>
        /// 获取一个值，用来只是当前启动器是处于运行状态
        /// </summary>
        public bool IsRunning
        {
            get { return running; }
        }

        #endregion

        #region 方法
        ///// <summary>
        ///// 创建插件启动器
        ///// </summary>
        ///// <returns></returns>
        //public static BundleStarter CreateStarter()
        //{
        //    lock (lockObj)
        //    {
        //        if (instance == null)
        //            instance = new BundleStarter();
        //        return instance;
        //    }
        //}

        /// <summary>
        /// 启动插件框架
        /// </summary>
        public void Start()
        {
            // 检查插件启动器是否已启动
            if (running)
                throw new IllegalStateException("Bundle Framework has started.");
            // 初始化适配器
            adaptor = CreateAdaptor();
            // 初始化框架
            framework = new Core.Framework(adaptor);
            // 初始化上下文
            context = framework.GetBundle(0).BundleContext;
            // 启动框架
            framework.Launch();
            // 加载插件
            IBundle[] startBundles = LoadBasicBundles();
            // 设置当前框架的启动级别
            SetStartLevel(GetStartLevel());
            // 确认所有启动的插件是否已激活
            //EnsureBundlesActive(startBundles);
            // 设置当前启动器已启动
            running = true;
        }

        /// <summary>
        /// 插件启动器关闭
        /// </summary>
        public void Shutdown()
        {
            if (!running || framework == null)
                return;
            framework.Close();
            framework = null;
            context = null;
            running = false;
        }

        /// <summary>
        /// 创建适配器
        /// </summary>
        /// <returns></returns>
        private IFrameworkAdaptor CreateAdaptor()
        {
            return new BaseAdaptor();
        }

        /// <summary>
        /// 加载基础插件，并获启动列表
        /// </summary>
        /// <returns></returns>
        private IBundle[] LoadBasicBundles()
        {
            // 获取待初始化的插件列表
            IBundle[] curInitBundles = context.Bundles;
            // 插件解析处理
            if (!ResolvePackages(curInitBundles))
                return null;
            // 启动插件
            StartBundles(curInitBundles);

            return curInitBundles;
        }

        /// <summary>
        /// 获取默认的启动级别
        /// </summary>
        /// <returns></returns>
        private int GetStartLevel()
        {
            return ConfigConstant.DEFAULT_INITIAL_STARTLEVEL;
        }

        /// <summary>
        /// 启动列表中所有的插件
        /// </summary>
        /// <param name="bundles"></param>
        private void StartBundles(IBundle[] bundles)
        {
            IServiceReference reference = context.GetServiceReference(typeof(IStartLevel).FullName);
            IStartLevel startService = null;
            if (reference != null)
                startService = context.GetService<IStartLevel>(reference);
            try
            {
                foreach (AbstractBundle bundle in bundles)
                {
                    if (bundle == null) break;
                    if (bundle.BundleData.Policy == ActivatorPolicy.Lazy)
                        StartBundle(bundle, BundleOptions.Transient);
                    else
                    {
                        // always set the startlevel incase it has changed (bug 111549)
                        // this is a no-op if the level is the same as previous launch.
                        if (bundle.StartLevel == 0 && startService != null)
                            startService.SetBundleStartLevel(bundle, GetStartLevel());
                        StartBundle(bundle, BundleOptions.General);
                    }
                }
            }
            finally
            {
                if (reference != null) context.UngetService(reference);
            }

        }

        /// <summary>
        /// 启动插件
        /// </summary>
        /// <param name="bundle"></param>
        /// <param name="options"></param>
        private void StartBundle(IBundle bundle, BundleOptions options)
        {
            try
            {
                bundle.Start(options);
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
        /// 设置启动级别
        /// </summary>
        /// <param name="value"></param>
        private void SetStartLevel(int value)
        {
            IServiceReference reference = context.GetServiceReference(typeof(IStartLevel).FullName);
            var startService = context.GetService<IStartLevel>(reference);
            if (startService == null) return;

            startService.StartLevel = (value);
            context.UngetService(reference);

        }

        /// <summary>
        /// 确认插件是否已激活
        /// </summary>
        /// <param name="bundles"></param>
        private void EnsureBundlesActive(IBundle[] bundles)
        {
            IServiceReference reference = context.GetServiceReference(typeof(IStartLevel).FullName);
            var startService = context.GetService<IStartLevel>(reference);
            try
            {

                for (int i = 0; i < bundles.Length; i++)
                {
                    if (bundles[i].State != BundleState.Active)
                    {
                        if (bundles[i].State == BundleState.Installed)
                        {
                            // Log that the bundle is not resolved
                            continue;
                        }
                        //// check that the startlevel allows the bundle to be active (111550
                        //if (startService != null && (startService.GetBundleStartLevel(bundles[i]) <= startService.StartLevel))
                        //{
                        //    // Log that the bundle's level 
                        //}
                    }
                }
            }
            finally
            {
                context.UngetService(reference);
            }
        }

        /// <summary>
        /// 获取用户指定的类型的服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetService<T>()
        {
            string clazz = typeof(T).FullName;
            IServiceReference reference = context.GetServiceReference(clazz);
            if (reference == null) return default(T);
            T service = context.GetService<T>(reference);
            context.UngetService(reference);
            return service;
        }

        /// <summary>
        /// 获取服务注册者
        /// </summary>
        /// <param name="clazzes"></param>
        /// <param name="service"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public IServiceRegistration RegisterService(string[] clazzes,
            object service, IDictionary<string, object> properties)
        {
            return Register(clazzes, service, properties);
        }

        /// <summary>
        /// 解析插件
        /// </summary>
        /// <param name="bundles"></param>
        /// <returns></returns>
        private bool ResolvePackages(IBundle[] bundles)
        {
            IServiceReference packageAdminRef = context.GetServiceReference(typeof(IPackageAdmin).FullName);
            PackageAdminImpl packageAdmin = null;
            if (packageAdminRef != null)
                packageAdmin = (PackageAdminImpl)context.GetService(packageAdminRef);
            if (packageAdmin == null)
                return false;
            // TODO this is such a hack it is silly.  There are still cases for race conditions etc
            // but this should allow for some progress...

            bool isResolved = packageAdmin.ResolveBundles(bundles);
            context.UngetService(packageAdminRef);

            return isResolved;
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        /// <param name="names"></param>
        /// <param name="service"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        internal IServiceRegistration Register(string[] names, object service, IDictionary<string, object> properties)
        {
            if (properties == null)
                properties = new Dictionary<string, object>();
            properties.Add(ConfigConstant.SERVICE_RANKING, int.MaxValue);
            properties.Add(ConfigConstant.SERVICE_PID, this.context.Bundle.BundleId + "." + service.GetType().FullName); //$NON-NLS-1$
            return this.context.RegisterService(names, service, properties);
        }

        #endregion
    }
}

using System;
using System.Threading;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Loader;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Package;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Service;
using Cooperativeness.OBA.Word.Tools;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Core
{
    /// <summary>
    /// 定义抽象的插件对象
    /// </summary>
    internal abstract class AbstractBundle : IBundle, IComparable
    {
        private static readonly Logger Log = new Logger(typeof(AbstractBundle));
        #region 字段
        /** 当前插件的框架对象. */
        protected Framework framework;
        /** 插件的状态. */
        protected BundleState state;
        /** 插件解析状态. */
        protected ResolveState resolveState;
        /** 插件数据对象 */
        protected BaseData bundledata;
        /** 状态变化中的标记 */
        protected volatile Thread stateChanging;
        /** 插件规格对象 */
        protected IBundleSpecification bundleSpecification;
        /** 状态变换同步锁 */
        protected AutoResetEvent statechangeLock;

        #endregion

        #region 构造函数
        protected AbstractBundle(IBundleData bundledata, Framework framework)
        {
            resolveState = ResolveState.Unresolved;
            state = BundleState.Installed;
            this.bundledata = (BaseData)bundledata;
            this.bundledata.SetBundle(this);
            this.framework = framework;
            statechangeLock = new AutoResetEvent(true);
        }

        #endregion

        #region 属性
        /// <summary>
        /// 返回当前插件的状态
        /// </summary>
        public BundleState State
        {
            get { return (state); }
            internal set { this.state = value; }
        }

        /// <summary>
        /// 返回当前插件的解析状态
        /// </summary>
        internal ResolveState ResolveState
        {
            get { return resolveState; }
            set { resolveState = value; }
        }

        /// <summary>
        /// 获取当前插件对象所依赖的框架对象
        /// </summary>
        public Framework Framework
        {
            get { return framework; }
        }

        /// <summary>
        /// 获取一个值，该值用来指示当前插件是否处于激活或运行状态
        /// </summary>
        internal bool IsActive
        {
            get { return ((state & (BundleState.Active | BundleState.Starting)) != 0); }
        }

        /// <summary>
        /// 获取一个值，该值用来指示当前插件是否懒启动
        /// </summary>
        internal bool IsLazyStart
        {

            get { return bundledata.Policy == ActivatorPolicy.Lazy; }
        }

        /// <summary>
        /// 获取一个值，该值指示当前插件是否已解析
        /// </summary>
        public bool IsResolved
        {
            get { return (state & BundleState.Installed) == 0; }
        }

        /// <summary>
        /// 获取插件的唯一标示名称
        /// </summary>
        public string SymbolicName
        {
            get { return bundledata.SymbolicName; }
        }

        /// <summary>
        /// 获取插件的最后修改时间
        /// </summary>
        public long LastModified
        {
            get { return bundledata.LastModified; }
        }

        /// <summary>
        /// 获取插件数据
        /// </summary>
        public IBundleData BundleData
        {
            get { return bundledata; }
        }

        /// <summary>
        /// 获取插件版本信息
        /// </summary>
        public Version Version
        {
            get { return bundledata.Version; }
        }

        /// <summary>
        /// 获取插件的标识
        /// </summary>
        public long BundleId
        {
            get { return (bundledata.BundleId); }
        }

        /// <summary>
        /// 获得插件的位置
        /// </summary>
        public string Location
        {
            get { return bundledata.Location; }
        }

        /// <summary>
        /// 获取片段插件集合
        /// </summary>
        /// <returns></returns>
        internal virtual BundleFragment[] Fragments
        {
            get { return null; }
        }


        /// <summary>
        /// 获取宿主插件集合
        /// </summary>
        /// <returns></returns>
        internal virtual BundleHost[] Hosts
        {
            get { return null; }
        }

        /// <summary>
        /// 获取一个值，用来指示当前插件是否是片段插件
        /// </summary>
        internal virtual bool IsFragment
        {
            get { return false; }
        }

        /// <summary>
        /// 获取插件加载器
        /// </summary>
        internal abstract IBundleLoader BundleLoader { get; }

        /// <summary>
        /// 获取插件的上下文对象
        /// </summary>
        public IBundleContext Context
        {
            get { return this.BundleContext as IBundleContext; }
        }

        /// <summary>
        /// 获取插件的上下文对象
        /// </summary>
        internal abstract BundleContextImpl BundleContext { get; }

        /// <summary>
        /// 获取状态变化标志
        /// </summary>
        public Thread StateChanging
        {
            get { return stateChanging; }
        }

        /// <summary>
        /// 获取插件的启动级别
        /// </summary>
        internal int StartLevel
        {
            get { return bundledata.StartLevel; }
        }

        /// <summary>
        /// 获取或设置插件规格对象
        /// </summary>
        internal IBundleSpecification BundleSpecification
        {
            get { return this.bundleSpecification; }
            set { this.bundleSpecification = value; }
        }

        /// <summary>
        /// 获取或设置插件的启动策略
        /// </summary>
        internal ActivatorPolicy ActivatorPolicy
        {
            get { return this.bundledata.Policy; }
            set { this.bundledata.Policy = value; }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 根据插件数据对象创建插件对象
        /// </summary>
        /// <param name="bundledata"></param>
        /// <param name="framework"></param>
        /// <param name="isSetBundle"></param>
        /// <returns></returns>
        internal static AbstractBundle CreateBundle(IBundleData bundledata, Framework framework, bool isSetBundle)
        {
            AbstractBundle result;
            if (bundledata.BundleType == BundleType.Fragment)
                result = new BundleFragment(bundledata, framework);
            else
                result = new BundleHost(bundledata, framework);
            if (isSetBundle)
                bundledata.SetBundle(result);
            return result;
        }

        /// <summary>
        /// 加载插件
        /// </summary>
        internal abstract void Load();

        /// <summary>
        /// 关闭插件
        /// </summary>
        internal virtual void Close()
        {
            state = BundleState.Resolved;
        }

        /// <summary>
        /// 加载并实例化插件激活器
        /// </summary>
        /// <returns></returns>
        internal IBundleActivator LoadBundleActivator()
        {
            /* 如果激活器存在，加载插件激活器 */
            string activatorClassName = bundledata.Activator;
            if (activatorClassName != null)
            {
                try
                {
                    Type activatorClass = LoadClass(activatorClassName);
                    /* 为当前插件创建激活器实例 */
                    return Activator.CreateInstance(activatorClass) as IBundleActivator;
                }
                catch (Exception ex)
                {
                    Log.Debug(ex);
                    throw new BundleException(string.Format("加载激活器失败-[Bundle：{0}]。", this.bundledata.SymbolicName), BundleExceptionType.ACTIVATOR_ERROR);
                }
            }
            return null;
        }

        /// <summary>
        /// 从当前插件中加载指定类名的类型
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public abstract Type LoadClass(string className);

        /// <summary>
        /// 从当前插件中根据资源名称获取资源
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="loadMode"></param>
        /// <returns></returns>
        public abstract object LoadResource(string resourceName, ResourceLoadMode loadMode);

        /// <summary>
        /// 启动插件
        /// </summary>
        public virtual void Start()
        {
            Start(BundleOptions.General);
        }

        /// <summary>
        /// 启动插件
        /// </summary>
        public virtual void Start(BundleOptions option)
        {
            BeginStateChange();
            try
            {
                StartWorker(option);
            }
            finally
            {
                CompleteStateChange();
            }
        }

        /// <summary>
        /// 内部启动插件工作者
        /// </summary>
        /// <param name="options"></param>
        protected abstract void StartWorker(BundleOptions option);

        /// <summary>
        /// 停止插件
        /// </summary>
        public virtual void Stop()
        {
            this.Stop(BundleOptions.General);
        }

        /// <summary>
        /// 停止插件
        /// </summary>
        public virtual void Stop(BundleOptions option)
        {
            BeginStateChange();
            try
            {
                StopWorker(option);
            }
            finally
            {
                CompleteStateChange();
            }
        }


        /// <summary>
        /// 内部停止插件的工作者
        /// </summary>
        /// <param name="option"></param>
        protected abstract void StopWorker(BundleOptions option);

        /// <summary>
        /// 开始准备状态变化
        /// </summary>
        protected void BeginStateChange()
        {
            statechangeLock.WaitOne();
            bool doubleFault = false;
            while (true)
            {
                if (stateChanging == null)
                {
                    stateChanging = Thread.CurrentThread;
                    return;
                }
                if (doubleFault || (stateChanging == Thread.CurrentThread))
                {
                    throw new BundleException();
                }
                try
                {
                    Thread.Sleep(5000);
                }
                catch (Exception ex)
                {
                    Log.Debug(ex);
                    //Nothing to do
                }
                doubleFault = true;
            }
        }

        /// <summary>
        /// 状态变化完成
        /// </summary>
        protected void CompleteStateChange()
        {
            if (stateChanging == Thread.CurrentThread)
            {
                stateChanging = null;
                statechangeLock.Set();
                //Thread..notify();
                /*
                 * notify one waiting thread that the
                 * state change is complete
                 */
            }
        }

        /// <summary>
        /// 将当前插件转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            String name = bundledata.SymbolicName;
            if (name == null)
                name = "unknown"; //$NON-NLS-1$
            return (name + '_' + bundledata.Version + " [" + BundleId + "]"); //$NON-NLS-1$ //$NON-NLS-2$
        }

        /// <summary>
        /// 比较操作
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            int slcomp = StartLevel - ((AbstractBundle)obj).StartLevel;
            if (slcomp != 0)
            {
                return slcomp;
            }
            long idcomp = BundleId - ((AbstractBundle)obj).BundleId;
            return (idcomp < 0L) ? -1 : ((idcomp > 0L) ? 1 : 0);
        }

        /// <summary>
        /// 将当前设置为已解析状态
        /// </summary>
        internal void Resolve()
        {
            if (state == BundleState.Installed)
            {
                state = BundleState.Resolved;
                // Do not publish RESOLVED event here.  This is done by caller 
                // to Resolve if appropriate.
            }
        }

        /// <summary>
        /// 测试当前状态是否处于转换中
        /// </summary>
        /// <param name="thread"></param>
        /// <returns></returns>
        public bool TestStateChanging(object thread)
        {
            return stateChanging == thread;
        }

        /// <summary>
        /// 获取当前插件所有
        /// </summary>
        /// <returns></returns>
        public abstract IServiceReference[] GetRegisteredServices();

        /// <summary>
        /// 获取插件正在使用的所有服务的服务引用列表
        /// </summary>
        /// <returns></returns>
        public abstract IServiceReference[] GetServicesInUse();

        #endregion

        #region 内部类
        /// <summary>
        /// 定义插件状态异常类
        /// </summary>
        internal class BundleStatusException : Exception, IStatusExpception
        {
            private StatusCode code;
            private object status;

            BundleStatusException(string message, StatusCode code, object status)
                : base(message)
            {
                this.code = code;
                this.status = status;
            }

            public object Status
            {
                get { return status; }
            }

            public StatusCode StatusCode
            {
                get { return code; }
            }
        }
        
        #endregion

    }
}

using System;
using System.Reflection;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Loader;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Core
{
    /// <summary>
    /// 定义系统插件对象
    /// </summary>
    internal class SystemBundle : BundleHost, IFramework
    {
        #region 字段
        private IBundleLoader bundleLoader;

        #endregion

        #region 构造函数
        public SystemBundle(Framework framework)
            : base(new SystemBundleData(), framework)
        {
            this.state = BundleState.Resolved;
            this.context = CreateContext();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 指示系统插件是被解释的.
        /// </summary>
        /// <returns></returns>
        protected bool IsUnresolved()
        {
            return (false);
        }

        internal override IBundleLoader BundleLoader
        {
            get
            {
                if (this.bundleLoader == null)
                    bundleLoader = new SystemBundleLoader(this);
                return bundleLoader;
            }
        }

        #endregion

        #region 方法
        /// <summary>
        /// 初始化系统插件
        /// </summary>
        public void Init()
        {
            // no op for internal representation
        }

        /// <summary>
        /// 加载系统插件
        /// </summary>
        internal override void Load()
        {
            ///base.Load();
        }

        /// <summary>
        /// 关闭插件
        /// </summary>
        internal override void Close()
        {
            context.Close();
            context = null;
        }

        /// <summary>
        /// 加载指定类型名的类型
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public override Type LoadClass(string className)
        {
            Assembly assembly = this.GetType().Assembly;
            return assembly.GetType(className);
        }

        /// <summary>
        /// 加载用户指定的资源
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="loadMode"></param>
        /// <returns></returns>
        public override object LoadResource(string resourceName, ResourceLoadMode loadMode)
        {
            return null;
        }

        /// <summary>
        /// 启动插件
        /// </summary>
        public override void Start()
        {
            // Nothing to do
        }

        /// <summary>
        /// 启动插件
        /// </summary>
        public override void Start(BundleOptions option)
        {
            // Nothing to do
        }

        /// <summary>
        /// 启动系统插件
        /// </summary>
        internal void Resume()
        {
            /* initialize the startlevel service */
            framework.startLevelManager.initialize();

            /* Load all installed bundles */
            LoadInstalledBundles(framework.startLevelManager.GetInstalledBundles(framework.Bundles, false));
            /* Start the system bundle */
            try
            {
                framework.systemBundle.state = BundleState.Starting;
                framework.systemBundle.context.Start();
            }
            catch (BundleException be)
            {
                throw be;
            }

        }

        /// <summary>
        /// 加载已安装的所有插件
        /// </summary>
        /// <param name="installedBundles"></param>
        private void LoadInstalledBundles(AbstractBundle[] installedBundles)
        {
            for (int i = 0; i < installedBundles.Length; i++)
            {
                AbstractBundle bundle = installedBundles[i];
                bundle.Load();
            }
        }

        /// <summary>
        /// 停止插件
        /// </summary>
        public override void Stop()
        {
            // Nothing to do
        }

        /// <summary>
        /// 停止插件
        /// </summary>
        public override void Stop(BundleOptions option)
        {
            // Nothing to do
        }
        #endregion
    }
}

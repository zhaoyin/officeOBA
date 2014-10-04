using System;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Loader;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Service;
using Cooperativeness.OBA.Word.Tools;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Core
{
    /// <summary>
    /// 定义片段插件框架
    /// </summary>
    internal class BundleFragment : AbstractBundle
    {
        private static readonly Logger Log = new Logger(typeof(BundleFragment));
        #region 字段
        /** 附加宿主插件列表 */
	    protected BundleHost[] hosts;

        #endregion

        #region 构造函数
        public BundleFragment(IBundleData bundledata, Framework framework)
            : base(bundledata, framework)
        {
            hosts = null;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取加载器
        /// </summary>
        internal override IBundleLoader BundleLoader
        {
            get { return null; }
        }

        /// <summary>
        /// 获取插件上下文
        /// </summary>
        internal override BundleContextImpl BundleContext
        {
            get { return null; }
        }

        /// <summary>
        /// 获取一个值，该值用来指示当前插件是否是片段插件
        /// </summary>
        internal override bool IsFragment
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// 获取当前插件的宿主插件集合
        /// </summary>
        internal override BundleHost[] Hosts
        {
            get
            {
                return this.hosts;
            }
        }


        #endregion

        #region 方法
        /// <summary>
        /// 加载插件
        /// </summary>
        internal override void Load()
        {
            // nothing to do
        }

        /// <summary>
        /// 根据用户指定的类型名加载类型
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public override Type LoadClass(string className)
        {
            throw new TypeLoadException();
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="loadMode"></param>
        /// <returns></returns>
        public override object LoadResource(string resourceName, ResourceLoadMode loadMode)
        {
            return null;
        }

        /// <summary>
        /// 启动插件工作者
        /// </summary>
        /// <param name="option"></param>
        protected override void StartWorker(BundleOptions option)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 停止插件工作者
        /// </summary>
        /// <param name="options"></param>
        protected override void StopWorker(BundleOptions option)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取插件注册的所有服务引用列表
        /// </summary>
        /// <returns></returns>
        public override IServiceReference[] GetRegisteredServices()
        {
            return null;
        }

        /// <summary>
        /// 获取插件正在使用的服务的服务引用列表
        /// </summary>
        /// <returns></returns>
        public override IServiceReference[] GetServicesInUse()
        {
            return null;
        }

        /// <summary>
        /// 添加宿主插件
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        internal bool AddHost(BundleHost host)
        {
            if (host == null)
                return false;
            try
            {
                host.AttachFragment(this);
            }
            catch (BundleException be)
            {
                Log.Debug(be);
                //framework.publishFrameworkEvent(FrameworkEvent.ERROR, host, be);
                return false;
            }
            lock (this)
            {
                if (hosts == null)
                {
                    hosts = new BundleHost[] { host };
                    return true;
                }
                for (int i = 0; i < hosts.Length; i++)
                {
                    if (host == hosts[i])
                        return true; // already a host
                }
                BundleHost[] newHosts = new BundleHost[hosts.Length + 1];
                hosts.CopyTo(newHosts, 0);
                newHosts[newHosts.Length - 1] = host;
                hosts = newHosts;
            }
            return true;
        }

        #endregion
    }
}

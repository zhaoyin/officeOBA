using System;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Core;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Loader
{
    /// <summary>
    /// 定义插件加载器代理对象
    /// </summary>
    internal class BundleLoaderProxy
    {
        #region 字段
        private AbstractBundle bundle;
        private Framework framework;
        private IBundleLoader bundleLoader;

        #endregion

        #region 构造函数
        public BundleLoaderProxy(AbstractBundle bundle)
        {
            this.bundle = bundle;
            this.framework = bundle.Framework;
        }
        
        #endregion

        #region 属性
        /// <summary>
        /// 获取插件加载器
        /// </summary>
        public IBundleLoader BundleLoader
        {
            get { return CreateBundleLoader(); }
        }

        #endregion

        #region 方法
        /// <summary>
        /// 创建插件加载器
        /// </summary>
        internal IBundleLoader CreateBundleLoader()
        {
            if (bundleLoader != null)
                return bundleLoader;
            bundleLoader = new BundleLoaderImpl(bundle);
            return bundleLoader;
        }

        /// <summary>
        /// 根据类名获取对应的类型
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public Type LoadClass(string className)
        {
            if (this.bundleLoader != null)
                return bundleLoader.LoadClass(className);
            return null;
        }

        /// <summary>
        /// 根据资源名称和加载模式加载指定的资源
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public object LoadResource(string resourceName, ResourceLoadMode mode)
        {
            if (this.bundleLoader != null)
                return bundleLoader.LoadResource(resourceName, mode);
            return null;
        }
        #endregion
    }
}

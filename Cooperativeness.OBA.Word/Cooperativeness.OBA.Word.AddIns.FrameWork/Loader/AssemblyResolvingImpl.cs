using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Core;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Loader
{
    /// <summary>
    /// 定义程序集解析器
    /// </summary>
    internal class AssemblyResolvingImpl : IAssemblyResolving
    {
        #region 字段
        private Framework framework;
        private IList<IAssemblyResolverHook> resolverHooks;

        #endregion

        #region 构造函数
        public AssemblyResolvingImpl(Framework framework)
        {
            this.framework = framework;
            resolverHooks = new List<IAssemblyResolverHook>();
        }

        #endregion

        #region 方法
        /// <summary>
        /// 程序集解析入口方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {
            Assembly loadedAssembly= this.LoadBundleAssembly(args.Name);
            if (loadedAssembly == null)
            {
                return LoadFromHooks(args.Name);
            }

            return loadedAssembly;
        }

        /// <summary>
        /// 启动程序集解析器
        /// </summary>
        public void Start()
        {
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
        }

        /// <summary>
        /// 停止程序集启动器
        /// </summary>
        public void Stop()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= AssemblyResolve;
        }

        /// <summary>
        /// 根据指定的程序集名，加载程序集插件
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public Assembly LoadBundleAssembly(string fullName)
        {
            // 获取已安装的插件列表
            IList bundles = framework.Bundles.GetBundles();
            if (bundles == null || bundles.Count == 0) 
                return null;
            // 拷贝用户数据区
            AbstractBundle[] installedBundles = new AbstractBundle[bundles.Count];
            bundles.CopyTo(installedBundles, 0);
            // 检索插件，并加载程序集
            foreach (AbstractBundle bundle in installedBundles)
            {
                if (bundle.IsFragment || !bundle.IsResolved) 
                    continue;
                IRuntimeService serivce = bundle.BundleLoader as IRuntimeService;
                Assembly assembly = serivce.LoadBundleAssembly(fullName);
                if (assembly != null) return assembly;
            }

            return null;
        }

        /// <summary>
        /// 从钩子集合中加载程序集
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        private Assembly LoadFromHooks(string fullName)
        {
            foreach (IAssemblyResolverHook hook in resolverHooks)
            {
                Assembly findedAssembly = null;
                try
                {
                    findedAssembly = hook.Find(fullName);
                    if (findedAssembly != null) return findedAssembly;
                }
                catch { }
            }
            return null;
        }

        /// <summary>
        /// 注册解析程序集的钩子
        /// </summary>
        /// <param name="hook"></param>
        public void RegisterAssemblyResolverHook(IAssemblyResolverHook hook)
        {
            if (hook != null && !this.resolverHooks.Contains(hook))
            {
                this.resolverHooks.Add(hook);
            }
        }

        #endregion
    }
}

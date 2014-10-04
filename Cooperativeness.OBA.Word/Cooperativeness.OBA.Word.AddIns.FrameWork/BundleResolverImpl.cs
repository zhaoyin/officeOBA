using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFIDA.U8.IN.AddIns.Core;

namespace UFIDA.U8.IN.AddIns.Resolver
{
    /// <summary>
    /// 定义插件解析器
    /// </summary>
    internal class BundleResolverImpl : IBundleResolver
    {
        #region 字段
        private Framework framework;

        #endregion

        #region 构造函数
        public BundleResolverImpl(Framework framework)
        {
            this.framework = framework;
        }
        #endregion

        #region 方法
        /// <summary>
        /// 插件数据完整性解析
        /// </summary>
        /// <param name="bundles"></param>
        /// <returns></returns>
        public bool ResolveForValidation(IList<AbstractBundle> bundles)
        {
            // 检查数据不完整的插件
            IList<AbstractBundle> removalPendings = new List<AbstractBundle>();
            foreach (AbstractBundle bundle in bundles)
            {
                // 基础性数据完整性解析
                if (!BasicResolveForValidation(bundle))
                {
                    bundle.ResolveState = ResolveState.Resolved | ResolveState.Fault;
                    removalPendings.Add(bundle);
                    continue;
                }
                // 宿主数据完整性解析
                if (bundle.BundleData.BundleType == BundleType.Host
                    && !ResolveHostForValidation(bundle))
                {
                    bundle.ResolveState = ResolveState.Resolved | ResolveState.Fault;
                    removalPendings.Add(bundle);
                }
                else if (!ResolveFragmentForValidation(bundle))
                {
                    bundle.ResolveState = ResolveState.Resolved | ResolveState.Fault;
                    removalPendings.Add(bundle);
                }
            }
            // 将解析失败的插件从当前待解析插件列表中移除
            BundleUtil.Remove<AbstractBundle>(bundles, removalPendings);

            return bundles.Count != 0;
        }

        /// <summary>
        /// 基础性数据完整性解析
        /// </summary>
        /// <param name="bundle"></param>
        /// <returns></returns>
        private bool BasicResolveForValidation(AbstractBundle bundle)
        {
            BaseData bundleData = bundle.BundleData as BaseData;
            // 插件唯一标示名不能为空
            if (string.IsNullOrEmpty(bundleData.SymbolicName))
                return false;
            // 版本号校验
            if (bundleData.Version == null)
                bundleData.Version = new Version("1.0.0.0");

            return true;

        }

        /// <summary>
        /// 检查宿主插件数据完整性
        /// </summary>
        /// <param name="bundle"></param>
        /// <returns></returns>
        private bool ResolveHostForValidation(AbstractBundle bundle)
        {
            BaseData bundleData = bundle.BundleData as BaseData;
            // 检查是否存在激活器
            if (string.IsNullOrEmpty(bundleData.Activator))
                return false;

            return true;
        }

        /// <summary>
        /// 检查片段插件数据完整性
        /// </summary>
        /// <param name="bundle"></param>
        /// <returns></returns>
        private bool ResolveFragmentForValidation(AbstractBundle bundle)
        {
            BaseData bundleData = bundle.BundleData as BaseData;
            // 检查宿主
            if (string.IsNullOrEmpty(bundleData.Manifest.HostBundleSymbolicName))
                return false;
            // 检查宿主版本
            if (string.IsNullOrEmpty(bundleData.Manifest.HostBundleVersion))
                return false;

            return true;
        }

        /// <summary>
        /// 解析片段插件并将所有解析正确的片段插件附加到其宿主插件中
        /// </summary>
        /// <param name="bundles"></param>
        public void ResolveForAttachFragment(IList<AbstractBundle> bundles)
        {
            // 检查数据不完整的插件
            IList<AbstractBundle> removalPendings = new List<AbstractBundle>();
            foreach (AbstractBundle bundle in bundles)
            {
                // 检查当前插件是否为宿主插件，如果是宿主插件直接跳过
                if (!bundle.IsFragment) continue;
                // 获取当前片段插件依赖的宿主插件的唯一标识名和版本号
                string hostSymbolicName = bundle.BundleData.HostBundleSymbolicName;
                Version version = bundle.BundleData.HostBundleVersion;
                // 获取宿主插件
                AbstractBundle hostBundle = framework.Bundles.GetBundle(hostSymbolicName, version);
                // 验证宿主插件,以下情况标识片段插件无法附加并解析失败
                // 1. 未找到宿主插件，无法附加，解析失败
                // 2. 找到插件是片段插件，无法附加，解析失败
                // 3. 找到的插件，在数据有效性解析过程中，解析失败，无法附加
                if (hostBundle == null || hostBundle.IsFragment
                    || hostBundle.ResolveState == (ResolveState.Resolved | ResolveState.Fault))
                {
                    bundle.ResolveState = ResolveState.Resolved | ResolveState.Fault;
                    removalPendings.Add(bundle);
                }
                else
                {
                    // 将片段插件附加到宿主插件中
                    ((BundleFragment)bundle).AddHost(hostBundle as BundleHost);
                    removalPendings.Add(bundle);
                }
            }
            // 将片段插件从当前待解析插件列表中移除
            BundleUtil.Remove<AbstractBundle>(bundles, removalPendings);
        }

        /// <summary>
        /// 依赖解析处理
        /// </summary>
        /// <param name="bundles"></param>
        /// <returns></returns>
        public bool ResolveForDependency(IList<AbstractBundle> bundles)
        {
            // 带解析插件列表
            AbstractBundle[] bundlesForResolving = bundles.ToArray();
            // 按启动级别和标示进行排序
            BundleUtil.Sort(bundlesForResolving, 0, bundlesForResolving.Length);
            // 已解析插件列表
            IList<AbstractBundle> resolvedBundles = new List<AbstractBundle>();
            // 具有依赖关系的列表
            IList<AbstractBundle> dependencyBundles = new List<AbstractBundle>();
            // 分离处理
            foreach (var bundle in bundlesForResolving)
            {
                if (bundle.BundleData.DependentBundles == null
                    || bundle.BundleData.DependentBundles.Length == 0)
                    resolvedBundles.Add(bundle);
                else
                    dependencyBundles.Add(bundle);
            }
            // 解析依赖关系
            int index = 0;
            while (dependencyBundles.Count > 0)
            {
                IList<AbstractBundle> queue = new List<AbstractBundle>();
                AbstractBundle dependencyBundle = dependencyBundles.ElementAt(index);
                queue.Add(dependencyBundle);
                
            }

            return false;
        }

        /// <summary>
        /// 解析依赖关系
        /// </summary>
        /// <param name="bundle"></param>
        /// <returns></returns>
        internal bool ResolveDependeny(DependentBundle dependency)
        {
            return false;
        }

        #endregion
    }
}

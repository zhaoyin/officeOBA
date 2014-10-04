using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Core;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Package;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Resolver
{
    /// <summary>
    /// 定义解析器实现类
    /// </summary>
    internal class ResolverImpl : BaseResolver
    {
        #region 构造函数
        public ResolverImpl(Framework framework, PackageAdminImpl packageAdmin)
            : base(framework, packageAdmin)
        {
        }
        #endregion

        #region 方法
        /// <summary>
        /// 解析插件的元数据，检查是否符合规范和标准
        /// </summary>
        /// <param name="bundles"></param>
        /// <returns></returns>
        protected override bool ResolveMetaData(IList<Core.AbstractBundle> bundles)
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
                if (bundle.BundleData.BundleType == BundleType.Host)
                {
                    if (!ResolveHostForValidation(bundle))
                    {
                        bundle.ResolveState = ResolveState.Resolved | ResolveState.Fault;
                        removalPendings.Add(bundle);
                    }
                }
                else 
                {
                    if (!ResolveFragmentForValidation(bundle))
                    {
                        bundle.ResolveState = ResolveState.Resolved | ResolveState.Fault;
                        removalPendings.Add(bundle);
                    }
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
        /// 宿主、片段插件关系解析
        /// </summary>
        /// <param name="bundles"></param>
        protected override void ResolveRelation(IList<AbstractBundle> bundles)
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
        /// 依赖关系解析
        /// </summary>
        /// <param name="bundles"></param>
        /// <returns></returns>
        protected override bool ResolveDependent(IList<AbstractBundle> bundles)
        {
            // 构造解析树
            ResolverTree tree = ResolverTree.Parse(framework, this, bundles);
            // 如果构造失败，则清空集合，解析失败
            if (tree == null)
            {
                bundles.Clear();
                return false;
            }
            // 获取有序的已解析的插件类表
            IList<AbstractBundle> newBundles = tree.QueryBundlesInOrder();
            if (newBundles == null || newBundles.Count == 0)
            {
                bundles.Clear();
                return false;
            }
            // 清空当前集合列表
            bundles.Clear();
            BundleUtil.Combine(bundles, newBundles);

            return true;
        }
        #endregion
    }
}

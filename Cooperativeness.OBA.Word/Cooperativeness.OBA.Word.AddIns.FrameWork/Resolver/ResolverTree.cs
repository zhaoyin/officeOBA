using System;
using System.Collections;
using System.Collections.Generic;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Core;
using Cooperativeness.OBA.Word.Tools;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Resolver
{
    /// <summary>
    /// 定义解析树对象
    /// </summary>
    internal class ResolverTree
    {
        private static readonly Logger Log = new Logger(typeof(ResolverTree));
        #region 字段
        private Hashtable trackCache;
        private ResolverNodeCollection resolvers;
        private Framework framework;
        private ResolverImpl resolver;
        private Hashtable leafResolverBundles;

        #endregion

        #region 构造函数
        private ResolverTree(Framework framework, ResolverImpl resolver)
        {
            resolvers = new ResolverNodeCollection();
            trackCache = new Hashtable();
            leafResolverBundles = new Hashtable();
            this.resolver = resolver;
            this.framework = framework;
        }

        #endregion

        #region 方法
        /// <summary>
        /// 根据插件列表创建解析管理器
        /// </summary>
        /// <param name="bundle"></param>
        /// <returns></returns>
        public static ResolverTree Parse(Framework framework, ResolverImpl resolver, IList<AbstractBundle> bundles)
        {
            Hashtable cache = new Hashtable();
            ResolverTree admin = new ResolverTree(framework, resolver);
            foreach (AbstractBundle bundle in bundles)
            {
                if (!cache.Contains(bundle.ToString()))
                {
                    ResolverNode resolverBundle = admin.CreateResolverBundle(bundle);
                    if (resolverBundle != null)
                    {
                        //resolver.ResolveBundleStartPolicy(resolverBundle, ActivatorPolicy.Lazy);
                        admin.resolvers.Add(resolverBundle);
                        cache.Add(bundle.ToString(), bundle);
                    }
                }
            }
            return admin;
        }

        /// <summary>
        /// 创建解析插件
        /// </summary>
        /// <param name="bundle"></param>
        public ResolverNode CreateResolverBundle(AbstractBundle bundle)
        {
            try
            {
                // 建立跟踪器
                Tracker tracker = new Tracker();
                // 建立依赖关系
                ResolverNode resolverBundle = new ResolverNode(this, bundle, null);
                // 内部调用解析插件构造器
                InternalCreateResolverBundle(tracker, resolverBundle, bundle);

                // 检查当前插件是否已解析,并且解析失败
                if (bundle.ResolveState != (ResolveState.Resolved | ResolveState.Success))
                    return null;

                return resolverBundle;
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
                // 记录错误
            }
            return null;
        }

        /// <summary>
        /// 内部创建解析插件
        /// </summary>
        /// <param name="bundle"></param>
        private void InternalCreateResolverBundle(Tracker tracker, ResolverNode depencyProviderBy, AbstractBundle bundle)
        {
            // 检查当前插件是否已解析,并且解析失败
            if (bundle.ResolveState == (ResolveState.Resolved | ResolveState.Fault))
                return;
            // 检查缓存中是否存在,如果存在则说明出现了循环依赖
            if (tracker.Contains(depencyProviderBy))
            {
                LinkedListNode<ResolverNode> dependencyNode = tracker.Find(depencyProviderBy);
                if (dependencyNode.Next != null) return;
            }
            // 将当前解析插件放入追踪链中
            tracker.AddLast(depencyProviderBy);
            // 获取所有依赖
            DependentBundle[] dependencies = bundle.BundleData.DependentBundles;
            bool isResolved = false;
            if (dependencies != null)
            {
                foreach (var dependency in dependencies)
                {
                    // 检查依赖插件唯一标示名是否为空，为空则视依赖项无效
                    if (string.IsNullOrEmpty(dependency.BundleSymbolicName))
                        continue;
                    // 获取插件
                    AbstractBundle dependencyBundle = framework.Bundles.GetBundle(dependency.BundleSymbolicName, dependency.BundleVersion);
                    if ((dependencyBundle == null || dependencyBundle.IsFragment)
                        && dependency.Resolution == ResolutionMode.Mandatory)
                    {
                        // 设置解析状态
                        bundle.ResolveState = ResolveState.Resolved | ResolveState.Fault;
                        // 将当前解析插件从追踪器中移除
                        tracker.Remove(depencyProviderBy);
                        return;
                    }
                    // 构建解析插件对象
                    string key = dependencyBundle.ToString();
                    bool exist = trackCache.ContainsKey(key);
                    ResolverNode resolverBundle = !exist ? new ResolverNode(this, dependencyBundle, depencyProviderBy)
                        : trackCache[key] as ResolverNode;
                    if (exist) resolverBundle.DepencyProvidersBy.Add(depencyProviderBy);
                    // 嵌套搜索解析
                    if (!exist) InternalCreateResolverBundle(tracker, resolverBundle, dependencyBundle);
                    // 检查是否解析成功
                    isResolved = resolverBundle.Bundle.ResolveState == (ResolveState.Resolved | ResolveState.Success);
                    if (!isResolved && dependency.Resolution == ResolutionMode.Mandatory)
                    {
                        // 设置解析状态
                        bundle.ResolveState = ResolveState.Resolved | ResolveState.Fault;
                        // 将当前解析插件从追踪器中移除
                        tracker.Remove(depencyProviderBy);
                        return;
                    }
                    // 加入到依赖项中
                    depencyProviderBy.Dependencies.Add(resolverBundle);
                    // 调整逆向层级关系
                    int desLevel = resolverBundle.DesLevel + 1;
                    if (desLevel > depencyProviderBy.DesLevel) depencyProviderBy.DesLevel = desLevel;
                }
            }
            // 未解析状态下，解析当前插件，并设置解析状态
            if (bundle.ResolveState == ResolveState.Unresolved)
            {
                if (!resolver.ResolvePackage(bundle)) bundle.ResolveState = ResolveState.Resolved | ResolveState.Fault;
                else
                {
                    bundle.ResolveState = ResolveState.Resolved | ResolveState.Success;
                    // 加入缓存
                    string key = bundle.ToString();
                    if (!trackCache.ContainsKey(key)) trackCache.Add(key, depencyProviderBy);
                    // 记录叶子节点
                    key = depencyProviderBy.Bundle.ToString();
                    if (leafResolverBundles.ContainsKey(key))
                    {
                        if (depencyProviderBy.Dependencies.Count > 0)
                        {
                            leafResolverBundles.Remove(key);
                        }
                    }
                    else if (depencyProviderBy.Dependencies.Count == 0)
                    {
                        leafResolverBundles.Add(key, depencyProviderBy);
                    }
                }
            }
            // 将当前解析插件从追踪器中移除
            tracker.Remove(depencyProviderBy);
        }


        /// <summary>
        /// 获取有序的依赖插件序列
        /// </summary>
        /// <returns></returns>
        public IList<AbstractBundle> QueryBundlesInOrder()
        {
            if (resolvers.Count == 0) return null;
            // 创建插件列表
            IList<AbstractBundle> bundles = new List<AbstractBundle>();
            IList<ResolverNode> lResolvers = new List<ResolverNode>();
            // 获取当前所有解析的跟节点
            ResolverNode[] resolverBundles = new ResolverNode[leafResolverBundles.Count];
            leafResolverBundles.Values.CopyTo(resolverBundles, 0);
            BundleUtil.Sort(resolverBundles, 0, resolverBundles.Length);
            // 放入有序队列中
            Combine(bundles, resolverBundles);
            // 解析下一个层次
            ScanBundlesByOrder(bundles, resolverBundles);

            return  bundles;
        }

        /// <summary>
        /// 扫描解析插件的层次，最终获取一个有序的列表
        /// </summary>
        /// <param name="bundles"></param>
        /// <param name="resolvers"></param>
        public void ScanBundlesByOrder(IList<AbstractBundle> bundles, IList<ResolverNode> resolvers)
        {
            if (resolvers.Count == 0) return;
            // 定义层次列表
            ResolverNodeCollection providers = new ResolverNodeCollection();
            // 扫描上一级层次
            foreach (var resolver in resolvers)
            {
                if (resolver.DepencyProvidersBy.Count == 0)
                    continue;
                // 获取当前解析插件的所有依赖插件，并进行降序排列
                ResolverNode[] resolverBundles = resolver.DepencyProvidersBy.ToArray();
                foreach (var provider in resolverBundles)
                {
                    if (provider.DesLevel == resolver.DesLevel + 1)
                    {
                        providers.Add(provider);
                    }
                }
            }
            // 调整扫描的层次链表
            ResolverNode[] newResolvers = providers.ToArray();
            BundleUtil.Sort(newResolvers, 0, newResolvers.Length);
            // 合并当前层次
            Combine(bundles, newResolvers);
            // 扫描下一个子层次结构
            this.ScanBundlesByOrder(bundles, newResolvers);
        }

        /// <summary>
        /// 集合合并操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list1"></param>
        /// <param name="list1"></param>
        /// <returns></returns>
        private void Combine(IList<AbstractBundle> source, IList<ResolverNode> target)
        {
            foreach (var item in target)
            {
                if (!source.Contains(item.Bundle))
                    source.Add(item.Bundle);
            }
        }

        #endregion

        #region 内部类
        class Tracker : LinkedList<ResolverNode>
        {
        }
        #endregion
    }
}

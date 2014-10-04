using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Core;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Events;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Resolver;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Package
{
    /// <summary>
    /// 定义包管理器
    /// </summary>
    internal class PackageAdminImpl : IPackageAdmin
    {
        #region 字段
        protected Framework framework;
        protected IResolver resolver;

        #endregion

        #region 构造函数
        public PackageAdminImpl(Framework framework)
        {
            this.framework = framework;
        }

        #endregion

        #region 方法
        /// <summary>
        /// 解析插件
        /// </summary>
        /// <param name="bundles"></param>
        /// <returns></returns>
        public bool ResolveBundles(IBundle[] bundles)
        {
            if (resolver == null)
                resolver = new ResolverImpl(framework, this);
            // 组织链表
            IList<AbstractBundle> resolvingBundles = new List<AbstractBundle>();
            foreach (AbstractBundle bundle in bundles)
                resolvingBundles.Add(bundle);

            bool isResolved = resolver.Resolve(resolvingBundles);
            // 重新处理集合
            if (isResolved)
            {
                int resolvedCount = resolvingBundles.Count;
                for (int i = 0; i < bundles.Length; i++)
                {
                    if (i > resolvedCount - 1)
                        bundles[i] = null;
                    else
                    {
                        AbstractBundle resolvedBundle = resolvingBundles[i];
                        if (resolvedBundle.State == BundleState.Installed)
                            resolvedBundle.Resolve();
                        bundles[i] = resolvedBundle;
                    }
                }
            }

            return isResolved;

        }

        /// <summary>
        /// 解析插件扩展
        /// </summary>
        /// <param name="bundle"></param>
        public void ResolveExtension(AbstractBundle bundle)
        {
            #region 插件扩展点处理
            // 处理插件的扩展点
            IExtensionPoint[] points = bundle.BundleData.ExtensionPoints;
            if (points != null && points.Length > 0)
            {
                foreach (ExtensionPointImpl point in points)
                {
                    // 发布扩展点变化事件
                    ExtensionPointEventArgs e = new ExtensionPointEventArgs(point, CollectionChangedAction.Add);
                    if (point.Available) framework.PublishExtensionPointEvent(bundle, e);
                    // 如果当前事件没有取消则进行添加扩展点处理
                    if(!e.Cancel) framework.ExtensionAdmin.AddExtensionPoint(point);
                    
                }
            }
            #endregion

            #region 插件扩展处理
            // 处理插件扩展
            IExtension[] extensions = bundle.BundleData.Extensions;
            if (extensions != null && extensions.Length > 0)
            {
                foreach (ExtensionImpl extension in extensions)
                {
                    ExtensionEventArgs e = new ExtensionEventArgs(extension, CollectionChangedAction.Add);
                    framework.PublishExtensionEvent(bundle, e);
                    if (!e.Cancel) framework.ExtensionAdmin.AddExtension(extension);
                }
            }
            #endregion

            #region 片段插件解析
            // 处理片段插件扩展
            if (!bundle.IsFragment && bundle.Fragments != null)
            {
                foreach (BundleFragment fragment in bundle.Fragments)
                {
                    // 处理片段插件扩展点
                    points = bundle.BundleData.ExtensionPoints;
                    if (points != null && points.Length > 0)
                    {
                        foreach (ExtensionPointImpl point in points)
                        {
                            // 发布扩展点变化事件
                            ExtensionPointEventArgs e = new ExtensionPointEventArgs(point, CollectionChangedAction.Add);
                            if (point.Available) framework.PublishExtensionPointEvent(bundle, e);
                            // 如果当前事件没有取消则进行添加扩展点处理
                            if (!e.Cancel)
                            {
                                point.Owner = bundle;
                                framework.ExtensionAdmin.AddExtensionPoint(point);
                            }
                        }
                    }
                    // 处理片段插件扩展
                    extensions = bundle.BundleData.Extensions;
                    if (extensions != null && extensions.Length > 0)
                    {
                        foreach (ExtensionImpl extension in extensions)
                        {
                            ExtensionEventArgs e = new ExtensionEventArgs(extension, CollectionChangedAction.Add);
                            framework.PublishExtensionEvent(bundle, e);
                            if (e.Cancel)
                            {
                                extension.Owner = bundle;
                                framework.ExtensionAdmin.AddExtension(extension);
                            }
                        }
                    }
                }
            }
            #endregion
        }

        /// <summary>
        /// 根据版本信息和唯一标示名获取插件列表
        /// </summary>
        /// <param name="symbolicName"></param>
        /// <param name="versionRange"></param>
        /// <returns></returns>
        public IBundle[] GetBundles(string symbolicName, string versionRange)
        {
            IBundle[] bundles = framework.Bundles.GetBundles(symbolicName);
            IList<IBundle> newBundles = new List<IBundle>();
            VersionRange range = new VersionRange(versionRange);
            foreach (var bundle in bundles)
            {
                if (range.IsIncluded(bundle.Version))
                    newBundles.Add(bundle);
            }

            return newBundles.Count > 0 ? newBundles.ToArray() : null;
        }

        /// <summary>
        /// 获取指定插件的片段插件列表
        /// </summary>
        /// <param name="bundle"></param>
        /// <returns></returns>
        public IBundle[] GetFragments(IBundle bundle)
        {
            if (bundle is BundleHost)
                return ((AbstractBundle)bundle).Fragments;
            return null;
        }

        /// <summary>
        /// 获取指定插件的宿主插件列表
        /// </summary>
        /// <param name="bundle"></param>
        /// <returns></returns>
        public IBundle[] GetHosts(IBundle bundle)
        {
            if(bundle is BundleFragment)
                return ((AbstractBundle)bundle).Hosts;
            return null;
        }

        /// <summary>
        /// 根据类型获取插件对象
        /// </summary>
        /// <param name="clazz"></param>
        /// <returns></returns>
        public IBundle GetBundle(Type clazz)
        {
            // 获取当前URL的安装路径
            Uri url = new Uri(clazz.Assembly.CodeBase);
            string assemblyPath = url.LocalPath;
            // 获取所有已安装的插件
            IList installedBundles = framework.Bundles.GetBundles();
            AbstractBundle[] newBundles = new AbstractBundle[installedBundles.Count];
            installedBundles.CopyTo(newBundles, 0);
            // 搜索插件
            foreach (var bundle in newBundles)
            {
                if (bundle.BundleData.Assemblies == null
                    || bundle.BundleData.Assemblies.Length == 0)
                    continue;
                foreach (var assemblyEntry in bundle.BundleData.Assemblies)
                {
                    string path = Path.Combine(bundle.Location, assemblyEntry.Path);
                    if (path.EqualsIgnoreCase(assemblyPath))
                        return bundle;
                }
            }

            return null;

        }

        /// <summary>
        /// 获取插件的类型
        /// </summary>
        /// <param name="bundle"></param>
        /// <returns></returns>
        public BundleType GetBundleType(IBundle bundle)
        {
            return ((AbstractBundle)bundle).BundleData.BundleType;
        }
        #endregion
    }
}

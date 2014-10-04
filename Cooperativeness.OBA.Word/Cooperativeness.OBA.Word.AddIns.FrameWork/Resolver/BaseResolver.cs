using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Core;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Package;
using Cooperativeness.OBA.Word.Tools;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Resolver
{
    /// <summary>
    /// 定义基础抽象解析器对象
    /// </summary>
    internal abstract class BaseResolver : IResolver
    {
        private static readonly Logger Log = new Logger(typeof(BaseResolver));
        #region 字段
        protected Framework framework;
        protected PackageAdminImpl packageAdmin;

        #endregion

        #region 构造函数
        public BaseResolver(Framework framework, PackageAdminImpl packageAdmin)
        {
            this.framework = framework;
            this.packageAdmin = packageAdmin;
        }

        #endregion

        #region 方法
        /// <summary>
        /// 插件解析操作
        /// </summary>
        /// <param name="bundles"></param>
        /// <returns></returns>
        public bool Resolve(IList<AbstractBundle> bundles)
        {
            // 进行元数据解析
            bool success = ResolveMetaData(bundles);
            if (!success) return false;
            // 宿主、片段插件关系解析
            ResolveRelation(bundles);
            // 依赖解析
            success = ResolveDependent(bundles);


            return success;
        }

        /// <summary>
        /// 元数据解析操作，验证插件的元数据是否正确
        /// </summary>
        /// <param name="bundles"></param>
        /// <returns></returns>
        protected abstract bool ResolveMetaData(IList<AbstractBundle> bundles);

        /// <summary>
        /// 宿主、片段关系解析
        /// </summary>
        /// <param name="bundles"></param>
        protected abstract void ResolveRelation(IList<AbstractBundle> bundles);


        /// <summary>
        /// 依赖关系解析
        /// </summary>
        /// <param name="bundles"></param>
        /// <returns></returns>
        protected abstract bool ResolveDependent(IList<AbstractBundle> bundles);

        /// <summary>
        /// 解析插件包
        /// </summary>
        /// <param name="bundle"></param>
        internal virtual bool ResolvePackage(AbstractBundle bundle)
        {
            try
            {
                // 检测插件规格是否存在
                if (bundle.BundleSpecification == null)
                    bundle.BundleSpecification = new BundleSpecificationImpl(bundle);
                // 解析导出包和私有包
                ResolveExportedPackage(bundle);
                // 解析导入包
                ResolveImportPackage(bundle);
                // 解析服务包
                ResolveServicePackage(bundle);

                return true;
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
            }
            return false;
        }

        /// <summary>
        /// 解析导出包
        /// </summary>
        /// <param name="bundles"></param>
        /// <returns></returns>
        protected virtual void ResolveExportedPackage(AbstractBundle bundle)
        {
            // 解析导出包和私有包
            AssemblyEntry[] asmEntries = bundle.BundleData.Assemblies;
            if (asmEntries != null && asmEntries.Count() > 0)
            {
                foreach (var entry in asmEntries)
                {
                    if (entry.Share)
                    {
                        ExportedPackageImpl package = ExportedPackageImpl.Create(bundle, entry);
                        if(package!=null) bundle.BundleSpecification.AddExportedPackage(package);
                    }
                    else
                    {
                        PrivilegedPackage privilegedPackage = PrivilegedPackage.Create(bundle, entry);
                        if (privilegedPackage != null) bundle.BundleSpecification.AddPrivilegedPackage(privilegedPackage);
                    }
                }
            }
            // 扫描片段插件
            if (!bundle.IsFragment && bundle.Fragments != null && bundle.Fragments.Length > 0)
            {
                foreach (AbstractBundle fragment in bundle.Fragments)
                {
                    ResolveExportedPackage(fragment);
                }
            }
        }

        /// <summary>
        /// 解析导出包
        /// </summary>
        /// <param name="bundles"></param>
        /// <returns></returns>
        protected virtual void ResolveImportPackage(AbstractBundle bundle)
        {
            // 解析依赖包
            DependentBundle[] dependencyBundles = bundle.BundleData.DependentBundles;
            if (dependencyBundles != null && dependencyBundles.Count() > 0)
            {
                foreach (var dependency in dependencyBundles)
                {
                    // 检查导入包包名，如果为空则直接跳过
                    string symbolicName = dependency.BundleSymbolicName;
                    if (string.IsNullOrEmpty(symbolicName))
                        continue;
                    Version version = dependency.BundleVersion == null ? new Version("1.0.0.0") : dependency.BundleVersion;
                    // 获取插件对象
                    AbstractBundle dependencyBundle = framework.getBundleBySymbolicName(symbolicName, version);
                    // 如果为空该导入包无效
                    if ((dependencyBundle == null || dependencyBundle.IsFragment)
                        && dependency.Resolution == ResolutionMode.Mandatory)
                    {
                        throw new BundleException(string.Format("[{0}]Not find dependency bundle({1}_{2}).", bundle.ToString(), symbolicName, version.ToString()), BundleExceptionType.RESOLVE_ERROR);
                    }
                    // 如果未解析或解析失败则导入包无效
                    if (dependencyBundle.ResolveState == ResolveState.Unresolved
                        || (dependencyBundle.ResolveState == (ResolveState.Resolved | ResolveState.Fault)))
                    {
                        if (dependency.Resolution == ResolutionMode.Mandatory)
                            throw new BundleException(string.Format("[{0}]Dependency bundle({1}_{2}) failture resolved.", bundle.ToString(), symbolicName, version.ToString())
                                , BundleExceptionType.RESOLVE_ERROR);
                        else continue;
                    }
                    // 检查是组件依赖还是插件依赖
                    if (string.IsNullOrEmpty(dependency.AssemblyName))
                    {
                        ImportBundleImpl importBundle = new ImportBundleImpl(dependencyBundle,dependency.Resolution);
                        bundle.BundleSpecification.AddImportBundle(importBundle);
                    }
                    else
                    {
                        // 获取依赖组件的相关参数
                        string asmName = dependency.AssemblyName;
                        Version asmVersion = dependency.AssemblyVersion;
                        string culture = dependency.Culture;
                        string publicKeyToken = dependency.PublicKeyToken;
                        // 定义导出包
                        IExportedPackage exportedPackage = null;
                        // 获取导出包
                        if (asmVersion == null)
                        {
                            IExportedPackage[] exportedPackages = dependencyBundle.BundleSpecification.GetExportedPackage(asmName);
                            if (exportedPackages == null || exportedPackages.Length == 0)
                            {
                                if (dependency.Resolution == ResolutionMode.Mandatory)
                                    throw new BundleException(string.Format("[{0}]Not find exported package in dependency bundle({1}_{2}) failture resolved.", bundle.ToString(), symbolicName, version.ToString())
                                        , BundleExceptionType.RESOLVE_ERROR);
                                else continue;
                            }
                            exportedPackage = exportedPackages[0];
                        }
                        else if (!string.IsNullOrEmpty(culture) && !string.IsNullOrEmpty(publicKeyToken))
                        {
                            exportedPackage = dependencyBundle.BundleSpecification.GetExportedPackage(asmName, version, culture, publicKeyToken);
                            if (exportedPackage == null)
                            {
                                if (dependency.Resolution == ResolutionMode.Mandatory)
                                    throw new BundleException(string.Format("[{0}]Not find exported package in dependency bundle({1}_{2}) failture resolved.", bundle.ToString(), symbolicName, version.ToString())
                                        , BundleExceptionType.RESOLVE_ERROR);
                                else continue;
                            }
                        }
                        // 找到对应导入包,可以建议导入包到导出包的连线
                        ImportPackageImpl importPackage = new ImportPackageImpl(bundle, dependencyBundle, dependency);
                        bundle.BundleSpecification.AddImportPackage(importPackage);
                    }

                }
            }
            // 扫描片段插件
            if (!bundle.IsFragment && bundle.Fragments != null && bundle.Fragments.Length > 0)
            {
                foreach (AbstractBundle fragment in bundle.Fragments)
                    ResolveImportPackage(fragment);
            }
        }

        /// <summary>
        /// 解析服务包
        /// </summary>
        /// <param name="bundles"></param>
        /// <returns></returns>
        protected virtual void ResolveServicePackage(AbstractBundle bundle)
        {
            // 解析依赖包
            ServiceEntry[] serviceEntries = bundle.BundleData.Services;
            if (serviceEntries != null && serviceEntries.Count() > 0)
            {
                foreach (var serviceEntry in serviceEntries)
                {
                    if (string.IsNullOrEmpty(serviceEntry.Type))
                        continue;
                    if (string.IsNullOrEmpty(serviceEntry.Service))
                        continue;
                    ServicePackageImpl package = new ServicePackageImpl(bundle, serviceEntry);
                    bundle.BundleSpecification.AddServicePackage(package);
                }
            }
            // 扫描片段插件
            if (!bundle.IsFragment && bundle.Fragments != null && bundle.Fragments.Length > 0)
            {
                foreach (AbstractBundle fragment in bundle.Fragments)
                    ResolveServicePackage(fragment);
            }

        }

        /// <summary>
        /// 解析插件启动策略，如果启动插件本身是立即启动，则会导致其依赖项也会启动
        /// </summary>
        internal void ResolveBundleStartPolicy(ResolverNode resolverNode, ActivatorPolicy policy)
        {
            // 检查当前解析解点，并进行启动策略处理
            if (policy == ActivatorPolicy.Immediate 
                && resolverNode.Bundle.ActivatorPolicy == ActivatorPolicy.Lazy)
            {
                resolverNode.Bundle.ActivatorPolicy = policy;
            }
            // 变更当前上下文策略
            if (resolverNode.Bundle.ActivatorPolicy == ActivatorPolicy.Immediate)
                policy = ActivatorPolicy.Immediate;
            // 处理依赖解析插件解点启动策略
            foreach (ResolverNode dependency in resolverNode.Dependencies)
            {
                ResolveBundleStartPolicy(dependency, policy);
            }
        }
        #endregion
    }
}

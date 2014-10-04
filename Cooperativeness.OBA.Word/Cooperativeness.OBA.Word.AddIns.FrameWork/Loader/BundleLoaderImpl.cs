using System;
using System.IO;
using System.Reflection;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Core;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Package;
using Cooperativeness.OBA.Word.Tools;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Loader
{
    /// <summary>
    /// 定义插件加载器对象
    /// </summary>
    internal class BundleLoaderImpl : AbstractClassLoader, IBundleLoader, IClassLoader
    {
        private static readonly Logger Log = new Logger(typeof(BundleLoaderImpl));
        #region 字段
        private Framework framework;
        private AbstractBundle bundle;

        #endregion

        #region 构造函数
        public BundleLoaderImpl(AbstractBundle bundle)
        {
            this.bundle = bundle;
            this.framework = bundle.Framework;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取插件框架对象
        /// </summary>
        public Framework Framework
        {
            get { return this.framework; }
        }

        /// <summary>
        /// 获取当前加载器所属的插件对象
        /// </summary>
        public IBundle Bundle
        {
            get { return this.bundle; }
        }
        #endregion

        #region 方法

        #region 加载用户指定的类型
        /// <summary>
        /// 根据类名加载指定的类型
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public override Type LoadClass(string className)
        {
            // 尝试从缓存中加载
            Type type;
            if (TryGetCachedType(className, out type))
            {
                return type;
            }
            // 尝试加载系统类型
            if (className.StartsWith("System.") && TryGetSystemType(className, out type))
            {
                return type;
            }
            // 尝试从本地私有程序集加载类型
            type = LoadClassFromExportedPackages(className, bundle.BundleSpecification.GetPrivilegedPackages());
            if (type != null)
            {
                this.CacheLoadedType(className, type);
                return type;
            }
            // 尝试从导出包中加载类型
            type = LoadClassFromExportedPackages(className, bundle.BundleSpecification.GetExportedPackages());
            if (type != null)
            {
                this.CacheLoadedType(className, type);
                return type;
            }
            // 从片段程序集中加载
            type = LoadClassFromFragments(className);
            if (type != null)
            {
                this.CacheLoadedType(className, type);
                return type;
            }
            // 从依赖的插件中加载类型
            type = LoadClassFromDependencyBundles(this.bundle, className);
            if (type != null) return type;
            // 从倒入包中加载类型
            type = LoadClassFromImportPackages(this.bundle, className);
            if (type != null)
            {
                this.CacheLoadedType(className, type);
                return type;
            }

            return null;
        }

        /// <summary>
        /// 根据其他插件的导入包，加载指定的类型
        /// </summary>
        /// <param name="importPackage"></param>
        /// <returns></returns>
        internal Type LoadClassByImportPackage(string className, ImportPackageImpl importPackage)
        {
            try
            {
                // 尝试从缓存中加载
                Type type;
                if (TryGetCachedType(className, out type))
                {
                    return type;
                }
                // 查找当前插件的导出包
                IExportedPackage[] packages = this.bundle.BundleSpecification.GetExportedPackages(importPackage);
                type = LoadClassFromExportedPackages(className, packages);
                if (type != null) return type;
                // 从片段插件中查找导出包
                if (bundle.Fragments != null && bundle.Fragments.Length > 0)
                {
                    foreach (AbstractBundle fragment in bundle.Fragments)
                    {
                        packages = fragment.BundleSpecification.GetExportedPackages(importPackage);
                        type = LoadClassFromExportedPackages(className, packages);
                        if (type != null) return type;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
                // 记录日志
            }

            return null;
        }

        /// <summary>
        /// 从到处包中加载指定的类型
        /// </summary>
        /// <param name="className"></param>
        /// <param name="packages"></param>
        /// <returns></returns>
        private Type LoadClassFromExportedPackages(string className, IExportedPackage[] packages)
        {
            try
            {
                // 如果包围空，则直接返回空类型
                if (packages == null)
                    return null;
                // 开始搜索类型
                foreach (ExportedPackageImpl package in packages)
                {
                    // 尝试从缓存中加载
                    Assembly assembly = Assembly.Load(package.AssemblyName.FullName);
                    Type type = null;
                    if (assembly != null)
                    {
                        type = assembly.GetType(className);
                        if (type == null) continue;
                        else
                        {
                            this.CacheLoadedType(className, type);
                            return type;
                        }
                    }
                    // 尝试检测当前程序集中是否存在
                    type = package.ReflectOnlyPackageAssembly.GetType(className);
                    if (type == null) continue;
                    else
                    {
                        // 尝试加载程序集
                        assembly = Assembly.LoadFrom(package.ReflectOnlyPackageAssembly.Location);
                        type = assembly.GetType(className);
                        if (type != null) this.CacheLoadedType(className, type);
                        return type;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
            }

            return null;
        }

        /// <summary>
        /// 从片段程序集中加载
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        private Type LoadClassFromFragments(string className)
        {
            if (bundle.Fragments != null && bundle.Fragments.Length > 0)
            {
                Type type = null;
                foreach (var fragment in bundle.Fragments)
                {
                    // 尝试从本地私有程序集加载类型
                    type = LoadClassFromExportedPackages( className, fragment.BundleSpecification.GetPrivilegedPackages());
                    if (type != null) return type;
                    // 尝试从导出包中加载类型
                    type = LoadClassFromExportedPackages(className, fragment.BundleSpecification.GetExportedPackages());
                    if (type != null) return type;
                    // 尝试从依赖插件中加载
                    type = LoadClassFromDependencyBundles(fragment, className);
                    if (type != null) return type;
                    // 尝试从导入包中加载
                    type = LoadClassFromImportPackages(fragment, className);
                    if (type != null) return type;
                }
            }
            return null;
        }

        /// <summary>
        /// 从导入包中加载类型
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        private Type LoadClassFromImportPackages(AbstractBundle bundle, string className)
        {
            IImportPackage[] packages = bundle.BundleSpecification.GetImportPackages();
            if (packages == null) return null;
            foreach (ImportPackageImpl importPackage in packages)
            {
                // 获取倒入包依赖的插件对象
                AbstractBundle dependencyBundle = importPackage.Bundle as AbstractBundle;
                // 获取插件对象的加载器
                BundleLoaderImpl loader = (BundleLoaderImpl)dependencyBundle.BundleLoader;
                // 通过加载器加载类型
                Type type = loader.LoadClassByImportPackage(className, importPackage);
                if (type != null) return type;
            }
            return null;
        }

        /// <summary>
        /// 从依赖插件中加载指定的类型
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        private Type LoadClassFromDependencyBundles(AbstractBundle bundle, string className)
        {
            IImportBundle[] packages = bundle.BundleSpecification.GetImportBundles();
            if (packages == null) return null;
            foreach (IImportBundle importBundle in packages)
            {
                AbstractBundle referenceBundle = importBundle.Bundle as AbstractBundle;
                Type type = referenceBundle.LoadClass(className);
                if (type != null) return type;
            }
            return null;
        }

        #endregion

        #region 加载用户指定的资源
        /// <summary>
        /// 根据资源名称和加载模式加载指定的资源
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public override object LoadResource(string resourceName, ResourceLoadMode mode)
        {
            if (mode == ResourceLoadMode.Local)
                return LoadResourceFromLocal(this.bundle, resourceName);
            else if (mode == ResourceLoadMode.FragmentAndLocal)
                return LoadResourceFromFragmentLocal(resourceName);
            else
                return LoadResourceFromAssemblies(resourceName);
        }

        /// <summary>
        /// 从本地加载资源
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        private object LoadResourceFromLocal(AbstractBundle bundle, string resourceName)
        {
            try
            {
                string path = Path.Combine(bundle.Location, resourceName);
                FileStream stream = File.Open(path, FileMode.Open);
                return stream;
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
            }
            return null;
        }

        /// <summary>
        /// 从片段的本地加载资源
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        private object LoadResourceFromFragmentLocal(string resourceName)
        {
            if (bundle.Fragments != null && bundle.Fragments.Length > 0)
            {
                object obj = null;
                foreach (var fragment in bundle.Fragments)
                {
                    // 尝试从导入包中加载
                    obj = LoadResourceFromLocal(fragment, resourceName);
                    if (obj != null) return obj;
                    else continue;
                }
            }
            return null;
        }

        /// <summary>
        /// 从程序集中加载指定的资源
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        private object LoadResourceFromAssemblies(string resourceName)
        {
            // 尝试从缓存中加载
            object obj;
            if (TryGetCachedResource(resourceName, out obj))
            {
                return obj;
            }
            // 尝试从本地私有程序集加载
            Assembly assembly;
            obj = LoadResourceFromExportedPackage(resourceName, bundle.BundleSpecification.GetPrivilegedPackages(),out assembly);
            if (obj != null)
            {
                this.CacheLoadedResource(resourceName, assembly);
                return obj;
            }
            // 尝试从导出包中加载
            obj = LoadResourceFromExportedPackage(resourceName, bundle.BundleSpecification.GetExportedPackages(), out assembly);
            if (obj != null)
            {
                this.CacheLoadedResource(resourceName, assembly);
                return obj;
            }
            // 从片段程序集中加载
            obj = LoadResourceFromFragments(resourceName, out assembly);
            if (obj != null)
            {
                this.CacheLoadedResource(resourceName, assembly);
                return obj;
            }
            // 从依赖插件中加载
            obj = LoadResourceFromImportBundles(this.bundle, resourceName);
            if (obj != null) return obj;
            // 从导入包中加载
            obj = LoadResourceFromImportPackages(this.bundle, resourceName, out assembly);
            if (obj != null)
            {
                this.CacheLoadedResource(resourceName, assembly);
                return obj;
            }

            return null;
        }

        /// <summary>
        /// 从到处包中加载指定的资源
        /// </summary>
        /// <param name="className"></param>
        private object LoadResourceFromExportedPackage(string resourceName, IExportedPackage[] packages, out Assembly assembly)
        {
            assembly = null;
            try
            {
                if (packages == null) return null;
                foreach (ExportedPackageImpl package in packages)
                {
                    // 尝试从缓存中加载
                    assembly = Assembly.Load(package.AssemblyName.FullName);
                    object obj = null;
                    if (assembly != null)
                    {
                        obj = assembly.GetManifestResourceStream(resourceName);
                        if (obj != null) return obj;
                        else continue;
                    }
                    // 尝试检测当前程序集中是否存在
                    obj = package.ReflectOnlyPackageAssembly.GetManifestResourceStream(resourceName);
                    if (obj == null) continue;
                    else
                    {
                        // 尝试加载程序集
                        assembly = Assembly.LoadFrom(package.ReflectOnlyPackageAssembly.Location);
                        return assembly.GetManifestResourceStream(resourceName);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
            }

            return null;
        }

        /// <summary>
        /// 从片段程序集中加载资源
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        private object LoadResourceFromFragments(string resourceName, out Assembly assembly)
        {
            assembly = null;
            if (bundle.Fragments != null && bundle.Fragments.Length > 0)
            {
                object obj = null;
                foreach (var fragment in bundle.Fragments)
                {
                    // 尝试从本地私有程序集加载类型
                    obj = LoadResourceFromExportedPackage(resourceName, fragment.BundleSpecification.GetPrivilegedPackages(), out assembly);
                    if (obj != null) return obj;
                    // 尝试从导出包中加载类型
                    obj = LoadResourceFromExportedPackage(resourceName, fragment.BundleSpecification.GetExportedPackages(), out assembly);
                    if (obj != null) return obj;
                    // 尝试从依赖插件中加载
                    obj = LoadResourceFromImportBundles(fragment, resourceName);
                    if (obj != null) return obj;
                    // 尝试从导入包中加载
                    obj = LoadResourceFromImportPackages(fragment, resourceName, out assembly);
                    if (obj != null) return obj;
                    
                }
            }
            return null;
        }

        /// <summary>
        /// 从导入包中加载资源
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        private object LoadResourceFromImportPackages(AbstractBundle bundle, string resourceName,out Assembly assembly)
        {
            assembly = null;
            IImportPackage[] packages = bundle.BundleSpecification.GetImportPackages();
            if (packages == null) return null;
            foreach (ImportPackageImpl importPackage in packages)
            {
                AbstractBundle referenceBundle = importPackage.Bundle as AbstractBundle;
                IExportedPackage[] exportPackage = referenceBundle.BundleSpecification.GetExportedPackages(importPackage);
                foreach (ExportedPackageImpl package in packages)
                {
                    // 尝试从缓存中加载
                    assembly = Assembly.Load(package.AssemblyName.FullName);
                    object obj = null;
                    if (assembly != null)
                    {
                        obj = assembly.GetManifestResourceStream(resourceName);
                        if (obj != null) return obj;
                        else continue;
                    }
                    // 尝试检测当前程序集中是否存在
                    obj = package.ReflectOnlyPackageAssembly.GetManifestResourceStream(resourceName);
                    if (obj == null) continue;
                    else
                    {
                        // 尝试加载程序集
                        assembly = Assembly.LoadFrom(package.ReflectOnlyPackageAssembly.Location);
                        return assembly.GetManifestResourceStream(resourceName);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 从导入包中加载资源
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        private object LoadResourceFromImportBundles(AbstractBundle bundle, string resourceName)
        {
            IImportBundle[] packages = bundle.BundleSpecification.GetImportBundles();
            if (packages == null) return null;
            foreach (IImportBundle importBundle in packages)
            {
                AbstractBundle referenceBundle = importBundle.Bundle as AbstractBundle;
                object obj = referenceBundle.LoadResource(resourceName, ResourceLoadMode.ClassSpace);
                if (obj != null) return obj;
            }
            return null;
        }

        /// <summary>
        /// 根据用户指定的程序集名称获取程序集
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        protected override Assembly OnLoadBundleAssembly(string fullName)
        {
            try
            {
                // 从本地程序集加载
                Assembly assembly = LoadBundleAssemblyFromPackages(this.bundle, fullName);
                if (assembly != null)
                {
                    this.CacheLoadedAssembly(fullName, assembly);
                    return assembly;
                }
                // 从片段程序集中加载
                if (bundle.Fragments != null)
                {
                    foreach (AbstractBundle fragment in bundle.Fragments)
                    {
                        assembly = LoadBundleAssemblyFromPackages(fragment, fullName);
                        if (assembly != null)
                        {
                            this.CacheLoadedAssembly(fullName, assembly);
                            return assembly;
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
            }

            return null;
        }

        /// <summary>
        /// 从插件私有和导出包中加载程序集
        /// </summary>
        /// <param name="bundle"></param>
        /// <param name="fullName"></param>
        /// <returns></returns>
        private Assembly LoadBundleAssemblyFromPackages(AbstractBundle bundle, string fullName)
        {
            // 搜索私有包
            IExportedPackage[] packages = bundle.BundleSpecification.GetPrivilegedPackages();
            if (packages != null)
            {
                foreach (ExportedPackageImpl package in packages)
                {
                    if (package.AssemblyName.FullName.Equals(fullName))
                    {
                        // 尝试加载程序集
                        return Assembly.LoadFrom(package.ReflectOnlyPackageAssembly.Location);
                    }
                }
            }
            // 搜索导出包
            packages = bundle.BundleSpecification.GetExportedPackages();
            if (packages != null)
            {
                foreach (ExportedPackageImpl package in packages)
                {
                    if (package.AssemblyName.FullName.Equals(fullName))
                    {
                        // 尝试加载程序集
                        return Assembly.LoadFrom(package.ReflectOnlyPackageAssembly.Location);
                    }
                }
            }

            return null;
        }

        #endregion

        /// <summary>
        /// 触发启动插件
        /// </summary>
        protected override void TriggerStartBundle()
        {
            if (this.IsLazyTriggerSet())
            {
                this.state = LoaderState.Normal;
                this.bundle.Start();
            }
        }

        #endregion
    }
}

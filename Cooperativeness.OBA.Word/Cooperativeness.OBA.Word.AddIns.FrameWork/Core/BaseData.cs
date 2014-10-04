using System;
using System.Collections.Generic;
using System.Linq;
using Cooperativeness.OBA.Word.AddIns.FrameWork.MetaData;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Core
{
    /// <summary>
    /// 定义插件数据对象
    /// </summary>
    internal class BaseData : IBundleData
    {
        #region 字段
        private long id;
        private IBundle bundle;
        private int startLevel = -1;
        private BundleState state = BundleState.Installed;
        private string location;
        private long lastModified;
        protected XBundle manifest;
        private Version version;
        private Version hostBundleVersion;
        private IList<ServiceEntry> serviceEntries;
        private IList<AssemblyEntry> assemblyEntries;
        private IList<DependentBundle> dependentBundles;
        private IList<IExtension> extensions;
        private IList<IExtensionPoint> extensionPoints;

        #endregion

        #region 构造函数
        public BaseData(long id, XBundle manifest, string location)
        {
            this.id = id;
            this.manifest = manifest;
            this.location = location;
        }

        #endregion

        #region 属性
        /// <summary>
        /// 获取插件标识
        /// </summary>
        public long BundleId
        {
            get { return this.id; }
        }

        /// <summary>
        /// 获取插件唯一标识名
        /// </summary>
        public string SymbolicName
        {
            get { return this.manifest.SymbolicName; }
        }

        /// <summary>
        /// 获取插件的类型
        /// </summary>
        public BundleType BundleType
        {
            get 
            {
                if (string.IsNullOrEmpty(this.HostBundleSymbolicName))
                    return BundleType.Host;
                else
                    return BundleType.Fragment;
            }
        }

        /// <summary>
        /// 获取插件状态
        /// </summary>
        public BundleState State
        {
            get { return this.state; }
            internal set { this.state = value; }
        }

        /// <summary>
        /// 获取插件激活器
        /// </summary>
        public string Activator
        {
            get 
            {
                XActivator xActivator = this.manifest.Activator;
                if (xActivator == null) return null;
                return xActivator.Type;
            }
        }

        /// <summary>
        /// 获取或设置插件的启动级别
        /// </summary>
        public int StartLevel
        {
            get 
            {
                if (this.startLevel == -1)
                    this.startLevel = this.manifest.StartLevel;
                return this.startLevel;
                        
            }
            set { this.startLevel = value; }
        }

        /// <summary>
        /// 获取插件的激活策略
        /// </summary>
        public ActivatorPolicy Policy
        {
            get
            {
                XActivator xActivator = this.manifest.Activator;
                if (xActivator == null) return ActivatorPolicy.Lazy;
                return xActivator.Policy;
            }
            internal set
            {
                XActivator xActivator = this.manifest.Activator;
                xActivator.Policy = value;
            }
        }

        /// <summary>
        /// 获取或设置插件的位置
        /// </summary>
        public string Location
        {
            get { return this.location; }
        }

        /// <summary>
        /// 获取插件的版本号
        /// </summary>
        public Version Version
        {
            get
            {
                if (version == null)
                {
                    try
                    {
                        string bundleVersion = this.manifest.Version;
                        version = string.IsNullOrEmpty(bundleVersion) ? new Version("1.0.0.0") : new Version(bundleVersion);
                    }
                    catch
                    {
                        version = new Version("1.0.0.0");
                    }
                }
                return version;
            }
        }

        /// <summary>
        /// 获取插件最后修改时间
        /// </summary>
        public long LastModified
        {
            get { return this.lastModified; }
            internal set { this.lastModified = value; }
        }

        /// <summary>
        /// 获取插件清单对象
        /// </summary>
        public XBundle Manifest
        {
            get { return manifest; }
        }

        /// <summary>
        /// 获取插件依赖的所有程序集列表
        /// </summary>
        public AssemblyEntry[] Assemblies
        {
            get { return GetAssemblyEntries(); }
        }

        /// <summary>
        /// 获取插件依赖的所有插件列表
        /// </summary>
        public DependentBundle[] DependentBundles
        {
            get { return GetDependentBundles(); }
        }

        /// <summary>
        /// 获取插件依赖的所有扩展列表
        /// </summary>
        public IExtension[] Extensions
        {
            get { return GetExtensions(); }
        }

        /// <summary>
        /// 获取插件依赖的所有扩展点列表
        /// </summary>
        public IExtensionPoint[] ExtensionPoints
        {
            get { return GetExtensionPoints(); }
        }

        /// <summary>
        /// 获取插件依赖的所有服务列表
        /// </summary>
        public ServiceEntry[] Services
        {
            get { return GetServiceEntries(); }
        }

        /// <summary>
        /// 获取宿主插件唯一标识名
        /// </summary>
        public string HostBundleSymbolicName
        {
            get { return this.manifest.HostBundleSymbolicName; }
        }

        /// <summary>
        /// 获取宿主插件版本号
        /// </summary>
        public Version HostBundleVersion
        {
            get
            {
                if (string.IsNullOrEmpty(HostBundleSymbolicName))
                {
                    if (hostBundleVersion == null)
                    {
                        try
                        {
                            string version = this.manifest.HostBundleVersion;
                            hostBundleVersion = string.IsNullOrEmpty(version) ? new Version("1.0.0.0") : new Version(version);
                        }
                        catch
                        {
                            hostBundleVersion = new Version("1.0.0.0");
                        }
                    }
                }
                return hostBundleVersion;
            }
        }

        #endregion

        #region 方法
        /// <summary>
        /// 设置当前插件数据对象所属的插件对象
        /// </summary>
        /// <param name="bundle"></param>
        public void SetBundle(IBundle bundle)
        {
            this.bundle = bundle;
        }

        /// <summary>
        /// 将插件数据对象转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string name = SymbolicName;
            if (name == null)
                return Location;
            Version ver = Version;
            if (ver == null)
                return name;
            return name + "_" + ver; //$NON-NLS-1$
        }

        /// <summary>
        /// 获取所有服务的元数据集合
        /// </summary>
        /// <returns></returns>
        private ServiceEntry[] GetServiceEntries()
        {
            // 检查集合是否已存在
            if (serviceEntries != null)
                return serviceEntries.ToArray();
            // 获取服务集合元数据
            XServices xServices = manifest.Services;
            if (xServices == null) return null;
            // 获取所有服务元数据
            XService[] xServiceArray = xServices.Services;
            if (xServiceArray == null || xServiceArray.Length == 0)
                return null;
            foreach (XService xService in xServiceArray)
            {
                if (serviceEntries == null) serviceEntries = new List<ServiceEntry>();
                ServiceEntry entry = new ServiceEntry(xService);
                serviceEntries.Add(entry);  
            }

            return serviceEntries.ToArray();
        }

        /// <summary>
        /// 获取依赖的所有程序集的元数据集合
        /// </summary>
        /// <returns></returns>
        private AssemblyEntry[] GetAssemblyEntries()
        {
            // 检查集合是否已存在
            if (assemblyEntries != null)
                return assemblyEntries.ToArray();
            // 获取运行时元数据
            XRuntime xRuntime = manifest.Runtime;
            if (xRuntime == null) return null;
            // 获取所有依赖程序集元数据
            XAssembly[] xAssemblies = xRuntime.Assemblies;
            if (xAssemblies == null || xAssemblies.Length == 0)
                return null;
            foreach (XAssembly xAssembly in xAssemblies)
            {
                if (assemblyEntries == null) assemblyEntries = new List<AssemblyEntry>();
                AssemblyEntry entry = new AssemblyEntry(xAssembly);
                assemblyEntries.Add(entry);
            }

            return assemblyEntries.ToArray();
        }

        /// <summary>
        /// 获取依赖的所有插件的元数据集合
        /// </summary>
        /// <returns></returns>
        private DependentBundle[] GetDependentBundles()
        {
            // 检查集合是否已存在
            if (dependentBundles != null)
                return dependentBundles.ToArray();
            // 获取运行时元数据
            XRuntime xRuntime = manifest.Runtime;
            if (xRuntime == null) return null;
            // 获取所有依赖插件元数据
            XDependency[] xDependencies = xRuntime.Dependencies;
            if (xDependencies == null || xDependencies.Length == 0)
                return null;
            foreach (XDependency xDependency in xDependencies)
            {
                if (dependentBundles == null) dependentBundles = new List<DependentBundle>();
                DependentBundle entry = new DependentBundle(xDependency);
                dependentBundles.Add(entry);
            }

            return dependentBundles.ToArray();
        }

        /// <summary>
        /// 获取当前插件所有扩展的元数据集合
        /// </summary>
        /// <returns></returns>
        private IExtension[] GetExtensions()
        {
            // 检查集合是否已存在
            if (extensions != null)
                return extensions.ToArray();
            // 获取所有扩展元数据
            XExtension[] xExtensions = manifest.Extensions;
            if (xExtensions == null || xExtensions.Length == 0)
                return null;
            foreach (XExtension xExtension in xExtensions)
            {
                if (extensions == null) extensions = new List<IExtension>();
                ExtensionImpl entry = new ExtensionImpl(this.bundle,xExtension);
                extensions.Add(entry);
            }

            return extensions.ToArray();
        }

        /// <summary>
        /// 获取当前插件所有扩展点的元数据集合
        /// </summary>
        /// <returns></returns>
        private IExtensionPoint[] GetExtensionPoints()
        {
            // 检查集合是否已存在
            if (extensionPoints != null)
                return extensionPoints.ToArray();
            // 获取所有扩展元数据
            XExtensionPoint[] xExtensionPoints = manifest.ExtensionPoints;
            if (xExtensionPoints == null || xExtensionPoints.Length == 0)
                return null;
            foreach (XExtensionPoint xExtensionPoint in xExtensionPoints)
            {
                if (extensionPoints == null) extensionPoints = new List<IExtensionPoint>();
                ExtensionPointImpl entry = new ExtensionPointImpl(this.bundle, xExtensionPoint);
                extensionPoints.Add(entry);
            }

            return extensionPoints.ToArray();
        }
        #endregion
    }
}

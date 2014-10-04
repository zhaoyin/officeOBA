using System;
using System.Collections.Specialized;
using System.Reflection;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Loader
{
    /// <summary>
    /// 定义抽象的类加载器
    /// </summary>
    internal abstract class AbstractClassLoader : IClassLoader,IRuntimeService
    {
        #region 字段
        protected OrderedDictionary typeCache;
        protected OrderedDictionary assemblyCache;
        protected OrderedDictionary resourceAssemblyCache;
        protected LoaderState state = LoaderState.Normal;
        
        #endregion

        #region 构造函数
        public AbstractClassLoader()
        {
            typeCache = new OrderedDictionary();
            assemblyCache = new OrderedDictionary();
            resourceAssemblyCache = new OrderedDictionary();
        }

        #endregion

        #region 方法
        /// <summary>
        /// 根据类名获取对应的类型
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public abstract Type LoadClass(string className);

        /// <summary>
        /// 根据资源名称和加载模式加载指定的资源
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public abstract object LoadResource(string resourceName, ResourceLoadMode mode);

        /// <summary>
        /// 尝试从缓存中加载指定的类型
        /// </summary>
        /// <param name="className"></param>
        /// <param name="type"></param>
        protected bool TryGetCachedType(string className, out Type type)
        {
            type = null;
            if(string.IsNullOrEmpty(className)) return false;
            if (typeCache.Contains(className))
            {
                type = typeCache[className] as Type;
                return true;
            }
            else
            {
                Assembly[] assemblies = GetLoadedAssembliesFromCache();
                if (assemblies == null || assemblies.Length == 0)
                    return false;
                foreach (var assembly in assemblies)
                {
                    type = assembly.GetType(className);
                    if (type != null) return true;
                    else continue;
                }
            }
            return false;
        }

        /// <summary>
        /// 尝试从缓存中加载指定的程序集
        /// </summary>
        /// <param name="className"></param>
        /// <param name="type"></param>
        protected bool TryGetCachedAssembly(string fullName, out Assembly assembly)
        {
            assembly = null;
            if (string.IsNullOrEmpty(fullName)) return false;
            if (assemblyCache.Contains(fullName))
            {
                assembly = assemblyCache[fullName] as Assembly;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 尝试从缓存中加载指定的资源
        /// </summary>
        /// <param name="className"></param>
        /// <param name="type"></param>
        protected bool TryGetCachedResource(string resourceName, out object obj)
        {
            obj = null;
            if (string.IsNullOrEmpty(resourceName)) return false;
            if (resourceAssemblyCache.Contains(resourceName))
            {
                string asmName = resourceAssemblyCache[resourceName].ToString();
                Assembly assembly;
                if (TryGetCachedAssembly(asmName, out assembly))
                {
                    obj = assembly.GetManifestResourceStream(resourceName);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 缓存加载的类型
        /// </summary>
        /// <param name="className"></param>
        /// <param name="type"></param>
        protected void CacheLoadedType(string className, Type type)
        {
            // 触发启动插件
            TriggerStartBundle();
            // 缓存类型
            if (!typeCache.Contains(className))
            {
                typeCache.Add(className, type);
            }
            // 缓存组件
            if (!assemblyCache.Contains(type.Assembly.FullName))
            {
                assemblyCache.Add(type.Assembly.FullName, type.Assembly);
            }
        }

        /// <summary>
        /// 缓存加载的程序集
        /// </summary>
        /// <param name="className"></param>
        /// <param name="type"></param>
        protected void CacheLoadedAssembly(string fullName, Assembly assembly)
        {
            // 触发启动插件
            TriggerStartBundle();
            // 缓存组件
            if (!assemblyCache.Contains(fullName))
            {
                assemblyCache.Add(fullName, assembly);
            }
        }

        /// <summary>
        /// 缓存加载的资源
        /// </summary>
        /// <param name="className"></param>
        /// <param name="type"></param>
        protected void CacheLoadedResource(string resourceName, Assembly assembly)
        {
            if (!resourceAssemblyCache.Contains(resourceName))
            {
                resourceAssemblyCache.Add(resourceName, assembly.FullName);
                if (!assemblyCache.Contains(assembly.FullName))
                {
                    assemblyCache.Add(assembly.FullName, assembly);
                }
            }
        }

        /// <summary>
        /// 从缓存中获取已加载的程序集
        /// </summary>
        /// <returns></returns>
        protected Assembly[] GetLoadedAssembliesFromCache()
        {
            if(assemblyCache.Count == 0)
                return null;
            Assembly[] newAssemblies = new Assembly[assemblyCache.Count];
            assemblyCache.Values.CopyTo(newAssemblies,0);
            return newAssemblies;
        }

        /// <summary>
        /// 尝试加载系统类型
        /// </summary>
        /// <param name="className"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        protected bool TryGetSystemType(string className, out Type type)
        {
            type = Type.GetType(className);
            return type != null;
        }

        /// <summary>
        /// 根据程序集名称加载程序集
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public Assembly LoadBundleAssembly(string fullName)
        {
            // 先从缓存中加载程序集
            Assembly assembly;
            if (TryGetCachedAssembly(fullName, out assembly))
            {
                return assembly;
            }
            
            return this.OnLoadBundleAssembly(fullName); 
        }

        /// <summary>
        /// 根据程序集名称加载程序集
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        protected abstract Assembly OnLoadBundleAssembly(string fullName);

        /// <summary>
        /// 检查当前加载器是否处于懒加载待触发状态
        /// </summary>
        /// <returns></returns>
        public bool IsLazyTriggerSet()
        {
            return this.state == LoaderState.LazyTrigger;
        }

        /// <summary>
        /// 设置当前加载器为懒加载待触发状态
        /// </summary>
        public void SetLazyTrigger()
        {
            lock (this)
            {
                this.state = LoaderState.LazyTrigger;
            }
        }

        /// <summary>
        /// 触发启动插件
        /// </summary>
        protected virtual void TriggerStartBundle() { }

        #endregion

        
    }
}

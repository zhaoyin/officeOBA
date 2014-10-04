using System;
using System.Collections;
using System.Reflection;
using System.Text;
using System.Linq;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Core;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Package
{
    /// <summary>
    /// 定义导出包管理器
    /// </summary>
    internal class ExportedPackageAdmin
    {
        #region 字段
        private Hashtable packagesByName;
        private Hashtable packagesByAssembly;
        private IList packageByDeclarationOrder;
        private object lockObj = new object();

        private AbstractBundle bundle;

        #endregion

        #region 构造函数
        public ExportedPackageAdmin(AbstractBundle bundle)
        {
            this.bundle = bundle;
            packagesByName = new Hashtable();
            packagesByAssembly = new Hashtable();
            packageByDeclarationOrder = new ArrayList();
        }

        #endregion

        #region 方法
        /// <summary>
        /// 添加导入包对象
        /// </summary>
        /// <param name="package"></param>
        public void AddPackage(ExportedPackageImpl package)
        {
            if (package != null)
            {
                lock (lockObj)
                {
                    // 按声明序列存放到列表中
                    if (!packageByDeclarationOrder.Contains(package))
                        packageByDeclarationOrder.Add(package);
                    // 按名称存放到列表中
                    if (packagesByName.Contains(package.Name))
                    {
                        IList packages = packagesByName[package.Name] as IList;
                        if(!packages.Contains(package))
                            packages.Add(package);
                    }
                    else
                    {
                        IList packages = new ArrayList();
                        packages.Add(package);
                        packagesByName.Add(package.Name, packages);
                    }
                    // 按包程序集全名存放到列表中
                    string key = GenNameVersionKey(package);
                    if (!packagesByAssembly.Contains(package.AssemblyName.FullName))
                    {
                        packagesByAssembly.Add(package.AssemblyName.FullName, package);
                    }
                }
            }
        }

        /// <summary>
        /// 移除导入包对象
        /// </summary>
        /// <param name="package"></param>
        public void RemovePackage(ExportedPackageImpl package)
        {
            if (package != null)
            {
                lock (lockObj)
                {
                    // 从声明序列存放的列表中移除
                    if (!packageByDeclarationOrder.Contains(package))
                        packageByDeclarationOrder.Remove(package);
                    // 从按名称存放的列表中移除
                    if (!packagesByName.Contains(package.Name))
                    {
                        IList packages = packagesByName[package.Name] as IList;
                        if (packages.Contains(package))
                            packages.Remove(package);
                    }
                    // 按包程序集全名存放的列表中移除
                    if (!packagesByAssembly.Contains(package.AssemblyName.FullName))
                    {
                        packagesByAssembly.Remove(package.AssemblyName.FullName);
                    }
                }
            }
        }

        /// <summary>
        /// 获取所有的导出包列表
        /// </summary>
        /// <returns></returns>
        public IExportedPackage[] GetExportedPackages()
        {
            lock (lockObj)
            {
                IExportedPackage[] newPackages = new IExportedPackage[packageByDeclarationOrder.Count];
                packageByDeclarationOrder.CopyTo(newPackages, 0);
                return newPackages;
            }
        }

        /// <summary>
        /// 按名称获取导出包列表
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IExportedPackage[] GetExportedPackage(string name)
        {
            lock (lockObj)
            {
                if (packagesByName.Contains(name))
                {
                    IList packages = packagesByName[name] as IList;
                    IExportedPackage[] newPackages = new IExportedPackage[packages.Count];
                    packages.CopyTo(newPackages, 0);
                    return newPackages;
                }
                return null;
            }
        }

        /// <summary>
        /// 按名称和版本获取导出包列表
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IExportedPackage GetExportedPackage(string name,Version version)
        {
            lock (lockObj)
            {
                IExportedPackage[] packages = GetExportedPackage(name);
                if (packages == null) return null;
                foreach (var package in packages)
                {
                    if (package.Version.Equals(version))
                        return package;
                }
                return null;
            }
        }

        /// <summary>
        /// 按名称和版本获取导出包列表
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IExportedPackage GetExportedPackage(string name, Version version,string culture,string publicKeyToken)
        {
            lock (lockObj)
            {
                StringBuilder assemblyBuilder = new StringBuilder();
                assemblyBuilder.Append(name).Append(", ")
                    .AppendFormat("Version={0}, ",version.ToString())
                    .AppendFormat("Culture={0}, ",string.IsNullOrEmpty(culture)?"neutral":culture)
                    .AppendFormat("PublicKeyToken={0}",publicKeyToken);
                AssemblyName asmName = new AssemblyName(assemblyBuilder.ToString());
                
                // 检查缓存是否存在
                if (packagesByAssembly.Contains(asmName.FullName))
                {
                    return packagesByAssembly[asmName.FullName] as IExportedPackage;
                }
                return null;
            }
        }

        /// <summary>
        /// 根据导入包获取导出包
        /// </summary>
        /// <param name="name"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public IExportedPackage[] GetExportedPackages(IImportPackage importPackage)
        {
            // 如果当前是宿主插件，要求导入包指定的插件为当前插件
            if (!this.bundle.IsFragment && !importPackage.Bundle.Equals(this.bundle))
                return null;
            else             
            {
                // 所属插件是片段插件，要求其宿主必须是倒入包所依赖插件对象
                BundleHost[] hosts = this.bundle.Hosts;
                if(hosts == null || !hosts.Contains(importPackage.Bundle))
                {
                    return null;
                }
            }
            // 获取导出包
            if (importPackage.Version == null)
            {
                return bundle.BundleSpecification.GetExportedPackage(importPackage.Name);
            }
            else if (!string.IsNullOrEmpty(importPackage.Culture) && !string.IsNullOrEmpty(importPackage.PublicKeyToken))
            {
                return new IExportedPackage[] { bundle
                    .BundleSpecification
                    .GetExportedPackage(importPackage.Name, importPackage.Version, importPackage.Culture, importPackage.PublicKeyToken) };
            }

            return null;
        }

        /// <summary>
        /// 生成名称版本键
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        private string GenNameVersionKey(ExportedPackageImpl package)
        {
            return package.Name + "_" + package.Version.ToString();
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Core;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Package
{
    /// <summary>
    /// 定义插件规格对象
    /// </summary>
    internal class BundleSpecificationImpl : IBundleSpecification
    {
        #region 字段
        private AbstractBundle bundle;
        private ExportedPackageAdmin exportedPackageAdmin;
        private IList<ImportBundleImpl> importingBundles;
        private IList<IImportPackage> importingPackages;
        private IList<PrivilegedPackage> privilegedPackages;
        private IList<IServicePackage> servicePackages;

        #endregion

        #region 构造函数
        public BundleSpecificationImpl(AbstractBundle bundle)
        {
            this.bundle = bundle;
            this.exportedPackageAdmin = new ExportedPackageAdmin(bundle);
            this.importingBundles = new List<ImportBundleImpl>();
            this.importingPackages = new List<IImportPackage>();
            this.privilegedPackages = new List<PrivilegedPackage>();
            this.servicePackages = new List<IServicePackage>();
        }
        #endregion

        #region 方法
        /// <summary>
        /// 添加导入包对象
        /// </summary>
        /// <param name="package"></param>
        public void AddExportedPackage(ExportedPackageImpl package)
        {
            this.exportedPackageAdmin.AddPackage(package);
        }

        /// <summary>
        /// 移除导入包对象
        /// </summary>
        /// <param name="package"></param>
        public void RemoveExportedPackage(ExportedPackageImpl package)
        {
            this.exportedPackageAdmin.RemovePackage(package);
        }

        /// <summary>
        /// 获取所有的导出包列表
        /// </summary>
        /// <returns></returns>
        public IExportedPackage[] GetExportedPackages()
        {
            return this.exportedPackageAdmin.GetExportedPackages();
        }

        /// <summary>
        /// 根据报名获取导入包
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IExportedPackage[] GetExportedPackage(string name)
        {
            return this.exportedPackageAdmin.GetExportedPackage(name);
        }

        /// <summary>
        /// 根据包名和版本获取指定导出包
        /// </summary>
        /// <param name="name"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public IExportedPackage GetExportedPackage(string name, Version version)
        {
            return this.exportedPackageAdmin.GetExportedPackage(name, version);
        }

        /// <summary>
        /// 根据包名和版本获取指定包
        /// </summary>
        /// <param name="name"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public IExportedPackage GetExportedPackage(string name, Version version, string culture, string publicKeyToken)
        {
            return this.exportedPackageAdmin.GetExportedPackage(name, version,culture,publicKeyToken);
        }

        /// <summary>
        /// 根据导入包获取导出包
        /// </summary>
        /// <param name="name"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public IExportedPackage[] GetExportedPackages(IImportPackage importPackage)
        {
            return this.exportedPackageAdmin.GetExportedPackages(importPackage);
        }

        /// <summary>
        /// 获取所有的私有包列表
        /// </summary>
        /// <returns></returns>
        public PrivilegedPackage[] GetPrivilegedPackages()
        {
            lock (privilegedPackages)
            {
                return privilegedPackages.ToArray();
            }
        }

        /// <summary>
        /// 添加私有包
        /// </summary>
        /// <returns></returns>
        public void AddPrivilegedPackage(PrivilegedPackage package)
        {
            lock (privilegedPackages)
            {
                if (!this.privilegedPackages.Contains(package))
                {
                    this.privilegedPackages.Add(package);
                }
            }
        }

        /// <summary>
        /// 移除私有包
        /// </summary>
        /// <returns></returns>
        public void RemovePrivilegedPackage(PrivilegedPackage package)
        {
            lock (privilegedPackages)
            {
                if (this.privilegedPackages.Contains(package))
                {
                    this.privilegedPackages.Remove(package);
                }
            }
        }

        /// <summary>
        /// 添加导入插件
        /// </summary>
        /// <returns></returns>
        public void AddImportBundle(ImportBundleImpl importBundle)
        {
            lock (importingBundles)
            {
                if (!this.importingBundles.Contains(importBundle))
                {
                    this.importingBundles.Add(importBundle);
                }
            }
        }

        /// <summary>
        /// 移除导入插件
        /// </summary>
        /// <returns></returns>
        public void RemoveImportBundle(ImportBundleImpl importBundle)
        {
            lock (importingBundles)
            {
                if (this.importingBundles.Contains(importBundle))
                {
                    this.importingBundles.Remove(importBundle);
                }
            }
        }

        /// <summary>
        /// 获取导入的插件列表
        /// </summary>
        /// <returns></returns>
        public IImportBundle[] GetImportBundles()
        {
            lock (importingBundles)
            {
                return this.importingBundles.ToArray();
            }
        }

        /// <summary>
        /// 添加导入包对象
        /// </summary>
        /// <returns></returns>
        public void AddImportPackage(IImportPackage package)
        {
            lock (importingPackages)
            {
                if (!this.importingPackages.Contains(package))
                {
                    this.importingPackages.Add(package);
                }
            }
        }

        /// <summary>
        /// 移除导入包对象
        /// </summary>
        /// <returns></returns>
        public void RemoveImportPackage(IImportPackage package)
        {
            lock (importingPackages)
            {
                if (this.importingPackages.Contains(package))
                {
                    this.importingPackages.Remove(package);
                }
            }
        }

        /// <summary>
        /// 获取导入包列表
        /// </summary>
        /// <returns></returns>
        public IImportPackage[] GetImportPackages()
        {
            lock (importingPackages)
            {
                return this.importingPackages.ToArray();
            }
        }

        /// <summary>
        /// 添加服务包对象
        /// </summary>
        /// <returns></returns>
        public void AddServicePackage(IServicePackage servicePackage)
        {
            lock (servicePackages)
            {
                if (!this.servicePackages.Contains(servicePackage))
                {
                    this.servicePackages.Add(servicePackage);
                }
            }
        }

        /// <summary>
        /// 移除服务包对象
        /// </summary>
        /// <returns></returns>
        public void RemoveServicePackage(IServicePackage servicePackage)
        {
            lock (servicePackages)
            {
                if (this.servicePackages.Contains(servicePackage))
                {
                    this.servicePackages.Remove(servicePackage);
                }
            }
        }

        /// <summary>
        /// 获取服务包列表
        /// </summary>
        /// <returns></returns>
        public IServicePackage[] GetServicePackages()
        {
            lock (servicePackages)
            {
                return this.servicePackages.ToArray();
            }
        }
        #endregion
    }
}

using System;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Package
{
    /// <summary>
    /// 定义插件规格接口契约
    /// </summary>
    internal interface IBundleSpecification
    {
        /// <summary>
        /// 添加导入包对象
        /// </summary>
        /// <param name="package"></param>
        void AddExportedPackage(ExportedPackageImpl package);

        /// <summary>
        /// 移除导入包对象
        /// </summary>
        /// <param name="package"></param>
        void RemoveExportedPackage(ExportedPackageImpl package);

        /// <summary>
        /// 获取所有的导出包列表
        /// </summary>
        /// <returns></returns>
        IExportedPackage[] GetExportedPackages();

        /// <summary>
        /// 根据报名获取导入包
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IExportedPackage[] GetExportedPackage(string name);

        /// <summary>
        /// 根据包名和版本获取指定导出包
        /// </summary>
        /// <param name="name"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        IExportedPackage GetExportedPackage(string name, Version version);

        /// <summary>
        /// 根据包名和版本获取指定包
        /// </summary>
        /// <param name="name"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        IExportedPackage GetExportedPackage(string name, Version version,string culture,string publicKeyToken);

        /// <summary>
        /// 根据导入包获取导出包
        /// </summary>
        /// <param name="name"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        IExportedPackage[] GetExportedPackages(IImportPackage importPackage);

        /// <summary>
        /// 添加私有包
        /// </summary>
        /// <returns></returns>
        void AddPrivilegedPackage(PrivilegedPackage package);

        /// <summary>
        /// 移除私有包
        /// </summary>
        /// <returns></returns>
        void RemovePrivilegedPackage(PrivilegedPackage package);

        /// <summary>
        /// 获取所有的私有包列表
        /// </summary>
        /// <returns></returns>
        PrivilegedPackage[] GetPrivilegedPackages();

        /// <summary>
        /// 添加导入插件
        /// </summary>
        /// <returns></returns>
        void AddImportBundle(ImportBundleImpl importBundle);

        /// <summary>
        /// 移除导入插件
        /// </summary>
        /// <returns></returns>
        void RemoveImportBundle(ImportBundleImpl importBundle);

        /// <summary>
        /// 获取导入的插件列表
        /// </summary>
        /// <returns></returns>
        IImportBundle[] GetImportBundles();

        /// <summary>
        /// 添加导入包对象
        /// </summary>
        /// <returns></returns>
        void AddImportPackage(IImportPackage package);

        /// <summary>
        /// 移除导入包对象
        /// </summary>
        /// <returns></returns>
        void RemoveImportPackage(IImportPackage package);

        /// <summary>
        /// 获取导入包列表
        /// </summary>
        /// <returns></returns>
        IImportPackage[] GetImportPackages();

        /// <summary>
        /// 添加服务包对象
        /// </summary>
        /// <returns></returns>
        void AddServicePackage(IServicePackage servicePackage);

        /// <summary>
        /// 移除服务包对象
        /// </summary>
        /// <returns></returns>
        void RemoveServicePackage(IServicePackage servicePackage);

        /// <summary>
        /// 获取服务包列表
        /// </summary>
        /// <returns></returns>
        IServicePackage[] GetServicePackages();
    }
}

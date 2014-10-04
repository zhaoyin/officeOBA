using System;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork
{
    /// <summary>
    /// 定义插件数据接口契约
    /// </summary>
    internal interface IBundleData
    {
        /// <summary>
        /// 获取插件的标识
        /// </summary>
        long BundleId { get; }

        /// <summary>
        /// 获取插件的唯一标识名
        /// </summary>
        string SymbolicName { get; }

        /// <summary>
        /// 获取插件的状态
        /// </summary>
        BundleState State { get; }

        /// <summary>
        /// 获取插件的启动级别
        /// </summary>
        int StartLevel { get; set; }

        /// <summary>
        /// 获取插件的位置
        /// </summary>
        string Location { get; }

        /// <summary>
        /// 获取插件的版本信息
        /// </summary>
        Version Version { get; }

        /// <summary>
        /// 获取插件的激活策略
        /// </summary>
        ActivatorPolicy Policy { get; }

        /// <summary>
        /// 获取插件的类型
        /// </summary>
        BundleType BundleType { get; }

        /// <summary>
        /// 获取插件的激活器类型全名
        /// </summary>
        string Activator { get; }

        /// <summary>
        /// 设置与当前插件数据对象关联的插件对象
        /// </summary>
        /// <param name="bundle"></param>
        void SetBundle(IBundle bundle);

        /// <summary>
        /// 获取插件最后修改时间
        /// </summary>
        long LastModified { get; }

        /// <summary>
        /// 获取宿主插件名称
        /// </summary>
        string HostBundleSymbolicName { get; }

        /// <summary>
        /// 获取宿主插件的版本号
        /// </summary>
        Version HostBundleVersion { get; }

        /// <summary>
        /// 获取运行时依赖的程序集列表
        /// </summary>
        AssemblyEntry[] Assemblies { get; }

        /// <summary>
        /// 获取依赖的插件列表
        /// </summary>
        DependentBundle[] DependentBundles { get; }

        /// <summary>
        /// 获取插件的扩展列表
        /// </summary>
        IExtension[] Extensions { get; }

        /// <summary>
        /// 获取插件的扩展点列表
        /// </summary>
        IExtensionPoint[] ExtensionPoints { get; }

        /// <summary>
        /// 获取插件的服务列表
        /// </summary>
        ServiceEntry[] Services { get; }
    }
}

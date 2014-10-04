using System;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Package
{
    /// <summary>
    /// 定义导出包接口契约
    /// </summary>
    internal interface IExportedPackage
    {
        /// <summary>
        /// 获取程序集名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 获取程序集版本号
        /// </summary>
        Version Version { get; }

        /// <summary>
        /// 获取程序集支持的语言
        /// </summary>
        string Culture { get; }

        /// <summary>
        /// 获取程序集的公钥令牌
        /// </summary>
        string PublicKeyToken { get; }

        /// <summary>
        /// 获取导出包的插件对象
        /// </summary>
        IBundle ExportingBundle { get; }
    }
}

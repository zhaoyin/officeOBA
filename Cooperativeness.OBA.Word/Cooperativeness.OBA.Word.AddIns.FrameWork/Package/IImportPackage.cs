using System;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Package
{
    /// <summary>
    /// 定义导入包接口契约
    /// </summary>
    public interface IImportPackage : IImportBundle
    {
        /// <summary>
        /// 获取包名
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 获取包的版本号
        /// </summary>
        Version Version { get; }

        /// <summary>
        /// 获取程序集支持的语言
        /// </summary>
        string Culture { get; }

        /// <summary>
        /// 获取程序集的公钥Token信息
        /// </summary>
        string PublicKeyToken { get; }

        /// <summary>
        /// 获取导出包所属的插件
        /// </summary>
        IBundle ImportingBundle { get; }
    }
}

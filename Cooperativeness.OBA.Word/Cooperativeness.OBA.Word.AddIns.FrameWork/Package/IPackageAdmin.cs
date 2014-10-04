using System;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Core;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Package
{
    /// <summary>
    /// 定义包管理器契约
    /// </summary>
    internal interface IPackageAdmin
    {
	    /// <summary>
	    /// 解析指定的插件列表
	    /// </summary>
	    /// <param name="bundles"></param>
	    /// <returns></returns>
	    bool ResolveBundles(IBundle[] bundles);

        /// <summary>
        /// 解析插件扩展
        /// </summary>
        /// <param name="bundle"></param>
        void ResolveExtension(AbstractBundle bundle);

	    /// <summary>
	    /// 获取具有指定标识名和版本范围的插件列表
	    /// </summary>
	    /// <param name="symbolicName"></param>
	    /// <param name="versionRange"></param>
	    /// <returns></returns>
        IBundle[] GetBundles(string symbolicName, string versionRange);

	    /// <summary>
	    /// 获取指定插件的片段插件列表
	    /// </summary>
	    /// <param name="bundle"></param>
	    /// <returns></returns>
        IBundle[] GetFragments(IBundle bundle);

	    /// <summary>
	    /// 获取指定插件的所有宿主插件
	    /// </summary>
	    /// <param name="bundle"></param>
	    /// <returns></returns>
        IBundle[] GetHosts(IBundle bundle);

        /// <summary>
        /// 根据指定的类型，获取同一个加载器加载的插件
        /// 如果该类，未被加载过，则直接返回空
        /// </summary>
        /// <param name="clazz"></param>
        /// <returns></returns>
	    IBundle GetBundle(Type clazz);

	    /// <summary>
	    /// 获取指定插件的类型
	    /// </summary>
	    /// <param name="bundle"></param>
	    /// <returns></returns>
        BundleType GetBundleType(IBundle bundle);
    }
}

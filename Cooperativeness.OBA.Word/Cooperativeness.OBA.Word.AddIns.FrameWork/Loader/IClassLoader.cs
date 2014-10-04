using System;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Loader
{
    /// <summary>
    /// 定义类加载器接口契约
    /// </summary>
    internal interface IClassLoader
    {
        /// <summary>
        /// 根据类名获取对应的类型
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        Type LoadClass(string className);

        /// <summary>
        /// 根据资源名称和加载模式加载指定的资源
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        object LoadResource(string resourceName, ResourceLoadMode mode);
    }
}

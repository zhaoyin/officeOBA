using System.Collections.Generic;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork
{
    /// <summary>
    /// 定义插件扩展点契约
    /// </summary>
    public interface IExtensionPoint
    {
        /// <summary>
        /// 获取扩展点所有者
        /// </summary>
        IBundle Owner { get; }

        /// <summary>
        /// 获取扩展点所有的扩展
        /// </summary>
        IList<IExtension> Extensions { get; }

        /// <summary>
        /// 获取扩展点
        /// </summary>
        string Point { get; }

        /// <summary>
        /// 获取扩展信息需要遵循的架构
        /// </summary>
        string Schema { get; }

        /// <summary>
        /// 添加扩展信息
        /// </summary>
        /// <param name="extension"></param>
        bool AddExtension(IExtension extension);

        /// <summary>
        /// 移除扩展信息
        /// </summary>
        /// <param name="extension"></param>
        bool RemoveExtension(IExtension extension);
    }
}

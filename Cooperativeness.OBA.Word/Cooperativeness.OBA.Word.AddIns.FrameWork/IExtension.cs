using System.Collections.Generic;
using System.Xml.Linq;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork
{
    /// <summary>
    /// 定义插件扩展接口契约
    /// </summary>
    public interface IExtension
    {
        /// <summary>
        /// 获取扩展对象所属的插件
        /// </summary>
        IBundle Owner { get; }

        /// <summary>
        /// 获取扩展点
        /// </summary>
        string Point { get; }

        /// <summary>
        /// 获取扩展的数据
        /// </summary>
        IList<XElement> Data { get; }
    }
}

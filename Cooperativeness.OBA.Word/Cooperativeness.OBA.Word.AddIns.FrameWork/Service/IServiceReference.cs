using System;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Service
{
    /// <summary>
    /// 定义服务引用契约
    /// </summary>
    public interface IServiceReference : IComparable
    {
        /// <summary>
        /// 获取属性键数组
        /// </summary>
        /// <returns></returns>
        string[] PropertyKeys { get; }

        /// <summary>
        /// 获取引用服务的注册的插件对象
        /// </summary>
        IBundle Bundle { get; }

        /// <summary>
        /// 获取所有使用服务的插件对象
        /// </summary>
        /// <returns></returns>
        IBundle[] UsingBundles { get; }

        /// <summary>
        /// 根据属性键获取属性值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object GetProperty(string key);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bundle"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        bool IsAssignableTo(IBundle bundle, string className);
    }
}

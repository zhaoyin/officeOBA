using System.Collections.Generic;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Package
{
    /// <summary>
    /// 定义服务包接口契约
    /// </summary>
    internal interface IServicePackage
    {
        /// <summary>
        /// 获取接口类型
        /// </summary>
        string InterfaceType { get; }

        /// <summary>
        /// 获取服务类型
        /// </summary>
        string ServiceType { get; }

        /// <summary>
        /// 获取服务属性
        /// </summary>
        IDictionary<string, object> ServiceProperty { get; }

        /// <summary>
        /// 获取服务的拥有者
        /// </summary>
        IBundle Owner { get; }
    }
}

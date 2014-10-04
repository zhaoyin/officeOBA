using System.Collections.Generic;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Service
{
    /// <summary>
    /// 定义服务注册者契约
    /// </summary>
    public interface IServiceRegistration
    {
        /// <summary>
        /// 获取一个服务引用
        /// </summary>
        IServiceReference Reference { get; }

        /// <summary>
        /// 更新服务的属性
        /// </summary>
        /// <param name="property"></param>
        void SetProperties(IDictionary<string,object> properties);

        /// <summary>
        /// 注销一个服务
        /// </summary>
        void Unregister();
    }
}

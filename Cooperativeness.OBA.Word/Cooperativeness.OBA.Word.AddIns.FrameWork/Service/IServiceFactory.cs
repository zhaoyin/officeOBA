
namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Service
{
    /// <summary>
    /// 定义服务工厂契约
    /// </summary>
    public interface IServiceFactory
    {
        /// <summary>
        /// 获取一个服务对象
        /// </summary>
        /// <param name="bundle"></param>
        /// <param name="registration"></param>
        /// <returns></returns>
        object GetService(IBundle bundle, IServiceRegistration registration);

        /// <summary>
        /// 释放一个服务对象
        /// </summary>
        /// <param name="bundle"></param>
        /// <param name="registration"></param>
        /// <param name="service"></param>
        void UngetService(IBundle bundle, IServiceRegistration registration, object service);
    }
}

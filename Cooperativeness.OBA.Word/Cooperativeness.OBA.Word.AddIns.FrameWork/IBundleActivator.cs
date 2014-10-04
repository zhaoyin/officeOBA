
namespace Cooperativeness.OBA.Word.AddIns.FrameWork
{
    /// <summary>
    /// 定义插件激活器接口契约
    /// </summary>
    public interface IBundleActivator
    {
        /// <summary>
        /// 启动插件
        /// </summary>
        /// <param name="context">插件上下文</param>
        void Start(IBundleContext context);

        /// <summary>
        /// 停止Bundle
        /// </summary>
        /// <param name="context">插件上下文</param>
        void Stop(IBundleContext context);
    }
}

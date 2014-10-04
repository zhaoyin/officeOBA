
namespace Cooperativeness.OBA.Word.AddIns.FrameWork
{
    /// <summary>
    /// 定义Bundle激活器激活策略
    /// </summary>
    public enum ActivatorPolicy
    {
        /// <summary>
        /// 立即激活，当框架激活时立即启动Bundle
        /// </summary>
        Immediate,

        /// <summary>
        /// 晚激活，当Bundle类加载时激活
        /// </summary>
        Lazy
    }
}

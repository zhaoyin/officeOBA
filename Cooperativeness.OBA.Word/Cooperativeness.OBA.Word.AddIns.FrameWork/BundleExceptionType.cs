
namespace Cooperativeness.OBA.Word.AddIns.FrameWork
{
    /// <summary>
    /// 定义异常类型
    /// </summary>
    public enum BundleExceptionType
    {
        /// <summary>
        /// 未知异常
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// 不支持此操作异常
        /// </summary>
        UNSUPPORTED_OPERATION,

        /// <summary>
        /// 非法操作异常
        /// </summary>
        INVALID_OPERATION,

        /// <summary>
        /// 插件清单异常
        /// </summary>
        MANIFEST_ERROR,

        /// <summary>
        /// 解析异常
        /// </summary>
        RESOLVE_ERROR,

        /// <summary>
        /// 激活器发生错误异常
        /// </summary>
        ACTIVATOR_ERROR,

        /// <summary>
        /// 状态转换异常
        /// </summary>
        STATECHANGE_ERROR,

        /// <summary>
        /// 当机异常，安装失败造成的异常
        /// </summary>
        DUPLICATE_BUNDLE_ERROR,

        /// <summary>
        /// 启动异常，由于插件激动级别高于框架的启动级别
        /// </summary>
        START_TRANSIENT_ERROR
    }
}

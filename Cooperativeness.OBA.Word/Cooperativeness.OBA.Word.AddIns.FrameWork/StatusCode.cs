
namespace Cooperativeness.OBA.Word.AddIns.FrameWork
{
    /// <summary>
    /// 定义状态码
    /// </summary>
    public enum StatusCode
    {
        /// <summary>
        /// 确认状态码
        /// </summary>
        OK = 1,

        /// <summary>
        /// 通知状态码
        /// </summary>
        INFO = 1<<1,

        /// <summary>
        /// 警告状态码
        /// </summary>
        WARNING = 1 << 2,

        /// <summary>
        /// 错误状态码
        /// </summary>
        ERROR = 1 << 3,
    }
}

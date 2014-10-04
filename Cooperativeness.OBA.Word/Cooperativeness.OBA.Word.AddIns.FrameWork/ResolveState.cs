using System;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork
{
    /// <summary>
    /// 定义解析状态
    /// </summary>
    [Flags]
    public enum ResolveState
    {
        /// <summary>
        /// 未解析状态
        /// </summary>
        Unresolved = 1,

        /// <summary>
        /// 已解析状态
        /// </summary>
        Resolved = 1 << 1,

        /// <summary>
        /// 成功状态
        /// </summary>
        Success = 1 << 2,

        /// <summary>
        /// 失败
        /// </summary>
        Fault = 1 << 3
    }
}

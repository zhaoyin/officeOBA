using System;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork
{
    /// <summary>
    /// 定义Bundle的状态
    /// </summary>
    [Flags]
    public enum BundleState
    {
        /// <summary>
        /// 空状态
        /// </summary>
        [EnumString("None")]
        None = 0,

        /// <summary>
        /// 已安装状态
        /// </summary>
        [EnumString("Installed")]
        Installed = 1,

        /// <summary>
        /// 已解析状态
        /// </summary>
        [EnumString("Resolved")]
        Resolved = 1<<1,

        /// <summary>
        /// 正在启动状态
        /// </summary>
        [EnumString("Starting")]
        Starting = 1<<2,

        /// <summary>
        /// 已激活状态
        /// </summary>
        [EnumString("Active")]
        Active = 1 << 3,

        /// <summary>
        /// 正在停止状态
        /// </summary>
        [EnumString("Stopped")]
        Stopping = 1 << 4,
    }
}

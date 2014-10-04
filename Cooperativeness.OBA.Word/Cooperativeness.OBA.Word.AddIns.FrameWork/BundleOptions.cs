using System;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork
{
    /// <summary>
    /// 定义Bundle启动或停止选项
    /// </summary>
    [Serializable]
    public enum BundleOptions
    {
        /// <summary>
        /// 传统启动
        /// </summary>
        General = 0,

        /// <summary>
        /// 临时启动
        /// </summary>
        Transient,
    }
}


namespace Cooperativeness.OBA.Word.AddIns.FrameWork
{
    /// <summary>
    /// 定义依赖解析方式
    /// </summary>
    public enum ResolutionMode
    {
        /// <summary>
        ///  依赖必须强制解析成功这个插件才能够被启动
        /// </summary>
        [EnumString("Mandatory")]
        Mandatory,

        /// <summary>
        /// 依赖可以解析失败
        /// </summary>
        [EnumString("Optional")]
        Optional
    }
}

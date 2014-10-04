
namespace Cooperativeness.OBA.Word.AddIns.FrameWork
{
    /// <summary>
    /// 定义资源加载模式
    /// </summary>
    public enum ResourceLoadMode
    {
        /// <summary>
        /// 在Bundle本地的Export中搜索资源。
        /// </summary>
        Local,

        /// <summary>
        /// 在Bundle本地和片段的Export中搜索资源。
        /// </summary>
        FragmentAndLocal,

        /// <summary>
        /// 在Bundle的类型空间中搜索资源，此搜索方式与类搜索方式一样。 
        /// </summary>
        ClassSpace
    }
}

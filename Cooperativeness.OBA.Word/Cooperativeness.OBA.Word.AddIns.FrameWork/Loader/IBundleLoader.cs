using Cooperativeness.OBA.Word.AddIns.FrameWork.Core;


namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Loader
{
    /// <summary>
    /// 定义插件加载器
    /// </summary>
    internal interface IBundleLoader : IClassLoader
    {
        /// <summary>
        /// 获取当前插件框架
        /// </summary>
        Framework Framework { get; }

        /// <summary>
        /// 获取当前加载器所属的插件
        /// </summary>
        IBundle Bundle { get; }
    }
}

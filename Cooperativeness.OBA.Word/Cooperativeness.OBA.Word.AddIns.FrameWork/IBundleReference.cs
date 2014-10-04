
namespace Cooperativeness.OBA.Word.AddIns.FrameWork
{
    /// <summary>
    /// 定义插件引用接口契约
    /// </summary>
    public interface IBundleReference
    {
        /// <summary>
        /// 获取当前引用的插件对象
        /// </summary>
        IBundle Bundle { get; }
    }
}

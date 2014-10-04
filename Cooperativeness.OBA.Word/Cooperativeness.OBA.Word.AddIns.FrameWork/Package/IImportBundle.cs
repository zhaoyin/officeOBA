
namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Package
{
    /// <summary>
    /// 定义导入的插件接口契约
    /// </summary>
    public interface IImportBundle
    {
        /// <summary>
        /// 获取导入的插件对象
        /// </summary>
        IBundle Bundle { get; }

        /// <summary>
        /// 获取依赖解析方式
        /// </summary>
        ResolutionMode Resolution { get; }
    }
}

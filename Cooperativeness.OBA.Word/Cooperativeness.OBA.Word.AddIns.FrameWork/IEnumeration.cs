
namespace Cooperativeness.OBA.Word.AddIns.FrameWork
{
    /// <summary>
    /// 定义枚举接口契约
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEnumeration<T>
    {
        /// <summary>
        /// 检查是否还有更多的元素
        /// </summary>
        /// <returns></returns>
        bool HasMoreElements();

        /// <summary>
        /// 获取下一个元素
        /// </summary>
        /// <returns></returns>
        T NextElement();
    }
}

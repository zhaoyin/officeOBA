
namespace Cooperativeness.OBA.Word.AddIns.FrameWork
{
    /// <summary>
    /// 定义状态异常接口契约
    /// </summary>
    public interface IStatusExpception
    {
        /// <summary>
        /// 获取状态对象
        /// </summary>
        object Status{get;}

        /// <summary>
        /// 获取状态码
        /// </summary>
        StatusCode StatusCode{get;}
    }
}

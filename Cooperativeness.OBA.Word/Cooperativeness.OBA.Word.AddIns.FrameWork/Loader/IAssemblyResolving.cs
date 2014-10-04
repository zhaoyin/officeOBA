
namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Loader
{
    /// <summary>
    /// 定义组件解析器接口契约
    /// </summary>
    internal interface IAssemblyResolving : IRuntimeService
    {
        /// <summary>
        /// 启动程序集解析器
        /// </summary>
        void Start();

        /// <summary>
        /// 停止程序集解析器
        /// </summary>
        void Stop();
    }

}

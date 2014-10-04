using System.Reflection;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Loader
{
    /// <summary>
    /// 定义类服务接口契约
    /// </summary>
    internal interface IRuntimeService
    {
        /// <summary>
        /// 根据程序集名称加载程序集
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        Assembly LoadBundleAssembly(string fullName);
    }
}

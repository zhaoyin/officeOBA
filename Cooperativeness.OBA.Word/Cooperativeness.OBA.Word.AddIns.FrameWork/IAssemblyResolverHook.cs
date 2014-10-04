using System.Reflection;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork
{
    /// <summary>
    /// 定义程序集加载器钩子
    /// </summary>
    public interface IAssemblyResolverHook
    {
        /// <summary>
        /// 根据程序集全名，获取指定的程序集
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        Assembly Find(string fullName);
    }
}


namespace Cooperativeness.OBA.Word.Tools.Language
{
    /// <summary>
    /// 定义多语资源管理器
    /// </summary>
    public interface IMultiLanguage
    {
        /// <summary>
        /// 根据多语名称获取多语值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string GetString(string name);

        /// <summary>
        /// 根据当前多语名称和语言标识获取多语值
        /// </summary>
        /// <param name="name"></param>
        /// <param name="langId"></param>
        /// <returns></returns>
        string GetString(string name, string langId);
    }

}

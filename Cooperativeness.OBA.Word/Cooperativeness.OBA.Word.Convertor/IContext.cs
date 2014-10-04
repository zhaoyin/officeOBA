
namespace Cooperativeness.OBA.Word.Convertor
{
    /// <summary>
    /// 定义上下文契约
    /// </summary>
    public interface IContext
    {
        /// <summary>
        /// 获取工作目录
        /// </summary>
        string Workplace { get; }

        /// <summary>
        /// 在当前工作目录下创建一个临时目录
        /// </summary>
        /// <returns></returns>
        string CreateDirectory();

        /// <summary>
        /// 根据用户指定的目录名，在当前工作目录下创建一个临时目录
        /// </summary>
        /// <returns></returns>
        string CreateDirectory(string name);

        /// <summary>
        /// 拷贝文件，将指定的文件拷贝到指定目录下
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        void Copy(string source, string target);
    }
}

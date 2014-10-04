
namespace Cooperativeness.OBA.Word.Convertor
{
    /// <summary>
    /// 定义文档转换接口契约
    /// </summary>
    public interface IConverter
    {
        /// <summary>
        /// 将指定的文件进行转换处理
        /// </summary>
        /// <param name="fileName">要转换的文档名称</param>
        /// <param name="outputDir">输出路径</param>
        /// <returns>返回}换后的文档路径</returns>
        string ConvertTo(string fileName, string outputDir);
    }
}

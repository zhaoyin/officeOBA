using System;
using System.IO;
using Cooperativeness.OBA.Word.Tools;

namespace Cooperativeness.OBA.Word.Convertor
{
    /// <summary>
    /// 定义抽象的转换器
    /// </summary>
    public abstract class AbstractConverter : IConverter
    {
        private static readonly Logger Log = new Logger(typeof(AbstractConverter));
        #region 方法
        /// <summary>
        /// 将指定的文件进行转换处理
        /// </summary>
        /// <param name="outputDir"></param>
        /// <param name="fileName">要转换的文档名称</param>
        /// <returns>转换后的文档路径</returns>
        public string ConvertTo(string fileName, string outputDir)
        {
            try
            {
                if (Verify(fileName, outputDir))
                {
                    return OnConvert(fileName, outputDir);
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
            }
            return null;
        }

        /// <summary>
        /// 用户定制转换处理逻辑
        /// </summary>
        /// <param name="fileName">要转换的文档名称</param>
        /// <returns>转换后的文档路径</returns>
        protected abstract string OnConvert(string fileName, string outputDir);

        /// <summary>
        /// 校验处理，验证文件是否存在，验证转换器是否支持当前的文件格式
        /// </summary>
        /// <param name="fileName">要转换的文档名称</param>
        /// <returns>如果符合则返回true，否则返回false</returns>
        protected bool Verify(string fileName, string outputDir)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    bool success = OnVerify(fileName);
                    if (success)
                    {
                        if (!Directory.Exists(outputDir))
                            Directory.CreateDirectory(outputDir);
                        return true;
                    }
                }
            }
            catch { }
            return false;
        }

        /// <summary>
        /// 用户定制的校验逻辑
        /// </summary>
        /// <param name="fileName">要转换的文档名称</param>
        /// <returns>如果符合则返回true，否则返回false</returns>
        protected abstract bool OnVerify(string fileName);

        /// <summary>
        /// 根据模式创建转换器
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static IConverter CreateConverter(ConverterMode mode)
        {
            switch (mode)
            {
                case ConverterMode.HTML:
                    return new HtmlConverter();
                case ConverterMode.WORD:
                    return new WordConverter();
                default:
                    return null;
            }
        }

        #endregion
    }
}

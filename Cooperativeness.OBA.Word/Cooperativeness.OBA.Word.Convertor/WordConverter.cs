using System;
using System.IO;
using OfficeWord = Microsoft.Office.Interop.Word;

namespace Cooperativeness.OBA.Word.Convertor
{
    /// <summary>
    /// 定义Word转换器
    /// </summary>
    internal class WordConverter:AbstractConverter
    {
        /// <summary>
        /// 转换处理
        /// </summary>
        /// <param name="outputDir"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        protected override string OnConvert(string fileName, string outputDir)
        {
            // 定义Word应用程序对象
            OfficeWord.Application wordApp = null;
            // 定义Word文档
            OfficeWord.Document doc = null;
            // 缺省参数
            object unknown = Type.Missing;
            // 定义临时目录
            string tempDir = string.Empty;
            try
            {
                // 将打开的文件拷贝到临时目录下
                tempDir = Path.Combine(outputDir, Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempDir);
                string target = Path.Combine(tempDir, Path.GetFileName(fileName));
                FileUtils.Copy(fileName, target);
                // 创建Word应用程序对象
                wordApp = new OfficeWord.ApplicationClass();
                // 设置可见性
                wordApp.Visible = false;
                // 保存文件路径
                string saveWordPath = Path.Combine(outputDir, Path.GetFileNameWithoutExtension(fileName) + ".docx");
                object saveTarget = saveWordPath;
                // 打开文件
                object source = target;
                // 以只读的方式打开
                object readOnly = true;
                // 打开Word文档
                doc = wordApp.Documents.Open(ref source, ref unknown, ref readOnly
                    , ref unknown, ref unknown, ref unknown
                    , ref unknown, ref unknown, ref unknown
                    , ref unknown, ref unknown, ref unknown
                    , ref unknown, ref unknown, ref unknown,ref unknown);
                // 设置格式
                object saveFormat = OfficeWord.WdSaveFormat.wdFormatDocumentDefault;
                // 设置视图样式
                doc.ActiveWindow.View.Type = OfficeWord.WdViewType.wdPrintView;
                // 另存为HTML格式文件
                doc.SaveAs(ref saveTarget, ref saveFormat
                    , ref unknown, ref unknown, ref unknown
                    , ref unknown, ref unknown, ref unknown
                    , ref unknown, ref unknown, ref unknown
                    , ref unknown, ref unknown, ref unknown
                    , ref unknown, ref unknown);

                return saveWordPath;
            }
            catch
            {
                return null;
            }
            finally
            {
                // 关闭文档
                if (doc != null) doc.Close(ref unknown, ref unknown, ref unknown);
                // 退出Word应用
                if (wordApp != null) wordApp.Quit(ref unknown, ref unknown, ref unknown);
                // 删除临时目录
                if (!string.IsNullOrEmpty(tempDir) && Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }
        }

        /// <summary>
        /// 校验处理
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        protected override bool OnVerify(string fileName)
        {
            return fileName.IsHtmlDocument();
        }
    }
}

using System.IO;

namespace Cooperativeness.OBA.Word.Convertor
{
    /// <summary>
    /// 定义字符串扩展对象
    /// </summary>
    public static class DocumentExntends
    {
        /// <summary>
        /// 判断是否是HTML文档
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool IsHtmlDocument(this string fileName)
        {
            // 获取扩展名
            if (string.IsNullOrEmpty(fileName)) return false;
            string extension = Path.GetExtension(fileName).ToLower();
            if (!extension.Equals(".htm") && !extension.Equals(".html"))
                return false;
            return true;
        }

        /// <summary>
        /// 判断是否是WORD文档
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool IsWordDocument(this string fileName)
        {
            // 获取扩展名
            if (string.IsNullOrEmpty(fileName)) return false;
            string extension = Path.GetExtension(fileName).ToLower();
            if (!extension.Equals(".doc") && !extension.Equals(".docx"))
                return false;
            return true;
        }

        /// <summary>
        /// 检查HTML文档是否含有附件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool HasHtmlAttachment(this string fileName)
        {
            if (fileName.IsHtmlDocument())
            {
                string path = Path.GetDirectoryName(fileName);
                string name = Path.GetFileNameWithoutExtension(fileName);
                string attachmentDirName = Path.Combine(path, name + ".files");
                if (Directory.Exists(attachmentDirName))
                {
                    return true;
                }
            }
            return false;
        }
    }
}

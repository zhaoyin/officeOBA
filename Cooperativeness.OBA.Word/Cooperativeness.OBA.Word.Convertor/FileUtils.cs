using System;
using System.IO;

namespace Cooperativeness.OBA.Word.Convertor
{
    /// <summary>
    /// 定义文件助手类
    /// </summary>
    public class FileUtils
    {
        /// <summary>
        /// 拷贝文件到指定的目录下面
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public static void Copy(string source, string target)
        {
            // 将文件复制到缓存路径下
            File.Copy(source, target, true);
            if (source.IsHtmlDocument())
            {
                // 获取文件名
                string name = Path.GetFileNameWithoutExtension(source);
                // 获取附件名
                string attachmentDir = Path.Combine(Path.GetDirectoryName(source), name + ".files");
                // 创建目录
                DirectoryInfo sourceDirInfo = new DirectoryInfo(attachmentDir);
                if (sourceDirInfo.Exists)
                {
                    DirectoryInfo descDirInfo = new DirectoryInfo(Path.GetDirectoryName(target));
                    // 拷贝目录
                    CopyDirectory(sourceDirInfo.FullName, descDirInfo.FullName + @"\" + sourceDirInfo.Name, true);
                }
            }
        }

        /// <summary>
        /// 拷贝目录
        /// </summary>
        /// <param name="sourceDir"></param>
        /// <param name="destDir"></param>
        public static void CopyDirectory(string sourceDir, string destDir, bool overwrite)
        {
            // 创建源目录和目标目录
            DirectoryInfo source = new DirectoryInfo(sourceDir);
            DirectoryInfo dest = new DirectoryInfo(destDir);

            // 检查是否出现循环拷贝
            if (dest.FullName.StartsWith(source.FullName, StringComparison.CurrentCultureIgnoreCase))
            {
                throw new Exception("父目录不能拷贝到子目录！");
            }

            // 检查源目录是否存在
            if (!source.Exists) return;
            // 检查目标目录是否存在
            if (!dest.Exists) dest.Create();

            // 获取源目录所有文件并执行拷贝操作
            FileInfo[] files = source.GetFiles();
            for (int i = 0; i < files.Length; i++)
            {
                File.Copy(files[i].FullName, dest.FullName + @"\" + files[i].Name, overwrite);
            }

            // 获取源目录下所有的目录，并执行目录拷贝操作
            DirectoryInfo[] dirs = source.GetDirectories();
            for (int j = 0; j < dirs.Length; j++)
            {
                CopyDirectory(dirs[j].FullName, dest.FullName + @"\" + dirs[j].Name, overwrite);
            }
        }

        /// <summary>
        /// 拷贝目录内容
        /// </summary>
        /// <param name="sourceDir"></param>
        /// <param name="destDir"></param>
        public static void CopyDirectoryContent(string sourceDir, string destDir, bool overwrite)
        {
            // 创建源目录和目标目录
            DirectoryInfo source = new DirectoryInfo(sourceDir);
            DirectoryInfo dest = new DirectoryInfo(destDir);

            // 检查是否出现循环拷贝
            if (dest.FullName.StartsWith(source.FullName, StringComparison.CurrentCultureIgnoreCase))
            {
                throw new Exception("父目录不能拷贝到子目录！");
            }

            // 检查源目录是否存在
            if (!source.Exists) return;
            // 检查目标目录是否存在
            if (!dest.Exists)
            {
                dest.Create();
                FileSystemInfo[] files = source.GetFileSystemInfos();
                foreach (FileSystemInfo file in files)
                {
                    if (file is FileInfo)
                        File.Copy(file.FullName, dest.FullName + @"\" + file.Name, overwrite);
                    else
                        CopyDirectory(file.FullName, dest.FullName + @"\" + file.Name, overwrite);
                }
            }
            else
            {
                FileSystemInfo[] files = source.GetFileSystemInfos();
                foreach (FileSystemInfo file in files)
                {
                    if (overwrite)
                    {
                        if (file is FileInfo)
                            File.Copy(file.FullName, dest.FullName + @"\" + file.Name, overwrite);
                        else
                            CopyDirectory(file.FullName, dest.FullName + @"\" + file.Name, overwrite);
                    }
                    else
                    {
                        string destFullName = dest.FullName + @"\" + file.Name;
                        if (file is FileInfo && !File.Exists(destFullName))
                            File.Copy(file.FullName, dest.FullName + @"\" + file.Name, overwrite);
                        else if (!Directory.Exists(destFullName))
                            CopyDirectory(file.FullName, dest.FullName + @"\" + file.Name, overwrite);
                    }
                }
            }
        }
    }
}

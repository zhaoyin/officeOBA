using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Checksums;
namespace Cooperativeness.FileTransfer.Core
{
    public class SharpZipLibHelper
    {
        private static readonly Logger Log = new Logger(typeof(SharpZipLibHelper));
        private static readonly int Level = 8;
        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="files"></param>
        /// <param name="targetFileName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool Compress(string[] files, string targetFileName, string password)
        {
            //如果已经存在目标文件，询问用户是否覆盖
            if (File.Exists(targetFileName))
            {
                // if (!_ProcessOverwrite(targetFileName))
                Log.Error("Compress执行失败：目标文件已存在！");
                return false;
            }
            try
            {
                using (var outputStream=new FileStream(targetFileName,FileMode.OpenOrCreate,FileAccess.Read))
                {
                    using (var s = new ZipOutputStream(outputStream))
                    {
                    var crc = new Crc32();
                    s.SetLevel(Level);
                    if (!string.IsNullOrEmpty(password))
                    {
                        s.Password = password;
                    }
                    foreach (string file in files)
                    {
                        //打开压缩文件
                        using (FileStream fs = File.OpenRead(file))
                        {
                            var buffer = new byte[fs.Length];
                            fs.Read(buffer, 0, buffer.Length);
                            var entry = new ZipEntry(file);
                            entry.DateTime = DateTime.Now;
                            entry.Size = fs.Length;
                            crc.Reset();
                            crc.Update(buffer);
                            entry.Crc = crc.Value;
                            s.PutNextEntry(entry);
                            s.Write(buffer, 0, buffer.Length);
                            fs.Flush();
                            fs.Close();
                        }
                    }
                    s.Finish();
                    s.Flush();
                        s.Close();
                    }
                    outputStream.Close();
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw new ApplicationException(e.Message);
                return false;
            }
            if (File.Exists(targetFileName)) return true;
            return false;
        }

        #region 压缩文件夹,支持递归
        /// <summary>
        ///　压缩文件夹
        /// </summary>
        /// <param name="dir">待压缩的文件夹</param>
        /// <param name="targetFileName">压缩后文件路径（包括文件名）</param>
        /// <param name="recursive">是否递归压缩</param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool Compress(string dir, string targetFileName, bool recursive,string password)
        {
            //如果已经存在目标文件，询问用户是否覆盖
            if (File.Exists(targetFileName))
            {
                // if (!_ProcessOverwrite(targetFileName))
                Log.Error("Compress执行失败：目标文件已存在！");
                return false;
            }
            var ars = new string[2];
            if (recursive == false)
            {
                //return Compress(dir, targetFileName);
                ars[0] = dir;
                ars[1] = targetFileName;
                return ZipFileDictory(ars,password);
            }
            try
            {
                //open
                using (FileStream zipFile = File.Create(targetFileName))
                {
                    var zipStream = new ZipOutputStream(zipFile);
                    if (!string.IsNullOrEmpty(password))
                    {
                        zipStream.Password = password;
                    }
                    if (!string.IsNullOrEmpty(dir))
                    {
                        _CompressFolder(dir, zipStream, dir.Substring(3));
                    }
                    //close
                    zipStream.Finish();
                    zipStream.Flush();
                    zipStream.Close();
                }
            }
            catch (IOException e)
            {
                Log.Error(e);
                throw new ApplicationException(e.Message);
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw new ApplicationException(e.Message);
            }
            
            if (File.Exists(targetFileName)) return true;
            return false;
        }


        /// <summary>
        /// 压缩目录
        /// </summary>
        /// <param name="args">数组(数组[0]: 要压缩的目录; 数组[1]: 压缩的文件名)</param>
        /// <param name="password"></param>
        private static bool ZipFileDictory(string[] args,string password)
        {
            if (args.Length != 2)
            {
                Log.Error("参数错误！");
                return false;
            }
            ZipOutputStream s = null;
            try
            {
                string[] filenames = Directory.GetFiles(args[0]);
                var crc = new Crc32();
                s = new ZipOutputStream(File.Create(args[1]));
                s.SetLevel(Level);
                if (!string.IsNullOrEmpty(password))
                {
                    s.Password = password;
                }
                foreach (string file in filenames)
                {
                    //打开压缩文件
                    using (FileStream fs = File.OpenRead(file))
                    {
                        var buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, buffer.Length);
                        var entry = new ZipEntry(file);
                        entry.DateTime = DateTime.Now;
                        entry.Size = fs.Length;
                        crc.Reset();
                        crc.Update(buffer);
                        entry.Crc = crc.Value;
                        s.PutNextEntry(entry);
                        s.Write(buffer, 0, buffer.Length);
                        fs.Flush();
                        fs.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw new ApplicationException(e.Message);
                return false;
            }
            finally
            {
                if (s != null)
                {
                    s.Finish();
                    s.Flush();
                    s.Close();
                }
            }
            return true;
        }


        /// <summary>
        /// 压缩某个子文件夹
        /// </summary>
        /// <param name="basePath"></param>
        /// <param name="zips"></param>
        /// <param name="zipfolername"></param>     
        private static void _CompressFolder(string basePath, ZipOutputStream zips, string zipfolername)
        {
            if (File.Exists(basePath))
            {
                _AddFile(basePath, zips, zipfolername);
                return;
            }
            string[] names = Directory.GetFiles(basePath);
            foreach (string fileName in names)
            {
                _AddFile(fileName, zips, zipfolername);
            }
            names = Directory.GetDirectories(basePath);
            foreach (string folderName in names)
            {
                _CompressFolder(folderName, zips, zipfolername);
            }
        }
        /// <summary>
        ///　压缩某个子文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="zips"></param>
        /// <param name="zipfolername"></param>
        private static void _AddFile(string fileName, ZipOutputStream zips, string zipfolername)
        {
            if (File.Exists(fileName))
            {
                _CreateZipFile(fileName, zips, zipfolername);
            }
        }
        /// <summary>
        /// 压缩单独文件
        /// </summary>
        /// <param name="fileToZip"></param>
        /// <param name="zips"></param>
        /// <param name="zipfolername"></param>
        private static void _CreateZipFile(string fileToZip, ZipOutputStream zips, string zipfolername)
        {
            try
            {
                using (var sreamToZip = new FileStream(fileToZip, FileMode.Open, FileAccess.Read))
                {
                    string temp = fileToZip;
                    string temp1 = zipfolername;
                    if (temp1.Length > 0)
                    {
                        int i = temp1.LastIndexOf("\\",StringComparison.CurrentCultureIgnoreCase) + 1;//这个地方原来是个bug用的是"//"，导致压缩路径过长路径2012-7-2
                        int j = temp.Length - i;
                        temp = temp.Substring(i, j);
                    }
                    var zipEn = new ZipEntry(temp.Substring(3));
                    zips.PutNextEntry(zipEn);
                    var buffer = new byte[16384];
                    Int32 size = sreamToZip.Read(buffer, 0, buffer.Length);
                    zips.Write(buffer, 0, size);
                    try
                    {
                        while (size < sreamToZip.Length)
                        {
                            int sizeRead = sreamToZip.Read(buffer, 0, buffer.Length);
                            zips.Write(buffer, 0, sizeRead);
                            size += sizeRead;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                        throw new ApplicationException(ex.Message);
                    }
                    sreamToZip.Flush();
                    sreamToZip.Close();
                }

            }
            catch (Exception e)
            {
                Log.Error(e);
                throw new ApplicationException(e.Message);
            }
        }
        #endregion

        #region
        /// <summary>
        /// 解压缩目录
        /// </summary>
        /// <param name="zipDirectoryPath">压缩目录路径</param>
        /// <param name="unZipDirecotyPath">解压缩目录路径</param>
        /// <param name="password">密码</param>
        public static void DeCompress(string zipDirectoryPath, string unZipDirecotyPath, string password)
        {
            Log.Info("DeCompress-Begin");
            try
            {
                while (unZipDirecotyPath.LastIndexOf("\\", StringComparison.CurrentCultureIgnoreCase) + 1 ==
                       unZipDirecotyPath.Length) //检查路径是否以"\"结尾
                {
                    unZipDirecotyPath = unZipDirecotyPath.Substring(0, unZipDirecotyPath.Length - 1); //如果是则去掉末尾的"\"
                    //Log.Debug("DeCompress-01");
                }
                //Log.Debug("DeCompress-02");
                using (var inputStream = new FileStream(zipDirectoryPath, FileMode.Open, FileAccess.Read))
                {
                    using (var zipStream = new ZipInputStream(inputStream))
                    {
                        //判断Password
                        if (!string.IsNullOrEmpty(password))
                        {
                            zipStream.Password = password;
                        }
                        ZipEntry zipEntry = null;
                        while ((zipEntry = zipStream.GetNextEntry()) != null)
                        {
                            string directoryName = Path.GetDirectoryName(zipEntry.Name);
                            string fileName = Path.GetFileName(zipEntry.Name);
                            if (!string.IsNullOrEmpty(directoryName))
                            {
                                string directoryPath = Path.Combine(unZipDirecotyPath, directoryName);
                                if (!Directory.Exists(directoryPath))
                                {
                                    Directory.CreateDirectory(directoryPath);
                                }
                            }
                            if (!string.IsNullOrEmpty(fileName))
                            {
                                //if (zipEntry.CompressedSize == 0)
                                //    break;
                                if (zipEntry.IsDirectory)//如果压缩格式为文件夹方式压缩
                                {
                                    directoryName = Path.GetDirectoryName(unZipDirecotyPath + @"\" + zipEntry.Name);
                                    if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
                                    {
                                        Directory.CreateDirectory(directoryName);
                                    }
                                }
                                else//2012-5-28修改，支持单个文件压缩时自己创建目标文件夹
                                {
                                     if (!Directory.Exists(unZipDirecotyPath))
                                     {
                                        Directory.CreateDirectory(unZipDirecotyPath);
                                     }
                                }
                                    //Log.Debug("test-def");
                                var singleFileName = unZipDirecotyPath + @"\" + zipEntry.Name;
                                FileUtil.DeleteFile(singleFileName);
                                using (var stream = new FileStream(unZipDirecotyPath + @"\" + zipEntry.Name,
                                                                FileMode.OpenOrCreate, FileAccess.Write))
                                //File.Create(unZipDirecotyPath + @"\" + zipEntry.Name))
                                {
                                    while (true)
                                    {
                                        var buffer = new byte[8192];
                                        int size = zipStream.Read(buffer, 0, buffer.Length);
                                        if (size > 0)
                                        {
                                            stream.Write(buffer, 0, size);
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                    //Log.Debug("test-hjk");
                                    stream.Close();
                                }
                            }
                        }
                        zipStream.Close();
                    }
                    inputStream.Close();
                }

            }
            catch (IOException e)
            {
                Log.Error(e);
                throw new ApplicationException(e.Message,e);
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw new ApplicationException(e.Message,e);
            }
        }

        #endregion

    }
}

using System;
using System.IO;

namespace Cooperativeness.FileTransfer.Core
{
    /// <summary>
    /// Provides modified version for Create File
    /// </summary>
	public sealed class FileUtil
    {
        private static readonly Logger Log = new Logger(typeof(FileUtil));

        public static string[] GetDirectories(string dirPath, string searchPattern)
        {
            try
            {
                if (searchPattern.Trim().Length == 0)
                {
                    return Directory.GetDirectories(dirPath);
                }
                return Directory.GetDirectories(dirPath, searchPattern);
            }
            catch (Exception e)
            {
                Log.Debug(e);
                return null;
            }
        }

        public static string[] GetFiles(string dirPath, string searchPattern,bool isFullPath)
        {
            try
            {
                string[] files = (searchPattern.Trim().Length == 0) ? Directory.GetFiles(dirPath) : Directory.GetFiles(dirPath, searchPattern);
                if (isFullPath)
                {
                    return files;
                }
                if (files.Length == 0) return files;
                var results=new string[files.Length];
                var i = 0;
                foreach (var file in files)
                {
                    var fileinfo = new FileInfo(file);
                    results[i] = fileinfo.Name;
                    i += 1;
                }
                return results;
            }
            catch (Exception e)
            {
                Log.Debug(e);
                return null;
            }
        }

        public static bool CreateFile(string fileName, int filesize)
		{
			FileStream fs  = null;
			try
			{
                if(string.IsNullOrEmpty(fileName)) throw new FileNotFoundException("文件不存在!");
				string directory = Path.GetDirectoryName(fileName);
                if(string.IsNullOrEmpty(directory)) throw new DirectoryNotFoundException("文件路径非法!");
				if (!Directory.Exists(directory))
				{
					Directory.CreateDirectory(directory);
				}

				if(File.Exists(fileName + ".dnl") && File.Exists(fileName) == false)
				{
					File.Delete(fileName + ".dnl");
				}
				fs = new FileStream(fileName, FileMode.Create);
				fs.SetLength(filesize);
				
				return true;
			}
			catch( Exception e)
			{
                Log.Error(e);
				throw;
			}
			finally
			{
				if (fs != null)
				{
					fs.Close();
				}
			}
		}

        public static void DeleteFile(string sFilePath)
        {
            try
            {
                if (File.Exists(sFilePath))
                {
                    File.Delete(sFilePath);
                }
            }
            catch (IOException ex)
            {
                Log.Error(ex);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public static void CreateDirectory(string sPath)
        {
            try
            {
                if (!Directory.Exists(sPath))
                {
                    Directory.CreateDirectory(sPath);
                }
            }
            catch (IOException e)
            {
                Log.Warn(e);
            }
            catch (Exception e)
            {
                Log.Warn(e);
            }
        }

        public static void DeleteDirectory(string sPath)
        {
            try
            {
                if (Directory.Exists(sPath))
                {
                    Directory.Delete(sPath, true);
                }
            }
            catch (IOException ex)
            {
                Log.Warn(ex.Message);
            }
            catch (Exception e)
            {
                Log.Warn(e.Message);
            }
        }

        static readonly string _syncLock = "ThreadingSecurity";
        public static void WriteFile(string filePath, string text)
        {
            lock (_syncLock)
            {
                try
                {
                    if (!File.Exists(filePath))
                    {
                        FileStream fs = File.Create(filePath);
                        fs.Close();
                    }

                    var writer = new StreamWriter(filePath, true, System.Text.Encoding.GetEncoding("gb2312"));
                    writer.WriteLine(text);
                    writer.Close();
                }
                catch (Exception err)
                {
                    Log.Warn(err);
                }
            }
        }

        public static bool SaveAsFile(string sourceFile, string destFile)
        {
            if (!File.Exists(sourceFile)) return false;
            if (File.Exists(destFile)) File.Delete(destFile);
            //using (var writer = new FileStream(destFile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
            //{
            //    using (var reader = new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            //    {
            //        int readlength = 0;
            //        var buffer = new byte[sourceFile.Length];
            //        while ((readlength = reader.Read(buffer, 0, buffer.Length)) > 0)
            //        {
            //            writer.Write(buffer, 0, readlength);
            //            writer.Flush();
            //        }
            //        reader.Flush();
            //        reader.Close();
            //    }
            //    writer.Close();
            //}
            try
            {
                //根据选择来设定分割的小文件的大小
                using (var inputStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read))
                    //以文件的全路对应的字符串和文件打开模式来初始化FileStream文件流实例
                {
                    long filesize = inputStream.Length;
                    using (var reader = new BinaryReader(inputStream)) //以FileStream文件流来初始化BinaryReader文件阅读器
                    {
                        byte[] TempBytes = null;
                        //每次分割读取的最大数据
                        using (var outputStream = new FileStream(destFile, FileMode.OpenOrCreate, FileAccess.Write))
                        {
                            using (var writer = new BinaryWriter(outputStream))
                            {
                                Log.Info("开始写流数据--file:" + destFile + "--size:" + filesize);
                                //long position = 0;
                                while (filesize!=outputStream.Position)
                                {
                                    TempBytes = reader.ReadBytes(BufferSize);
                                    writer.Write(TempBytes);
                                    //position += BufferSize;
                                    TempBytes = null;
                                    //Log.Info("开始写流数据--position:" + position);
                                }
                                Log.Info("写流数据完成--file:" + destFile);
                                writer.Close();
                            }
                            outputStream.Close();
                        }
                        reader.Close();
                    }
                    inputStream.Close();
                }
                return true;
            }
            catch (EndOfStreamException e)
            {
                Log.Error(e);
                return true;
            }
            catch (OutOfMemoryException e)
            {
                Log.Error(e);
                throw new OutOfMemoryException(e.Message);
            }
            catch (IOException e)
            {
                Log.Error(e);
                throw new IOException(e.Message);
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw new Exception(e.Message);
            }
        }


        private static readonly int PositionInfoLength = 3;
        private static readonly int BufferSize = 8192;

        public static string SplitFiles(string inputFile, int chunkSize)
        {
            try
            {
                var result = "";
                //根据选择来设定分割的小文件的大小
                using (var inputStream = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
                    //以文件的全路对应的字符串和文件打开模式来初始化FileStream文件流实例
                {
                    if (chunkSize >= inputStream.Length || chunkSize <= 0)
                    {
                        Log.Error("拆分的文件超出原文件大小或者拆分的文件参数有误！");
                        return Path.GetFileName(inputFile);
                    }
                    using (var reader = new BinaryReader(inputStream)) //以FileStream文件流来初始化BinaryReader文件阅读器
                    {
                        byte[] TempBytes = null;
                        //每次分割读取的最大数据
                        var iFileCount = (int) (inputStream.Length/chunkSize);
                        //小文件总数
                        if (inputStream.Length%chunkSize != 0) iFileCount++;
                        Log.Info("iFileCount:" + iFileCount);
                        for (int i = 1; i <= iFileCount; i++)
                        {
                            string file = PreparePathFileName(inputFile, i);
                            string name = GetNoPathFileName(inputFile, i);
                            using (var outputStream = new FileStream(file, FileMode.OpenOrCreate, FileAccess.Write))
                            {
                                using (var writer = new BinaryWriter(outputStream))
                                {
                                    Log.Info("开始写流数据--file:"+file);
                                    int position = 0;
                                    int modValue = chunkSize % BufferSize;
                                    while (position < chunkSize - modValue)
                                    {

                                        TempBytes = reader.ReadBytes(BufferSize);
                                        writer.Write(TempBytes);
                                        position += BufferSize;
                                        TempBytes = null;
                                    }
                                    if (modValue > 0)
                                    {
                                        TempBytes = reader.ReadBytes(modValue);
                                        writer.Write(TempBytes);
                                    }
                                    result += name + ",";
                                    Log.Info("写流数据完成--file:" + name);
                                    writer.Close();
                                }
                                outputStream.Close();
                            }
                        }
                        reader.Close();
                    }
                    inputStream.Close();
                }
                if (result.EndsWith(",")) result = result.Substring(0, result.Length - 1);
                return result;
            }
            catch (OutOfMemoryException e)
            {
                Log.Error(e);
                throw new OutOfMemoryException(e.Message);
            }
            catch (IOException e)
            {
                Log.Error(e);
                throw new IOException(e.Message);
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw new Exception(e.Message);
            }
        }

        private static string GetNoPathFileName(string filePath, int position)
        {
            return Path.GetFileNameWithoutExtension(filePath) +
                              SplitParameter.Separator +
                              position.ToString().PadLeft(PositionInfoLength, '0') +
                              Path.GetExtension(filePath) +
                              SplitParameter.CutUpExtension;
        }

        private static string PreparePathFileName(string inputFile, int position)
        {
            string fileName = GetNoPathFileName(inputFile,position);
            //为了客户端好下载，老规矩，一律加后缀.txt但是外面得不到这个.txt后缀
            fileName = fileName + ".txt";
            var dir = Path.GetDirectoryName(inputFile);
            if (!string.IsNullOrEmpty(dir))
            {
                return Path.Combine(dir, fileName);
            }
            return fileName;
        }

        public static bool MergeFiles(string outputPath, string searchPattern, bool deleteOrginFile,string mergeFilePath)
        {
            try
            {
                string[] files = GetFilesToMerge(outputPath, searchPattern);
                if (files.Length > 0)
                {
                    if (string.IsNullOrEmpty(mergeFilePath))
                    {
                        var fileName = PrepareFileName(files[0]);
                        mergeFilePath = Path.Combine(outputPath, fileName);
                    }
                    //if (File.Exists(mergeFilePath))
                    //{
                    //    Log.Debug("文件已经存在！");
                    //    return true;
                    //}
                    using (var outputFile = new FileStream(mergeFilePath, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        foreach (string file in files)
                        {
                            int bytesRead = 0;
                            var buffer = new byte[BufferSize];
                            using (var inputTempFile = new FileStream(file, FileMode.OpenOrCreate, FileAccess.Read))
                            {
                                while ((bytesRead = inputTempFile.Read(buffer, 0, BufferSize)) > 0)
                                {
                                    outputFile.Write(buffer, 0, bytesRead);
                                }
                                inputTempFile.Close();
                            }
                            if (deleteOrginFile)
                            {
                                File.Delete(file);
                            }
                        }
                        outputFile.Close();
                    }
                }
                return true;
            }
            catch (OutOfMemoryException e)
            {
                Log.Error(e);
                throw new OutOfMemoryException(e.Message);
            }
            catch (IOException e)
            {
                Log.Error(e);
                throw new IOException(e.Message);
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw new IOException(e.Message);
            }
        }

        private static string[] GetFilesToMerge(string outPutPath, string searchPattern)
        {
            if (!string.IsNullOrEmpty(searchPattern))
            {
                return Directory.GetFiles(outPutPath, "*" + searchPattern + "*" + SplitParameter.CutUpExtension,SearchOption.TopDirectoryOnly);
            }
            return Directory.GetFiles(outPutPath, "*" + SplitParameter.CutUpExtension,SearchOption.TopDirectoryOnly);
        }

        private static string PrepareFileName(string file)
        {
            string fileName = Path.GetFileNameWithoutExtension(file);
            if (!string.IsNullOrEmpty(fileName))
            {
                return fileName.Substring(0, fileName.IndexOf(SplitParameter.Separator, StringComparison.CurrentCultureIgnoreCase)) +
                       Path.GetExtension(fileName);
            }
            return fileName;
        }

    }

    public sealed class SplitParameter
    {
        public const string CutUpExtension = ".ucup";
        public const string Separator = ".";
    }

}
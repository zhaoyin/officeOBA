using System;
using System.IO;
using System.Web.Services.Protocols;
using System.Xml;
using Cooperativeness.FileTransfer.Core;

namespace Cooperativeness.FileTransfer
{
    public enum FaultCode
    {
        Client = 0,
        Server = 1
    }

    public class TransferServices : MarshalByRefObject,  IDisposable
    {
        private static readonly string DefalutNamespace = "http://XPlugin.XX/FileTransfer/";
        public TransferServices()
        {
            Log.Info("TransferServices is initialed!");
        }

        public string ServerPath { get; set; }

        private string _serverFilePath = string.Empty;
        public string ServerFilePath { 
            get { return _serverFilePath; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    FileUtil.CreateDirectory(value);
                    //if (!Directory.Exists(value))
                    //{
                    //    Directory.CreateDirectory(value);
                    //}
                }
                _serverFilePath = value;
            }
        }

        #region  变量

        private static readonly Logger Log = new Logger(typeof(TransferServices));

        #endregion

        #region IDisposable 成员

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #endregion

        private string GetFileFullPath(string filename, string directoryPath)
        {
            string filePath = string.IsNullOrEmpty(directoryPath)
                                     ? ServerFilePath + filename
                                     : ServerPath + directoryPath + @"\\" + filename;
            return filePath;
        }

        private string GetFileDirectory(string directoryPath)
        {
            if (string.IsNullOrEmpty(ServerPath))
            {
                throw RaiseException("GetFileDirectory", "未获取到服务器文件目录！");
            }
            if (string.IsNullOrEmpty(directoryPath) || ServerPath.EndsWith(directoryPath, StringComparison.CurrentCultureIgnoreCase))
            {
                return ServerPath ;
            }
            return ServerPath + directoryPath + @"\\";
        }

        #region TransferServices 成员

        public long GetFileSize(string fileName,string directoryPath)
        {
            string filePath = GetFileFullPath(fileName,directoryPath);
            if (File.Exists(filePath))
            {
                return new FileInfo(filePath).Length ;
            }
            Log.Debug("文件不能存在，无法得到文件大小!");
            return 0;
        }

        public bool DeleteFile(string fileName,string directoryPath)
        {
            try
            {
                string filePath = GetFileFullPath(fileName,directoryPath);
                FileUtil.DeleteFile(filePath);
                //File.Delete(filePath);
                return true;
            }
            catch (IOException e)
            {
                Log.Warn(e);
                throw RaiseException("DeleteDirectory", e.Message);
            }
            catch (Exception e)
            {
                Log.Warn(e);
                throw RaiseException("DeleteDirectory", e.Message);
            }
        }

        public bool DeleteDirectory(string directoryPath)
        {
            try
            {
                if (string.IsNullOrEmpty(ServerPath))
                {
                    throw RaiseException("DeleteDirectory", "未设置服务器文件目录！");
                }
                if (string.IsNullOrEmpty(directoryPath) ||
                    ServerPath.EndsWith(directoryPath, StringComparison.CurrentCultureIgnoreCase))
                {
                    throw RaiseException("DeleteDirectory", "不能删除服务器根目录！");
                }
                FileUtil.DeleteDirectory(ServerPath + @"\\" + directoryPath);
                //Directory.Delete(ServerPath + @"\\" + directoryPath, true);
                return true;
            }
            catch (IOException e)
            {
                Log.Warn(e);
                throw RaiseException("DeleteDirectory", e.Message);
            }
            catch (Exception e)
            {
                Log.Warn(e);
                throw RaiseException("DeleteDirectory", e.Message);
            }
        }
        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="localFileList"></param>
        /// <param name="passWord"></param>
        /// <returns></returns>
        public string PackageFiles(string[] localFileList, string passWord)
        {
            try
            {
                if (localFileList.Length == 0)
                {
                    Log.Debug("没有文件可供压缩!");
                    return "";
                }
                string zipFileName = Guid.NewGuid().ToString() + ".zip";
                string zipFileFullPath = ServerFilePath + zipFileName;

                //var zip = new SharpZipLibHelper();
                bool result = SharpZipLibHelper.Compress(localFileList, zipFileFullPath, passWord);
                Log.Debug("Compression : " + zipFileFullPath);
                FileUtil.SaveAsFile(zipFileFullPath, zipFileFullPath + ".txt");
                if (result) return zipFileName;
                return "";
            }
            catch (ApplicationException e)
            {
                Log.Error(e);
                throw RaiseException("PackageFiles", e.Message);
            }
            catch (IOException e)
            {
                Log.Error(e);
                throw RaiseException("PackageFiles", e.Message);
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw RaiseException("PackageFiles", e.Message);
            }

        }

        public string PackageFiles(string locaDirectoryPath, string passWord)
        {
            try
            {
                Log.Debug("PackageFiles---begin");
                string dirPath = GetFileDirectory(locaDirectoryPath);
                Log.Debug("PackageFiles--dirPath:" + dirPath);
                if (!Directory.Exists(dirPath))
                {
                    Log.Error("要压缩的文件目录不存在！");
                    return "";
                }
                //var zip = new SharpZipLibHelper();
                string zipFileName = Guid.NewGuid().ToString() + ".zip";
                string zipFileFullPath = ServerFilePath + zipFileName;
                Log.Debug("PackageFiles---zipFileFullPath:" + zipFileFullPath + "--dirPath:" + dirPath);
                bool result = SharpZipLibHelper.Compress(dirPath, zipFileFullPath, true, passWord);
                FileUtil.SaveAsFile(zipFileFullPath, zipFileFullPath + ".txt");
                if (result) return zipFileName;
                return "";
            }
            catch (IOException e)
            {
                Log.Error(e);
                throw RaiseException("PackageFiles", e.Message);
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw RaiseException("PackageFiles", e.Message);
            }
        }


        public bool UnpackageFiles(string fileName, string localDirectoryPath,string passWord,string unPackagePath)
        {
            try
            {
                string zipFileName = GetFileFullPath(fileName,localDirectoryPath);
                //本机进行解压缩操作
                Log.Debug("begin UnpackageFiles");
                //var zc = new SharpZipLibHelper();

                string path = GetFileDirectory(unPackagePath);
                Log.Debug("UnpackageFiles: " + zipFileName);
                if (!File.Exists(zipFileName))
                {
                    Log.Error("UnpackageFiles :" + zipFileName + " does not exist");
                    return false;
                }
                Log.Debug("UnpackageFiles--Path:" + path);
                //FileUtil.DeleteDirectory(path);
                if (Directory.Exists(path))
                {
                    Log.Info("该解压目录已经存在！请注意！！！path:"+path);
                }
                FileUtil.CreateDirectory(path);
                SharpZipLibHelper.DeCompress(zipFileName, path, passWord);

                Log.Debug("DeCompression : " + zipFileName);
                return true;
            }
            catch (IOException e)
            {
                Log.Error(e);
                throw RaiseException("UnpackageFiles", e.Message);
            }
            catch (SoapException e)
            {
                Log.Error(e);
                throw RaiseException("UnpackageFiles",e.Message);
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw RaiseException("UnpackageFiles", e.Message);
            }
        }

        public bool TestConnectedness()
        {
            return true;
        }

        public string SplitFiles(string remoteFile,string directoryPath, int chunkSize)
        {
            try
            {
                string filePath = GetFileFullPath(remoteFile, directoryPath);
                string txtFile = filePath + ".txt";
                if (File.Exists(txtFile))
                {
                    filePath = txtFile;
                }
                return FileUtil.SplitFiles(filePath, chunkSize);
            }
            catch (IOException e)
            {
                Log.Error(e);
                throw RaiseException("SplitFiles", e.Message);
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw RaiseException("SplitFiles", e.Message);
            }
        }

        public bool MergeFiles(string directoryPath, string searchPattern, bool deleteOrginFile,string mergeFilePath)
        {
            try
            {
                string filePath = string.IsNullOrEmpty(directoryPath)
                                     ? ServerFilePath 
                                     : ServerPath + directoryPath + @"\\" ;
                return FileUtil.MergeFiles(filePath, searchPattern, deleteOrginFile, mergeFilePath);
            }
            catch (IOException e)
            {
                Log.Error(e);
                throw RaiseException("MergeFiles", e.Message);
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw RaiseException("MergeFiles", e.Message);
            }
        }

        public string[] GetDirectories(string dirPath, string searchPattern)
        {
            string sDir = string.IsNullOrEmpty(dirPath) ? ServerFilePath : ServerPath + dirPath + @"\\";
            return FileUtil.GetDirectories(sDir, searchPattern);
        }

        public string[] GetFiles(string dirPath, string searchPattern,bool isFullPath)
        {
            string sDir = string.IsNullOrEmpty(dirPath) ? ServerFilePath : ServerPath + dirPath + @"\\";
            return FileUtil.GetFiles(sDir, searchPattern, isFullPath);
        }

        #endregion

        #region Exception 处理

        public SoapException RaiseException(string uri, string err)
        {
            return RaiseException(uri, err, FaultCode.Server);
        }

        public SoapException RaiseException(string uri, SystemException e, FaultCode code)
        {
            return RaiseException(uri, e.Message, code);
        }

        public SoapException RaiseException(string uri, string errorMessage, FaultCode code)
        {
            return RaiseException(uri, "", errorMessage, "", "", code);
        }

        public SoapException RaiseException(string uri, string webServiceNamespace, string errorMessage, string errorNumber, string errorSource, FaultCode code)
        {
            if (webServiceNamespace.Length == 0)
            {
                webServiceNamespace = DefalutNamespace;
            }

            XmlQualifiedName faultCodeLocation = null;
            switch (code)
            {
                case FaultCode.Client:
                    faultCodeLocation = SoapException.ClientFaultCode;
                    break;
                case FaultCode.Server:
                    faultCodeLocation = SoapException.ServerFaultCode;
                    break;
            }

            var xmlDoc = new XmlDocument();
            XmlNode rootNode = xmlDoc.CreateNode(XmlNodeType.Element, SoapException.DetailElementName.Name, SoapException.DetailElementName.Namespace);

            XmlNode errorNode = xmlDoc.CreateNode(XmlNodeType.Element, "Error", webServiceNamespace);
            XmlNode errorNumberNode = xmlDoc.CreateNode(XmlNodeType.Element, "ErrorNumber", webServiceNamespace);
            errorNumberNode.InnerText = errorNumber;

            XmlNode errorMessageNode = xmlDoc.CreateNode(XmlNodeType.Element, "ErrorMessage", webServiceNamespace);
            errorMessageNode.InnerText = errorMessage;

            XmlNode errorSourceNode = xmlDoc.CreateNode(XmlNodeType.Element, "ErrorSource", webServiceNamespace);
            errorSourceNode.InnerText = errorSource;

            errorNode.AppendChild(errorNumberNode);
            errorNode.AppendChild(errorMessageNode);
            errorNode.AppendChild(errorSourceNode);

            rootNode.AppendChild(errorNode);

            var soapEx = new SoapException(errorMessage, faultCodeLocation, uri, rootNode);

            return soapEx;
        }

        #endregion
    }
}

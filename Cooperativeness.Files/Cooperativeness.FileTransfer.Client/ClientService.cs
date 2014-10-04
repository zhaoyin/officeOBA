using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Serialization;
using Cooperativeness.FileTransfer.Core;
using Cooperativeness.FileTransfer.Downloader;
using Cooperativeness.FileTransfer.Uploader;

namespace Cooperativeness.FileTransfer
{
    public class ClientService : MarshalByRefObject,IClientService, IDisposable
    {
        public ClientService()
        {
            Log.Info("Client is initialed!");
        }

        #region 错误常量

        private const string FileServerIsNotSetting = "没有设置文件服务器，请先设置文件服务器参数!";
        private const string FileNotExist = "文件不存在 ";
        private const string DirectoryNotExist = "文件夹不存在 ";
        private const string RemoteFileNotExist = "远程文件目录不存在 ";
        private const string ForbiddenDelRoot = "不能删除服务器根目录！";
        private const string NotConnectServer="无法连接到文件服务地址。可能原因是:\r\n1.文件服务器未配置；\r\n2.IE设置了代理；\r\n3.文件服务器不可用。";
        private const string FileServerSettingIsError = "所使用的文件服务器信息配置有误!";
        private const string FileForCompessNotExist = "没有文件可供压缩!";
        #endregion

        #region  变量

        private static readonly Logger Log = new Logger(typeof(ClientService));

        private IWebEnvironment _environment;

        private static TransferInfo _transfer = null;

        private bool _supportBorkenResume;
        private bool _supportDebug;

        private static TransferWebServices _fileService = null;

        private TransferWebServices TransferService
        {
            get
            {
                if (_fileService == null)
                {
                    InitializeServer();
                }
                return _fileService;
            }
        }

        #endregion

        #region IDisposable 成员

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #endregion

        #region 公开属性
        
        private int _rate = 0;
        public int TransferRate
        {
            get { return _rate; }
        }

        private string _errmsg = string.Empty;
        public string ErrorMessage
        {
            get { return _errmsg; }
        }


        //private string _serverDir;
        /// <summary>
        /// 本机既是服务器又是客户端情况下设置服务器目录
        /// </summary>
        internal string ServerDir
        {
            get
            {
                //if (string.IsNullOrEmpty(_serverDir))
                //{
                //    _serverDir = Logger.XPluginInstallPath + @"\turbocrm70\storage\";
                //}
                //return _serverDir;
                return TransferService.GetWebServerPath("./");
            }
        }

        #endregion

        #region IClientService 成员

        public bool Upload(string localFile, string destFileName)
        {
            return Upload(localFile, destFileName, 2);
        }

        public bool Upload(string localFile, string destFileName, int chunkSize)
        {
            if (IsLocalMachine)
            {
                return LocalUpload(localFile,ref destFileName);
            }
            try
            {
                if (!File.Exists(localFile))
                {
                    _errmsg = FileNotExist + localFile;
                    throw new ApplicationException(_errmsg);
                }
                var file = new FileInfo(localFile);
                if (destFileName.Trim().Length == 0)
                {
                    destFileName = file.Name + "[__]" + Guid.NewGuid().ToString() + file.Extension + ".txt";
                }
                else
                {
                    destFileName = destFileName + ".txt";
                }
                //string remoteFileName = file.Name + "[__]" + Guid.NewGuid().ToString() + file.Extension + ".txt";
                IsConnected();
                Log.Debug("AddFile name of" + destFileName);
                return InvokeUpload(destFileName, localFile, chunkSize);
            }
            catch (SoapException e)
            {
                Log.Error(e);
                _errmsg = e.Message;
                throw new ApplicationException(e.Message);
            }
            catch (WebException e)
            {
                Log.Error(e);
                _errmsg = e.Message;
                throw new ApplicationException(e.Message);
            }
            catch (Exception e)
            {
                Log.Error(e);
                _errmsg = e.Message;
                throw new ApplicationException(e.Message);
            }
        }

        public string Upload(string localFile, int chunkSize)
        {
            var file = new FileInfo(localFile);
            string remoteFileName = file.Name + "[__]" + Guid.NewGuid().ToString() + file.Extension + ".txt";
            if (Upload(localFile, remoteFileName, chunkSize))
            {
                return remoteFileName;
            }
            return string.Empty;
        }

        public string Upload(string localFile)
        {
            if (File.Exists(localFile))
            {
                long filesize = new FileInfo(localFile).Length;
                if (filesize > 0x7fffffff)
                {
                    return Upload(localFile, 4);
                }
            }
            return Upload(localFile, 2);
        }

        public bool Download(string remoteFile, string destFile)
        {
            return Download(remoteFile, destFile, 5);
        }

        public bool Download(string remoteFile, string destFile, int chunkCount)
        {
            //如果本机
            if (IsLocalMachine)
            {
                return LocalDownload(remoteFile, destFile);
            }

            try
            {
                IsConnected();
                Log.Debug("Call ReadFile with remoteFile: " + remoteFile + " destFile: " + destFile);
                if (string.IsNullOrEmpty(remoteFile) || string.IsNullOrEmpty(destFile))
                {
                    _errmsg = "源文件或目标文件不存在!";
                    throw new ArgumentException(_errmsg);
                }
                long filesize = GetFileSize(remoteFile, "");
                if (filesize > 0x7fffffff) //如果文件大于Int32的最大值
                {
                    string[] parten = remoteFile.Split('.');
                    var result = TransferService.SplitFiles(remoteFile, "", 1610612736); //每1.5G拆成一个文件
                    string[] remoteFileArray = result.Split(',');
                    bool ret = false;
                    var destDir = Path.GetDirectoryName(destFile);
                    if (!string.IsNullOrEmpty(destDir))
                    {
                        foreach (var s in remoteFileArray)
                        {
                            var destSplitFile = Path.Combine(destDir, s);
                            ret = InvokeDownload(s, destSplitFile, (short) chunkCount);
                            if (ret)
                            {
                                DeleteFile(s + ".txt", "");
                            }
                        }
                        if (ret)
                        {
                            return FileUtil.MergeFiles(destDir, parten[0], true, destFile);
                        }
                    }
                }
                return InvokeDownload(remoteFile, destFile, (short)chunkCount);
            }
            catch (SoapException e)
            {
                Log.Error(e);
                _errmsg = e.Message;
                throw new ApplicationException(e.Message);
            }
            catch (Exception e)
            {
                Log.Error(e);
                Log.Debug(e.StackTrace);
                _errmsg = e.Message;
                throw new ApplicationException(e.Message);
            }
        }

        public long GetFileSize(string remotefileName,string serverdirectoryPath)
        {
            //如果本机
            if (IsLocalMachine)
            {
                Log.Info("GetFileSize---本机就是服务器！");
                string fileFullPath = string.IsNullOrEmpty(serverdirectoryPath)
                                                                   ? ServerFileDir + remotefileName
                                                                   : ServerDir + serverdirectoryPath + @"\\" + remotefileName;
                return LocalGetFileSize(fileFullPath);
            }
            try
            {
                IsConnected();
                return TransferService.GetFileSize(remotefileName, serverdirectoryPath);
            }
            catch (SoapException e)
            {
                _errmsg = e.Message;
                return 0;
            }
            catch (Exception e)
            {
                _errmsg = e.Message;
                return 0;
            }
        }

        public bool DeleteFile(string fileName, string serverdirectoryPath)
        {
            //如果本机
            if (IsLocalMachine)
            {
                Log.Info("DeleteFile---本机就是服务器！");
                string fileFullPath = string.IsNullOrEmpty(serverdirectoryPath)
                                                                   ? ServerFileDir+fileName
                                                                   : ServerDir + serverdirectoryPath + @"\\" + fileName;
                Log.Debug("DeleteFile---FilePath:"+fileFullPath);
                return LocalDeleteFile(fileFullPath);

            }
            try
            {
                IsConnected();
                return TransferService.DeleteFile(fileName, serverdirectoryPath);
            }
            catch (SoapException e)
            {
                _errmsg = e.Message;
                return false;
            }
            catch (Exception e)
            {
                _errmsg = e.Message;
                return false;
            }
        }

        public bool DeleteDirectory(string serverdirectoryPath)
        {
            //如果本机
            if (IsLocalMachine)
            {
                Log.Info("DeleteDirectory---本机就是服务器！");

                return LocalDeleteDirectory(serverdirectoryPath);
            }
            try
            {
                IsConnected();
                return TransferService.DeleteDirectory(serverdirectoryPath);
            }
            catch (SoapException e)
            {
                _errmsg = e.Message;
                return false;
            }
            catch (Exception e)
            {
                _errmsg = e.Message;
                return false;
            }
        }

        public string PackageFiles(string[] serverFileList, string passWord)
        {
            //如果本机
            if (IsLocalMachine)
            {
                Log.Info("PackageFiles---本机就是服务器！");
                return LocalPackageFiles(serverFileList, passWord);
            }
            try
            {
                IsConnected();
                return TransferService.PackageFiles(serverFileList, passWord);
            }
            catch (SoapException e)
            {
                _errmsg = e.Message;
                return "";
            }
            catch (Exception e)
            {
                _errmsg = e.Message;
                return "";
            }
        }

        public string PackageFiles(string serverDirectoryPath, string passWord)
        {
            //如果本机
            if (IsLocalMachine)
            {
                Log.Info("PackageFiles---本机就是服务器！");
                return LocalPackageFiles(serverDirectoryPath, passWord);
            }
            try
            {
                IsConnected();
                return TransferService.PackageFilesForDirectory(serverDirectoryPath, passWord);
            }
            catch (SoapException e)
            {
                _errmsg = e.Message;
                return "";
            }
            catch (Exception e)
            {
                _errmsg = e.Message;
                return "";
            }          
        }

        public bool UnpackageFiles(string fileName, string serverDirectoryPath, string passWord, string unPackagePath)
        {
            //如果本机
            if (IsLocalMachine)
            {
                return LocalUnpackageFiles(fileName, serverDirectoryPath, passWord, unPackagePath);
            }
            try
            {
                IsConnected();
                return TransferService.UnpackageFiles(fileName, serverDirectoryPath, passWord, unPackagePath);
            }
            catch (SoapException e)
            {
                _errmsg = e.Message;
                return false;
            }
            catch (Exception e)
            {
                _errmsg = e.Message;
                return false;
            }
        }

        public bool TestConnectedness()
        {
            return IsConnected();
        }

        public void SetServerInfo(string transferinfo)
        {
            Log.Debug(transferinfo);
            _fileService = null;
            var transfer=GetTransferInfo(transferinfo);
            if (transfer == null)
            {
                _errmsg = FileServerIsNotSetting;
                throw new ApplicationException(_errmsg);
            }
            InitFileServerInfo(transfer);
        }

        public bool BackUpFiles(string[] serverFileList, string password, string destFile)
        {
            try
            {
                string result = PackageFiles(serverFileList, password);
                if (!string.IsNullOrEmpty(result))
                {
                    bool downResult = Download(result, destFile);
                    if (downResult)
                    {
                        DeleteFile(result, "");
                        if (!IsLocalMachine)
                        {
                            //本机不是服务器时，服务器端会生成.txt文件方便下载，所以要同时删除.txt
                            DeleteFile(result + ".txt", "");
                        }
                        return true;
                    }
                }
                return false;
            }
            catch (IOException e)
            {
                Log.Error(e);
                _errmsg = e.Message;
                throw new ApplicationException(e.Message);
            }
            catch (Exception e)
            {
                Log.Error(e);
                _errmsg = e.Message;
                throw new ApplicationException(e.Message);
            }
        }

        public bool BackUpFiles(string serverDirectoryPath, string password, string destFile)
        {
            try
            {
                string result = PackageFiles(serverDirectoryPath, password);
                if (!string.IsNullOrEmpty(result))
                {
                    bool downResult = Download(result, destFile);
                    if (downResult)
                    {
                        DeleteFile(result, "");
                        if (!IsLocalMachine)
                        {
                            //本机不是服务器时，服务器端会生成.txt文件方便下载，所以要同时删除.txt
                            DeleteFile(result + ".txt", "");
                        }
                        return true;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(_errmsg))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (IOException e)
            {
                Log.Error(e);
                _errmsg = e.Message;
                throw new ApplicationException(e.Message);
            }
            catch (Exception e)
            {
                Log.Error(e);
                _errmsg = e.Message;
                throw new ApplicationException(e.Message);
            } 
        }

        public bool RestoreFiles(string localFile, string passWord, string unPackagePath)
        {
            string result = Upload(localFile);
            if (!string.IsNullOrEmpty(result))
            {
                bool ret=UnpackageFiles(result, "", passWord, unPackagePath);
                if (ret)
                {
                    DeleteFile(result, "");
                    return true;
                }
            }
            return false;
        }

        public string[] GetDirectories(string dirPath, string searchPattern)
        {
            //如果本机
            if (IsLocalMachine)
            {
                return LocalGetDirectories(dirPath, searchPattern);
            }
            try
            {
                IsConnected();
                return TransferService.GetDirectories(dirPath, searchPattern);
            }
            catch (SoapException e)
            {
                _errmsg = e.Message;
                return null;
            }
            catch (Exception e)
            {
                _errmsg = e.Message;
                return null;
            }
        }

        public string[] GetFiles(string dirPath, string searchPattern,bool isFullPath)
        {
            //如果本机
            if (IsLocalMachine)
            {
                return LocalGetFiles(dirPath, searchPattern, isFullPath);
            }
            try
            {
                IsConnected();
                return TransferService.GetFiles(dirPath, searchPattern,isFullPath);
            }
            catch (SoapException e)
            {
                _errmsg = e.Message;
                return null;
            }
            catch (Exception e)
            {
                _errmsg = e.Message;
                return null;
            }
        }

        #endregion

        #region 内部成员

        internal string HostName { get; set; }
        internal int HostPort { get;  set; }
        internal string ProtocolType { get; set; }
        internal string VirtualDirectory { get;  set; }
        internal string FileDirectory { get; set; }
        internal string UploadPostUrl { get; set; }
        protected string HostUrl { get; set; }

        internal void InitFileServerInfo(TransferInfo transferInfo)
        {
            HostName = transferInfo.FileServerInfo.HostName;
            if (string.IsNullOrEmpty(HostName))
            {
                throw new ApplicationException(FileServerIsNotSetting);
            }

            string localname = Dns.GetHostName();
            _isLocalMachine = (HostName.ToLower() == localname.ToLower());

            VirtualDirectory = transferInfo.FileServerInfo.VirtualDirectory;
            FileDirectory = transferInfo.FileServerInfo.FileDirectory;
            FileDirectory = string.IsNullOrEmpty(FileDirectory) ? "Accessories" : FileDirectory;
            ProtocolType = transferInfo.FileServerInfo.Connect;
            UploadPostUrl = transferInfo.FileServerInfo.UploadPostUrl;
            HostPort = int.Parse(transferInfo.FileServerInfo.Port);
            if (HostPort <= 0)
            {
                switch (ProtocolType.ToUpper())
                {
                    case "HTTP":
                        HostPort = 80;
                        break;
                    case "FTP":
                        HostPort = 21;
                        break;
                }
                HostUrl=ProtocolType + @"://" + HostName;
            }
            else
            {
                HostUrl = ProtocolType + @"://" + HostName + ":" + HostPort;
            }
           
            ProtocolType = transferInfo.FileServerInfo.Connect;

            bool isUseProxy = TypeConvert.ToBool(transferInfo.FileServerProxy.IsUsed,false);

            _supportDebug = TypeConvert.ToBool(transferInfo.FileServerInfo.SupportDebug, true);
            _supportBorkenResume = TypeConvert.ToBool(transferInfo.FileServerInfo.SupportBrokenResume, true);

            if (isUseProxy)
            {
                if (_environment == null) _environment = new WebEnvironment();

                _environment.UseProxy = true;
                _environment.ProxyUrl = transferInfo.FileServerProxy.UriAddress;
                int proxyPort = Int32.Parse(transferInfo.FileServerProxy.Port);
                if (proxyPort != 80 && proxyPort != 21)
                {
                    _environment.ProxyUrl += ":" + proxyPort;
                }
            }

            bool isUserAuth = TypeConvert.ToBool(transferInfo.FileServerAuth.IsUsed,false);
            if (isUserAuth)
            {
                if (_environment == null) _environment = new WebEnvironment();
                if (isUseProxy)
                {
                    _environment.ProxyUsername = transferInfo.FileServerAuth.UserName;
                    _environment.ProxyPassword = transferInfo.FileServerAuth.Password;
                }
                else
                {
                    _environment.Username = transferInfo.FileServerAuth.UserName;
                    _environment.Password = transferInfo.FileServerAuth.Password;
                }
            }
        }

        internal TransferInfo GetTransferInfo(string transferinfo)
        {
            if (_transfer == null)
            {
                var serializer = new XmlSerializer(typeof(TransferInfo));
                if (serializer.CanDeserialize(new XmlTextReader(new StringReader(transferinfo))))
                {
                    try
                    {
                        _transfer = (TransferInfo)serializer.Deserialize(new StringReader(transferinfo));
                    }
                    catch (Exception e)
                    {
                        _errmsg = e.Message;
                        throw new ApplicationException(e.Message);
                    }
                }
                else
                {
                    _errmsg = FileServerSettingIsError;
                    throw new ApplicationException(_errmsg);
                }
            }
            if (_transfer.FileServerInfo == null) _transfer = null;
            return _transfer;
         }

        internal string ServerFileDir
        {
            get
            {
                string dirpath = ServerDir + FileDirectory + @"\";
                FileUtil.CreateDirectory(dirpath);
                //if (!Directory.Exists(dirpath))
                //{
                //    Directory.CreateDirectory(dirpath);
                //}
                return dirpath;
                //return TransferService.GetWebServerPath("./"+FileDirectory);
            }
        }

        internal string UploadUrl
        {
            get
            {
                string filePathPrefix = "";
                if (!string.IsNullOrEmpty(VirtualDirectory))
                {
                    filePathPrefix = @"/" + VirtualDirectory + @"/";
                }
                if (!string.IsNullOrEmpty(UploadPostUrl))
                {
                    if (UploadPostUrl.IndexOf("HTTP://",StringComparison.CurrentCultureIgnoreCase) < 0)
                    {
                        return HostUrl + @"/" +filePathPrefix+ UploadPostUrl;
                    }
                    return UploadPostUrl;
                }
                return HostUrl + @"/" + filePathPrefix;
            }
        }

        internal string DownloadDir
        {
            get
            {
                string filePathPrefix = "";
                if (!string.IsNullOrEmpty(VirtualDirectory))
                {
                    filePathPrefix = @"/" + VirtualDirectory + @"/";
                }
                if (!string.IsNullOrEmpty(FileDirectory))
                {
                    filePathPrefix = filePathPrefix + @"/" + FileDirectory + @"/";
                }

                switch (ProtocolType.ToUpper())
                {
                    case "HTTP":
                        return HostUrl + @"/" + filePathPrefix;
                    case "FTP":
                        return HostUrl;
                }
                return HostUrl + @"/" + filePathPrefix;
            }
        }

        internal void InitializeServer()
        {
            CheckUrl();
            _fileService = new TransferWebServices(){};
            _fileService.Timeout = 1000 * 60 * 60; //服务超时设为1小时
            _fileService.Url = WebServicesUri;
            Log.Debug(_fileService.Url);
            _fileService.SetWebServicesDirectory(string.IsNullOrEmpty(VirtualDirectory)?"":"./");
            _fileService.SetWebServicesFileDirectory(string.IsNullOrEmpty(FileDirectory)?"./Accessories":"./"+FileDirectory);
            if (_environment != null)
            {
                Log.Info("Set the proxy");
                _fileService.Proxy = _environment.WebProxy;
                _fileService.Proxy.Credentials = _environment.ProxyCredentials;
            }

        }

        internal void CheckUrl()
        {
            if (HostUrl == string.Empty)
            {
                _errmsg = FileServerIsNotSetting;
                throw new ApplicationException(_errmsg);
            }
        }

        internal string WebServicesUri
        {
            get
            {
                return "http://" + HostName + ":" + HostPort + "/"+VirtualDirectory+"/TransferWebServices.asmx";
            }
        }

        private bool _isLocalMachine;
        /// <summary>
        /// 本机是否既是客户端又是服务器
        /// </summary>
        internal bool IsLocalMachine
        {
            get { return false; } // _isLocalMachine; }
        }

        #endregion

        #region 私有成员

        public string[] LocalGetDirectories(string dirPath, string searchPattern)
        {
            string sDir = string.IsNullOrEmpty(dirPath) ? ServerFileDir : ServerDir + dirPath + @"\";
            return FileUtil.GetDirectories(sDir, searchPattern);
        }

        public string[] LocalGetFiles(string dirPath, string searchPattern,bool isFullPath)
        {
            string sDir = string.IsNullOrEmpty(dirPath) ? ServerFileDir : ServerDir + dirPath + @"\";
            return FileUtil.GetFiles(sDir, searchPattern,isFullPath);
        }

        private bool LocalUnpackageFiles(string fileName, string serverDirectoryPath, string passWord,
                                         string unPackagePath)
        {
            try
            {
                Log.Info("LocalUnpackageFiles---本机就是服务器！");
                string fileFullPath = string.IsNullOrEmpty(serverDirectoryPath)
                                                                   ? ServerFileDir + fileName
                                                                   : ServerDir + serverDirectoryPath + @"\\" + fileName;
                if (!File.Exists(fileFullPath))
                {
                    Log.Error("该文件不存在---FilePath:" + fileFullPath);
                    return false;
                }
                string zipFileName = fileFullPath;
                //本机进行解压缩操作
                Log.Debug("begin UnpackageFiles");

                string path = string.IsNullOrEmpty(unPackagePath) ? ServerFileDir : ServerDir + unPackagePath + @"\\";
                Log.Debug("UnpackageFiles: " + zipFileName);
                if (!File.Exists(zipFileName))
                {
                    _errmsg = "UnpackageFiles :" + zipFileName + " does not exist";
                    Log.Error(_errmsg);
                    return false;
                }
                //var zc = new SharpZipLibHelper();
                SharpZipLibHelper.DeCompress(zipFileName, path, passWord);

                Log.Debug("DeCompression : " + zipFileName);
                return true;

            }
            catch (IOException e)
            {
                Log.Error(e);
                _errmsg = e.Message;
                throw new ApplicationException(e.Message);
            }
            catch (Exception e)
            {
                Log.Error(e);
                _errmsg = e.Message;
                throw new ApplicationException(e.Message);
            }
        }

        private string LocalPackageFiles(string serverDirectoryPath, string passWord)
        {
            string dirFullPath = string.IsNullOrEmpty(serverDirectoryPath)
                                                                               ? ServerDir
                                                                               : ServerDir + serverDirectoryPath + @"\\";
            try
            {
                Log.Info("LocalPackageFiles--dirFullPath:"+dirFullPath);
                if (!Directory.Exists(dirFullPath))
                {
                    Log.Error("该文件夹不存在---dirFullPath:" + dirFullPath);
                    return "";
                }
                //var zip = new SharpZipLibHelper();
                string zipFileName = Guid.NewGuid().ToString() + ".zip";
                string zipFileFullPath = ServerFileDir + zipFileName;
                bool result = SharpZipLibHelper.Compress(dirFullPath, zipFileFullPath, true, passWord);
                //FileUtil.SaveAsFile(zipFileFullPath, zipFileFullPath + ".txt"); //本地目录无需加.txt
                if (result) return zipFileName;
                return "";
            }
            catch (ApplicationException e)
            {
                Log.Error(e);
                _errmsg = e.Message;
                throw new ApplicationException(e.Message);
            }
            catch (IOException e)
            {
                Log.Error(e);
                _errmsg = e.Message;
                throw new ApplicationException(e.Message);
            }
            catch (Exception e)
            {
                Log.Error(e);
                _errmsg = e.Message;
                throw new ApplicationException(e.Message);
            }
        }

        private string LocalPackageFiles(string[] serverFileList, string passWord)
        {
            try
            {
                if (serverFileList.Length == 0)
                {
                    _errmsg = FileForCompessNotExist;
                    Log.Debug(_errmsg);
                    return "";
                }
                string zipFileName = Guid.NewGuid().ToString() + ".zip";
                string zipFileFullPath = ServerFileDir + zipFileName;
                //var zip = new SharpZipLibHelper();
                bool result = SharpZipLibHelper.Compress(serverFileList, zipFileFullPath, passWord);
                Log.Debug("Compression : " + zipFileFullPath);
                //FileUtil.SaveAsFile(zipFileFullPath, zipFileFullPath + ".txt"); //本地目录无需加.txt
                if (result) return zipFileName;
                return "";

            }
            catch (IOException e)
            {
                Log.Error(e);
                _errmsg = e.Message;
                throw new ApplicationException(e.Message);
            }
            catch (Exception e)
            {
                Log.Error(e);
                _errmsg = e.Message;
                throw new ApplicationException(e.Message);
            }
        }

        private long LocalGetFileSize(string filePath)
        {
            if (File.Exists(filePath))
            {
                return new FileInfo(filePath).Length ;
            }
            return 0;
        }

        private bool LocalDownload(string remoteFile, string destFile)
        {
            try
            {
                if (!Directory.Exists(ServerFileDir))
                {
                    _errmsg = RemoteFileNotExist + ServerFileDir;
                    Log.Error(_errmsg);
                    throw new DirectoryNotFoundException(_errmsg);
                }
                string remoteFileFullPath = ServerFileDir + remoteFile;
                FileUtil.SaveAsFile(remoteFileFullPath, destFile);
                return true;
            }
            catch (DirectoryNotFoundException e)
            {
                Log.Error(e);
                throw new ApplicationException(e.Message);
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw new ApplicationException(e.Message);
            }
        }

        private bool LocalUpload(string localFile,ref string destFileName)
        {
            try
            {
                if (!File.Exists(localFile))
                {
                    _errmsg = FileNotExist + localFile;
                    throw new ApplicationException(_errmsg);
                }
                var file = new FileInfo(localFile);
                if (destFileName.Trim().Length == 0)
                {
                    destFileName = file.Name+"_"+ Guid.NewGuid().ToString() + file.Extension;
                }
                //string remoteFile = Guid.NewGuid().ToString() + file.Extension;
                FileUtil.CreateDirectory(ServerFileDir);
                //if (!Directory.Exists(ServerFileDir))
                //{
                //    Directory.CreateDirectory(ServerFileDir);
                //}
                string remoteFileFullPath = ServerFileDir + destFileName;
                File.Copy(localFile, remoteFileFullPath, true);
                return true;
            }
            catch (IOException e)
            {
                Log.Error(e);
                _errmsg = e.Message;
                throw new ApplicationException(e.Message);
            }
            catch (Exception e)
            {
                Log.Error(e);
                _errmsg = e.Message;
                throw new ApplicationException(e.Message);
            }
        }

        private bool LocalDeleteDirectory(string serverdirectoryPath)
        {
            try
            {
                //必须有内容
                if (string.IsNullOrEmpty(serverdirectoryPath))
                {
                    _errmsg = ForbiddenDelRoot;
                    Log.Error(_errmsg);
                    return false;
                }
                string dirFullPath = ServerDir + serverdirectoryPath + @"\\";
                if (!Directory.Exists(dirFullPath))
                {
                    Log.Error("该文件夹不存在---FilePath:" + dirFullPath);
                    _errmsg = DirectoryNotExist + dirFullPath;
                    return false;
                }
                FileUtil.DeleteDirectory(dirFullPath);
                return true;
            }
            catch (IOException e)
            {
                Log.Error(e);
                _errmsg = e.Message;
                throw new ApplicationException(e.Message);
            }
            catch (Exception e)
            {
                Log.Error(e);
                _errmsg = e.Message;
                throw new ApplicationException(e.Message);
            }
        }

        private bool LocalDeleteFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Log.Error("该文件不存在---FilePath:" + filePath);
                    _errmsg = FileNotExist + filePath;
                    return false;
                }
                FileUtil.DeleteFile(filePath);
                //File.Delete(filePath);
                return true;
            }
            catch (IOException e)
            {
                Log.Error(e);
                _errmsg = e.Message;
                throw new ApplicationException(e.Message);
            }
            catch (Exception e)
            {
                Log.Error(e);
                _errmsg = e.Message;
                throw new ApplicationException(e.Message);
            }
        }

        private bool InvokeUpload(string destFileName, string localFileName, int chunkSize)
        {
            WebUploader uploader = UploaderFactory.GetWebUploader(new TransferParameter() { ChunkSize = chunkSize, Environment = _environment, LocalFile = localFileName, RemoteFile = destFileName, TransferUrl = UploadUrl, SupportBrokenResume = _supportBorkenResume, SupportDebug = _supportDebug,FileDirectory = FileDirectory});
            if (uploader != null)
            {
                try
                {
                    bool hasFinished = false;
                    bool hasException = false;
                    uploader.Completed += new CompletedEventHandler(OnCompleted);
                    uploader.Completed += new CompletedEventHandler(delegate(object sender, CompletedEventArgs args) { hasFinished = true; });
                    uploader.Progress += new ProgressEventHandle(OnProgress);
                    uploader.ExceptionError += new ExceptionEventHandle(delegate(object sender, ExceptionEventArgs args) { hasException = hasFinished = true; });
                    uploader.ExceptionError+=new ExceptionEventHandle(OnException);
                    uploader.Start();
                    while (!hasFinished)
                    {
                        Thread.Sleep(100);
                    }

                    if (hasException)
                    {
                        Log.Debug("文件上传遇到问题");
                        _errmsg = uploader.InnerException.Message;
                        throw uploader.InnerException;
                    }
                    return true;

                }
                catch (Exception ex)
                {
                    _errmsg = ex.Message;
                    throw new Exception("InvokeUpload error");
                }

            }
            return false;
        }

        private bool InvokeDownload(string remoteFile, string destFile, short chunkCount)
        {
            string downloadUrl = DownloadDir + remoteFile + ".txt";
            WebDownloader downloader = DownloaderFactory.GetDownloader(new TransferParameter() { ChunkCount = chunkCount, Environment = _environment, TransferUrl = downloadUrl, LocalFile = destFile, RemoteFile = remoteFile, SupportBrokenResume = _supportBorkenResume, SupportDebug = _supportDebug });
            try
            {
                bool hasFinished = false;
                downloader.Completed += new CompletedEventHandler(OnCompleted);
                downloader.Completed +=
                    new CompletedEventHandler(
                        delegate(object sender, CompletedEventArgs args) { hasFinished = true; });
                downloader.ExceptionError +=
                    new ExceptionEventHandle(
                        delegate(object sender, ExceptionEventArgs args) { hasFinished = true; });
                downloader.ExceptionError+=new ExceptionEventHandle(OnException);
                downloader.Progress += new ProgressEventHandle(OnProgress);
                downloader.Start();
                while (!hasFinished)
                {
                    Thread.Sleep(100);
                }
                return true;
            }
            catch (Exception ex)
            {
                _errmsg = ex.Message;
                throw new ApplicationException(ex.Message);
            }

        }

        private void OnCompleted(object sender, CompletedEventArgs args)
        {
            Log.Info("文件处理完毕:" + args.FileName);
        }

        private void OnException(object sender, ExceptionEventArgs args)
        {
            _errmsg = args.Message;
            Log.Debug("处理异常："+args.Message);
        }

        private void OnProgress(object sender, ProgressEventArgs args)
        {
            _rate = args.Rate;
            Log.Debug("文件处理进度"+args.Rate);
        }

        /// <summary>
        /// 判断是否能连接到文件服务器，若不能则产生“无法连接到文件服务器”的异常。
        /// </summary>
        /// <returns></returns>
        private bool IsConnected()
        {
            bool isConnected = false;
            try
            {
                isConnected= TransferService.TestConnectedness();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            if (!isConnected)
            {
                _errmsg = NotConnectServer;
                throw new Exception(_errmsg);
            }
            return true;
        }


        #endregion
    }
}

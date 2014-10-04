using System.ComponentModel;
using System.Web.Services;
using Cooperativeness.FileTransfer.Core;

namespace Cooperativeness.FileTransfer
{

    /// <summary>
    /// Transfer 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://XPlugin.XX/FileTransfer/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class TransferWebServices : WebService
    {
        private static readonly Logger Log = new Logger(typeof(TransferWebServices));

        private static TransferServices _services = null;
        public TransferWebServices()
        {
            if (_services == null)
            {
                _services = new TransferServices();
            }
        }

        [WebMethod]
        public string GetWebServerPath(string directoryPath)
        {
            string filepath = (string.IsNullOrEmpty(directoryPath)) ? "./" : directoryPath + "/";
            return Server.MapPath(filepath);
        }

        [WebMethod]
        public void SetWebServicesFileDirectory(string directoryName)
        {
            Log.Debug("SetWebServicesDirectory--directoryName:" + directoryName);
            directoryName = string.IsNullOrEmpty(directoryName) ? "./" : directoryName + "/";
            _services.ServerFilePath = Server.MapPath(directoryName);
            Log.Debug("SetWebServicesDirectory--Server.MapPath--end--ServerPath:" + _services.ServerFilePath);
        }

        [WebMethod]
        public void SetWebServicesDirectory(string directoryName)
        {
            Log.Debug("SetWebServicesDirectory--directoryName:" + directoryName);
            directoryName = string.IsNullOrEmpty(directoryName) ? "./" : directoryName + "/";
            _services.ServerPath = Server.MapPath(directoryName);
            Log.Debug("SetWebServicesDirectory--Server.MapPath--end--ServerPath:" + _services.ServerPath);
        }

        [WebMethod]
        public long GetFileSize(string fileName,string directoryPath)
        {
            return _services.GetFileSize(fileName,directoryPath);
        }

        [WebMethod]
        public bool DeleteFile(string remoteFile, string directoryPath)
        {
            return _services.DeleteFile(remoteFile,directoryPath);
        }

        [WebMethod]
        public bool DeleteDirectory(string directoryPath)
        {
            return _services.DeleteDirectory(directoryPath);
        }

        [WebMethod]
        public bool TestConnectedness()
        {
            return _services.TestConnectedness();
        }

        [WebMethod]
        public string PackageFiles(string[] localFileList,string passWord)
        {
            return _services.PackageFiles(localFileList,passWord);
        }

        [WebMethod]
        public string PackageFilesForDirectory(string locaDirectoryPath, string passWord)
        {
            return _services.PackageFiles(locaDirectoryPath,passWord);
        }

        [WebMethod]
        public bool UnpackageFiles(string fileName, string localDirectoryPath, string passWord,string unPackagePath)
        {
            return _services.UnpackageFiles(fileName, localDirectoryPath, passWord, unPackagePath);
        }

        [WebMethod]
        public string SplitFiles(string remotefile,string directoryPath, int chunkSize)
        {
            return _services.SplitFiles(remotefile,directoryPath, chunkSize);
        }

        [WebMethod]
        public bool MergeFiles(string directoryPath, string searchPattern, bool deleteOrginFile,string mergeFilePath)
        {
            return _services.MergeFiles(directoryPath, searchPattern, deleteOrginFile, mergeFilePath);
        }

        [WebMethod]
        public string[] GetDirectories(string dirPath, string searchPattern)
        {
            return _services.GetDirectories(dirPath, searchPattern);
        }

        [WebMethod]
        public string[] GetFiles(string dirPath, string searchPattern, bool isFullPath)
        {
            return _services.GetFiles(dirPath, searchPattern, isFullPath);
        }
    }
}

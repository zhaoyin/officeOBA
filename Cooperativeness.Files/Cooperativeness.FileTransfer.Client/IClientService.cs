
namespace Cooperativeness.FileTransfer
{
    interface IClientService
    {
        /// <summary>
        /// 返回当前目录的子目录。
        /// </summary>
        /// <param name="dirPath"></param>
        /// <param name="searchPattern"></param>
        /// <returns></returns>
        string[] GetDirectories(string dirPath, string searchPattern);
        /// <summary>
        /// 返回当前目录的文件列表。
        /// </summary>
        /// <param name="dirPath"></param>
        /// <param name="searchPattern"></param>
        /// <param name="isFullPath"></param>
        /// <returns></returns>
        string[] GetFiles(string dirPath, string searchPattern,bool isFullPath);
        /// <summary>
        /// 上传本地文件
        /// </summary>
        /// <param name="localFile">本地文件</param>
        /// <param name="destFileName">返回服务器端最终上传的结果（文件名）</param>
        /// <returns></returns>
        bool Upload(string localFile, string destFileName);
        /// <summary>
        /// 上传本地文件
        /// </summary>
        /// <param name="localFile">本地文件</param>
        /// <param name="destFileName">返回服务器端最终上传的结果（文件名）</param>
        /// <param name="chunkSize">分块上传，设置每块的大小，单位为M。默认为2M</param>
        /// <returns></returns>
        bool Upload(string localFile, string destFileName, int chunkSize);
        /// <summary>
        /// 上传本地文件
        /// </summary>
        /// <param name="localFile">本地文件</param>
        /// <returns>返回服务器端最终上传的结果（文件名）</returns>
        string Upload(string localFile);
        /// <summary>
        /// 上传本地文件
        /// </summary>
        /// <param name="localFile">本地文件</param>
        /// <param name="chunkSize">分块上传，设置每块的大小，单位为M。默认为2M</param>
        /// <returns>返回服务器端最终上传的结果（文件名）</returns>
        string Upload(string localFile, int chunkSize);
        /// <summary>
        /// 下载服务器端文件
        /// </summary>
        /// <param name="remoteFile">服务器端文件名</param>
        /// <param name="destFile">要下载到本地的文件路径（全路径）</param>
        /// <returns>下载成功与否</returns>
        bool Download(string remoteFile, string destFile);
        /// <summary>
        /// 下载服务器端文件
        /// </summary>
        /// <param name="remoteFile">服务器端文件名</param>
        /// <param name="destFile">要下载到本地的文件路径（全路径）</param>
        /// <param name="chunkCount">分块下载的块数控制，默认分5块下载</param>
        /// <returns>下载成功与否</returns>
        bool Download(string remoteFile, string destFile, int chunkCount);
        /// <summary>
        /// 取得服务器端文件大小
        /// </summary>
        /// <param name="serverfileName">服务器端文件名</param>
        /// <param name="serverdirectoryPath">服务器端目录名</param>
        /// <returns>返回按字节区分的文件大小</returns>
        long GetFileSize(string serverfileName, string serverdirectoryPath);
        /// <summary>
        /// 删除服务器端文件
        /// </summary>
        /// <param name="remoteFile">服务器端文件名</param>
        /// <param name="serverdirectoryPath">服务器端文件目录名</param>
        /// <returns>返回删除结果</returns>
        bool DeleteFile(string remoteFile, string serverdirectoryPath);
        /// <summary>
        /// 删除服务器端目录
        /// </summary>
        /// <param name="serverdirectoryPath">服务器端目录</param>
        /// <returns>返回删除结果</returns>
        bool DeleteDirectory(string serverdirectoryPath);
        /// <summary>
        /// 压缩服务器文件列表
        /// </summary>
        /// <param name="serverFileList">本地文件列表</param>
        /// <param name="passWord">文件压缩的密码</param>
        /// <returns>返回压缩结果</returns>
        string PackageFiles(string[] serverFileList, string passWord);
        /// <summary>
        /// 压缩服务器文件夹
        /// </summary>
        /// <param name="serverDirectoryPath">服务器端文件夹名称（相对于根目录）</param>
        /// <param name="passWord">文件压缩的密码</param>
        /// <returns>返回压缩结果</returns>
        string PackageFiles(string serverDirectoryPath, string passWord);
        /// <summary>
        /// 解压缩服务器端文件到指定路径
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="serverDirectoryPath">文件所在的目录</param>
        /// <param name="passWord">压缩文件的密码</param>
        /// <param name="unPackagePath">解压到的路径</param>
        /// <returns>返回解压缩结果</returns>
        bool UnpackageFiles(string fileName, string serverDirectoryPath, string passWord, string unPackagePath);
        /// <summary>
        /// 备份服务器数据
        /// </summary>
        /// <param name="serverFileList">服务器端文件列表</param>
        /// <param name="password">压缩文件的密码</param>
        /// <param name="destFile">本地目标文件</param>
        /// <returns></returns>
        bool BackUpFiles(string[] serverFileList, string password, string destFile);
        /// <summary>
        /// 备份服务器数据
        /// </summary>
        /// <param name="serverDirectoryPath">服务器端文件夹名称（相对于根目录）</param>
        /// <param name="password">密码</param>
        /// <param name="destFile">本地目标文件</param>
        /// <returns></returns>
        bool BackUpFiles(string serverDirectoryPath, string password, string destFile);
        /// <summary>
        /// 还原服务器数据
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="passWord"></param>
        /// <param name="unPackagePath"></param>
        /// <returns></returns>
        bool RestoreFiles(string localFile,  string passWord, string unPackagePath);
        /// <summary>
        /// 测试是否连接成功
        /// </summary>
        /// <returns></returns>
        bool TestConnectedness();
        /// <summary>
        /// 设置传输信息
        /* <transferinfo>
	        <fileserverinfo hostname='' port='' connect='' virtualdirectory='' filedirectory='' uploadposturl='' brokenresume='' debug='' />
	        <fileserverproxy isused='' uriaddress='' port='' />
	        <fileserverauth isused='' username='' password='' />
        </transferinfo>
         */
        /// </summary>
        /// <param name="transferinfo"></param>
        void SetServerInfo(string transferinfo);
    }
}

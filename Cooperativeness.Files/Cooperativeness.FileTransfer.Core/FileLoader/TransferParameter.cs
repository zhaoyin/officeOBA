
namespace Cooperativeness.FileTransfer.Core
{
    public class TransferParameter
    {
        /// <summary>
        /// 文件服务存放文件的文件夹名称
        /// </summary>
        public string FileDirectory { get; set; }

        public string TransferUrl { get; set; }

        public string RemoteFile { get; set; }

        public string LocalFile { get; set; }

        public int ChunkSize { get; set; }

        public bool SupportBrokenResume { get; set; }

        public bool SupportDebug { get; set; }

        public short ChunkCount
        {
            get { return _chunkCount; }
            set
            {
                _chunkCount = value;
                if (_chunkCount <= 0) _chunkCount = 1;
            }
        }
        private short _chunkCount = 0;

        public IWebEnvironment Environment { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is TransferParameter)) return false;
            var transferParameter = obj as TransferParameter;
            return TransferUrl == transferParameter.TransferUrl &&
                LocalFile == transferParameter.LocalFile &&
                ChunkCount == transferParameter.ChunkCount;
        }

        public override string ToString()
        {
            return string.Format(
            "InputParam\tTransferUrl:{0},SavePath:{1},ChunkCount:{2},UseAuthentication:{3},\r\n" +
            "UseProxy:{4},UseIEProxy:{5},Username:{6},\r\n" +
            "Password{7},ProxyUrl:{8},ProxyBypass:{9},\r\n" +
            "ProxyUsername:{10},ProxyPassword:{11},ProxyDomain:{12}",
            TransferUrl,LocalFile,ChunkCount.ToString(),
            Environment != null ? Environment.UseAuthentication.ToString() : string.Empty,
            Environment != null ? Environment.UseProxy.ToString() : string.Empty,
            Environment != null ? Environment.UseIEProxy.ToString() : string.Empty,
            Environment != null && Environment.Username != null ? Environment.Username : string.Empty,
            Environment != null && Environment.Password != null ? Environment.Password : string.Empty,
            Environment != null && Environment.ProxyUrl != null ? Environment.ProxyUrl : string.Empty,
            Environment != null && Environment.ProxyBypass != null ? Environment.ProxyBypass : string.Empty,
            Environment != null && Environment.ProxyUsername != null ? Environment.ProxyUsername : string.Empty,
            Environment != null && Environment.ProxyPassword != null ? Environment.ProxyPassword : string.Empty,
            Environment != null && Environment.ProxyDomain != null ? Environment.ProxyDomain : string.Empty
            );
        }
    }
}

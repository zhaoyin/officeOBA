using System.IO;
using AODL.IO;

namespace AODL.Document.Custom
{
    public class PackageCustomFile : ICustomFile
    {
        private readonly IFile _file;

        public PackageCustomFile(string mediaType, IFile file)
        {
            MediaType = mediaType;
            _file = file;
        }

        #region ICustomFile Members

        public string MediaType { get; private set; }

        public string FullPath
        {
            get { return _file.Path; }
        }

        public Stream OpenRead()
        {
            return _file.OpenRead();
        }

        #endregion
    }
}
using System.IO;

namespace AODL.IO
{
    public class PackageReaderFile : IFile
    {
        private readonly IPackageReader _packageReader;
        private readonly string _path;

        public PackageReaderFile(string path, IPackageReader packageReader)
        {
            _path = path;
            _packageReader = packageReader;
        }

        #region IFile Members

        public string Name
        {
            get { return System.IO.Path.GetFileName(_path); }
        }

        public string Path
        {
            get { return _path; }
        }

        public Stream OpenRead()
        {
            return _packageReader.Open(_path);
        }

        public void CopyTo(Stream stream)
        {
            using (stream)
            {
                using (Stream sourceStream = OpenRead())
                {
                    byte[] buffer = new byte[1024];
                    while (true)
                    {
                        int read = sourceStream.Read(buffer, 0, buffer.Length);
                        if (read <= 0)
                            return;
                        stream.Write(buffer, 0, read);
                    }
                }
            }
        }

        #endregion
    }
}
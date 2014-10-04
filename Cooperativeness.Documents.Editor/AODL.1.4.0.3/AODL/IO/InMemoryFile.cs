using System.IO;

namespace AODL.IO
{
    public class InMemoryFile : IFile
    {
        private readonly byte[] _contents;
        private readonly string _path;

        public InMemoryFile(string path, byte[] contents)
        {
            _path = path;
            _contents = contents;
        }

        public InMemoryFile(string path, Stream stream)
        {
            _path = path;
            using (stream)
            {
                using (MemoryStream targetStream = new MemoryStream())
                {
                    byte[] buffer = new byte[1024];
                    while (true)
                    {
                        int read = stream.Read(buffer, 0, buffer.Length);
                        if (read <= 0)
                            break;
                        targetStream.Write(buffer, 0, read);
                    }
                    _contents = targetStream.ToArray();
                }
            }
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
            return new MemoryStream(_contents);
        }

        public void CopyTo(Stream stream)
        {
            using (stream)
            {
                stream.Write(_contents, 0, _contents.Length);
            }
        }

        #endregion
    }
}
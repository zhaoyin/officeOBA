using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace AODL.IO
{
    public class InMemoryPackageReader : IPackageReader
    {
        private readonly Stream _odfStream;
        private ZipFile _zipFile;

        public InMemoryPackageReader(Stream odfStream)
        {
            _odfStream = odfStream;
        }

        #region IPackageReader Members

        public void Initialize(string odffilepath)
        {
            _zipFile = new ZipFile(_odfStream);
        }

        public Stream Open(string filepath)
        {
            int entry = _zipFile.FindEntry(filepath.Replace('\\', '/'), true);
            if (entry < 0)
                return Stream.Null;

            return _zipFile.GetInputStream(entry);
        }

        public bool DirectoryExists(string path)
        {
            int entry = _zipFile.FindEntry(path.Replace('\\', '/'), true);

            return (entry >= 0 && _zipFile[entry].IsDirectory);
        }

        public IFile GetFile(string filepath)
        {
            return new InMemoryFile(filepath, Open(filepath));
        }

        public IList<IFile> GetFiles(string directory)
        {
            IList<IFile> files = new List<IFile>();
            if (DirectoryExists(directory))
            {
                directory = directory.Replace('\\', '/');
                if (!directory.EndsWith("/"))
                    directory += '/';
                foreach (ZipEntry entry in _zipFile)
                {
                    if (entry.Name.StartsWith(directory))
                        files.Add(new InMemoryFile(entry.Name, Open(entry.Name)));
                }
            }

            return files;
        }

        public void Dispose()
        {
            try
            {
                if (_zipFile != null)
                    ((IDisposable)_zipFile).Dispose();
            }
            catch (Exception)
            {
            }
        }

        #endregion
    }
}
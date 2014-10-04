using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace AODL.IO
{
    public class OnDiskPackageReader : IPackageReader
    {
        private readonly string _unpackDir;

        public OnDiskPackageReader()
            : this(Path.Combine(Environment.CurrentDirectory, Convert.ToString(Guid.NewGuid())))
        {
        }

        public OnDiskPackageReader(string unpackDir)
        {
            if (string.IsNullOrEmpty(unpackDir))
                throw new ArgumentNullException("unpackDir");
            DirectoryInfo directoryInfo =
                new DirectoryInfo(Path.IsPathRooted(unpackDir)
                                      ? unpackDir
                                      : Path.Combine(Environment.CurrentDirectory, unpackDir));

            _unpackDir = directoryInfo.FullName;
        }

        #region IPackageReader Members

        public void Initialize(string odffilepath)
        {
            if (!Directory.Exists(_unpackDir))
                Directory.CreateDirectory(_unpackDir);

            ZipInputStream s = null;
            try
            {
                s = new ZipInputStream(File.OpenRead(odffilepath));

                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    string directoryName = Path.GetDirectoryName(theEntry.Name);
                    string fileName = Path.GetFileName(theEntry.Name);

                    if (directoryName != String.Empty)
                        Directory.CreateDirectory(Path.Combine(_unpackDir, directoryName));

                    if (fileName != String.Empty)
                    {
                        using (FileStream streamWriter = File.Create(Path.Combine(_unpackDir, theEntry.Name)))
                        {
                            // TODO: Switch this to MemoryStream which is accessible through a package factory
                            byte[] data = new byte[2048];
                            while (true)
                            {
                                int size = s.Read(data, 0, data.Length);
                                if (size <= 0)
                                    break;
                                streamWriter.Write(data, 0, size);
                            }
                        }
                    }
                }
            }
            finally
            {
                s.Close();
            }
        }

        public Stream Open(string filepath)
        {
            return File.OpenRead(Path.Combine(_unpackDir, filepath));
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(Path.Combine(_unpackDir, path));
        }

        public IList<IFile> GetFiles(string directory)
        {
            IList<IFile> files = new List<IFile>();
            DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(_unpackDir, directory));
            if (directoryInfo.Exists)
            {
                foreach (FileInfo fileInfo in directoryInfo.GetFiles())
                {
                    files.Add(new PackageReaderFile(fileInfo.FullName.Substring(_unpackDir.Length + 1), this));
                }
            }

            return files;
        }

        public IFile GetFile(string filepath)
        {
            return new PackageReaderFile(filepath, this);
        }

        public void Dispose()
        {
            try
            {
                if (Directory.Exists(_unpackDir))
                    Directory.Delete(_unpackDir, true);
            }
            catch (Exception)
            {
            }
        }

        #endregion
    }
}
using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace AODL.IO
{
    public class OnDiskPackageWriter : IPackageWriter
    {
        private readonly string _packDir;
        private string _odffilepath;

        public OnDiskPackageWriter()
            : this(Path.Combine(Environment.CurrentDirectory, Convert.ToString(Guid.NewGuid())))
        {
        }

        public OnDiskPackageWriter(string packDir)
        {
            if (string.IsNullOrEmpty(packDir))
                throw new ArgumentNullException("packDir");
            DirectoryInfo directoryInfo =
                new DirectoryInfo(Path.IsPathRooted(packDir)
                                      ? packDir
                                      : Path.Combine(Environment.CurrentDirectory, packDir));

            _packDir = directoryInfo.FullName;
        }

        #region IPackageWriter Members

        public void Initialize(string odffilepath)
        {
            _odffilepath = odffilepath;
            if (Directory.Exists(_packDir))
                Directory.Delete(_packDir, true);
        }

        public Stream Open(string filepath)
        {
            string fullPath = Path.Combine(_packDir, filepath);
            FileInfo fileInfo = new FileInfo(fullPath);
            if (!fileInfo.Directory.Exists)
                fileInfo.Directory.Create();

            return File.OpenWrite(fullPath);
        }

        public bool FileExists(string filepath)
        {
            return File.Exists(Path.Combine(_packDir, filepath));
        }

        public void EndWrite()
        {
            FastZip fz = new FastZip {CreateEmptyDirectories = true};
            fz.CreateZip(_odffilepath, _packDir, true, string.Empty);
        }

        public void Dispose()
        {
            try
            {
                if (Directory.Exists(_packDir))
                    Directory.Delete(_packDir, true);
            }
            catch (Exception)
            {
            }
        }

        #endregion
    }
}
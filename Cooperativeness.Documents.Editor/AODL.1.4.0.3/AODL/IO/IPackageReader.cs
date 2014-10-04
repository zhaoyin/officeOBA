using System;
using System.Collections.Generic;
using System.IO;

namespace AODL.IO
{
    public interface IPackageReader : IDisposable
    {
        void Initialize(string odffilepath);
        Stream Open(string filepath);
        bool DirectoryExists(string path);
        IList<IFile> GetFiles(string directory);
        IFile GetFile(string filepath);
    }
}
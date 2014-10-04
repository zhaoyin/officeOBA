using System;
using System.IO;

namespace AODL.IO
{
    public interface IPackageWriter : IDisposable
    {
        void Initialize(string odffilepath);
        Stream Open(string filepath);
        bool FileExists(string filepath);
        void EndWrite();
    }
}
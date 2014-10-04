using System.IO;

namespace AODL.IO
{
    public interface IFile
    {
        string Name { get; }
        string Path { get; }
        Stream OpenRead();
        void CopyTo(Stream stream);
    }
}
using System.IO;

namespace AODL.Document.Custom
{
    public interface ICustomFile
    {
        string MediaType { get; }
        string FullPath { get; }
        Stream OpenRead();
    }
}
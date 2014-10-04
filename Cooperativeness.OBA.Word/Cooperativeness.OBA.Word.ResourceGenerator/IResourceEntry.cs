using System.Resources;
using System.Xml.Linq;

namespace Cooperativeness.OBA.Word.ResourceGenerator
{
    public interface IResourceEntry
    {
        void Generate(IResourceWriter resourceWrite,XElement element);
    }
}

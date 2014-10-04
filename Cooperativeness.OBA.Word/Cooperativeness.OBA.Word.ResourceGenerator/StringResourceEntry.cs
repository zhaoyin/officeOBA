using System.Resources;
using System.Xml.Linq;

namespace Cooperativeness.OBA.Word.ResourceGenerator
{
    public class StringResourceEntry:AbstractResourceEntry
    {
        protected override void OnGenerate(IResourceWriter resourceWrite, XElement element)
        {
            string id = element.AttibuteStringValue("Id");
            string value = element.AttibuteStringValue("Value");
            if (string.IsNullOrEmpty(id)) return;
            resourceWrite.AddResource(id, value);
        }
    }
}

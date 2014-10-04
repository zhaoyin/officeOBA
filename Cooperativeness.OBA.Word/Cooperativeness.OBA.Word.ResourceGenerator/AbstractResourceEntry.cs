using System.Resources;
using System.Xml.Linq;

namespace Cooperativeness.OBA.Word.ResourceGenerator
{
    public abstract class AbstractResourceEntry : IResourceEntry
    {
        #region 方法

        public static IResourceEntry CreateResourceEntry(XElement element)
        {
            string type = element.AttibuteStringValue("Type");
            switch (type)
            {
                case "string":
                    return new StringResourceEntry();
                default:
                    return null;
            }
        }

        public void Generate(IResourceWriter resourceWrite, XElement element)
        {
            this.OnGenerate(resourceWrite, element);
        }

        protected abstract void OnGenerate(IResourceWriter resourceWrite, XElement element);

        #endregion
    }
}

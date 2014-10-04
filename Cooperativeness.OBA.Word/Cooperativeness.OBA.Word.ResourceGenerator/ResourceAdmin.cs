using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Resources;
using System.IO;
using Cooperativeness.OBA.Word.Tools;

namespace Cooperativeness.OBA.Word.ResourceGenerator
{
    public class ResourceAdmin
    {
        private static readonly Logger Log = new Logger(typeof(ResourceAdmin));
        #region IResourceAdmin 成员

        public static void Generator(string resourceDir, string targetDir)
        {
            try
            {
                var xmlDir = new DirectoryInfo(resourceDir);
                FileInfo[] xmlFiles = xmlDir.GetFiles("*.xml");
                foreach (var fileInfo in xmlFiles)
                {
                    Generate(fileInfo.FullName,targetDir);
                }
            }
            catch (IOException e)
            {
                Log.Debug(e);
            }
            catch (Exception e)
            {
                Log.Debug(e);
            }
        }

        public static void Generate(string resourceXml, string targetResourceDir)
        {
            try
            {
                // 加载多语文件
                XDocument xDocument = XDocument.Load(resourceXml);
                // 获取根目录
                XElement xResourceList = xDocument.Root;
                // 检查是否符合规范
                if (xResourceList != null)
                {
                    if (!xResourceList.Name.LocalName.Equals("ResourceList"))
                        return;
                    // 获取所有的资源列表
                    IEnumerable<XElement> xResources = xResourceList.Elements();
                    if (xResources.Count() > 0)
                    {
                        // 获取资源名称
                        string resourceName = Path.GetFileNameWithoutExtension(resourceXml) + ".resources";
                        if (!Directory.Exists(targetResourceDir))
                        {
                            Directory.CreateDirectory(targetResourceDir);
                        }
                        string path = Path.Combine(targetResourceDir, resourceName);
                        using (IResourceWriter resourceWriter = new ResourceWriter(path))
                        {
                            foreach (XElement xResource in xResources)
                            {
                                IResourceEntry re = AbstractResourceEntry.CreateResourceEntry(xResource);
                                if (re != null) re.Generate(resourceWriter, xResource);
                            }
                            resourceWriter.Generate();
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Log.Debug(e);
            }
        }

        public static void Generate(string resourceXml)
        {
            string path = Path.GetDirectoryName(resourceXml);
            if (!string.IsNullOrEmpty(path))
            {
                Generate(resourceXml,path);
            }
        }

        #endregion
    }
}

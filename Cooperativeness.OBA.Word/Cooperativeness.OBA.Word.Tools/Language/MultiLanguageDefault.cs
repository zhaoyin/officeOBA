using System;
using System.Collections.Specialized;
using System.IO;
using System.Resources;

namespace Cooperativeness.OBA.Word.Tools.Language
{
    /// <summary>
    /// 定义多语资源管理器器对象
    /// </summary>
    public class MultiLanguageDefault : IMultiLanguage
    {
        private static Logger Log=new Logger(typeof(MultiLanguageDefault));
        #region 字段
        private string baseDir;
        private ListDictionary resourcesByCulture;
        private const string ResourceName = "resources\\Resources.{0}.resources";

        #endregion

        #region 构造函数
        public MultiLanguageDefault(string resourceDir)
        {
            if (string.IsNullOrEmpty(resourceDir))
                throw new ArgumentNullException("resourceDir");
            resourcesByCulture = new ListDictionary();
            this.baseDir = resourceDir;
        }

        #endregion

        #region 方法

        /// <summary>
        /// 根据多语名称获取多语值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetString(string name)
        {
            try
            {
                // 检查资源是否存在，如果不存在则加载资源
                string culture = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
                if (!resourcesByCulture.Contains(culture))
                {
                    string path = Path.Combine(baseDir, string.Format(ResourceName, culture));
                    LoadResource(path, culture);
                }
                // 获取资源集合
                var resources = resourcesByCulture[culture] as OrderedDictionary;
                if (resources != null)
                {
                    if (resources.Contains(name)) return resources[name].ToString().Trim();
                }
                return null;
            }
            catch (Exception e)
            {
                Log.Debug(e);
            }

            return name;
        }

        /// <summary>
        /// 根据当前多语名称和语言标识获取多语值
        /// </summary>
        /// <param name="name"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public string GetString(string name, string culture)
        {
            try
            {
                // 检查资源是否存在，如果不存在则加载资源
                if (!resourcesByCulture.Contains(culture))
                {
                    string path = Path.Combine(baseDir, string.Format(ResourceName, culture));
                    LoadResource(path, culture);
                }
                // 获取资源集合
                var resources = resourcesByCulture[culture] as OrderedDictionary;
                if (resources != null)
                {
                    if (resources.Contains(name)) return resources[name].ToString();
                }
                return null;
            }
            catch (Exception e)
            {
                Log.Debug(e);
            }

            return name;
        }

        /// <summary>
        /// 加载指定的资源
        /// </summary>
        /// <param name="resName"></param>
        /// <param name="culture"></param>
        private void LoadResource(string resName, string culture)
        {
            using (IResourceReader rr = new ResourceReader(resName))
            {
                var iter = rr.GetEnumerator();
                while (iter.MoveNext())
                {
                    // 获取字典集合
                    OrderedDictionary resources = null;
                    if (!resourcesByCulture.Contains(culture))
                    {
                        resources = new OrderedDictionary();
                        resourcesByCulture.Add(culture, resources);
                    }
                    else
                    {
                        resources = resourcesByCulture[culture] as OrderedDictionary;
                    }
                    if (resources != null)
                    {
                        // 添加信息
                        resources.Add(iter.Key, iter.Value);
                    }
                }
            }
        }

        #endregion
    }
}

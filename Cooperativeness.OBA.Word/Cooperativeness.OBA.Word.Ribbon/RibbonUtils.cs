using System;
using System.Drawing;
using System.IO;
using Cooperativeness.OBA.Word.AddIns.FrameWork;

namespace Cooperativeness.OBA.Word.Ribbon
{
    /// <summary>
    /// 定义类型助手类
    /// </summary>
    public class RibbonUtils
    {
        /// <summary>
        /// 根据类型名称和插件对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="className"></param>
        /// <param name="bundle"></param>
        /// <returns></returns>
        public static T NewInstance<T>(string className, IBundle bundle)
        {
            Type type = bundle.LoadClass(className);
            if (type != null)
            {
                return (T)Activator.CreateInstance(type);
            }

            return default(T);
        }

        /// <summary>
        /// 根据类型名称和插件对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T NewInstance<T>()
        {
            return (T)Activator.CreateInstance(typeof(T));
        }

        /// <summary>
        /// 获取图片资源
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="mode"></param>
        /// <param name="bundle"></param>
        /// <returns></returns>
        public static Bitmap LoadImage(string resourceName,ResourceLoadMode mode, IBundle bundle)
        {
            using (var stream = (Stream)bundle.LoadResource(resourceName, mode))
            {
                if (stream == null) return null;
                return (Bitmap)Bitmap.FromStream(stream);
            }
        }
    }
}

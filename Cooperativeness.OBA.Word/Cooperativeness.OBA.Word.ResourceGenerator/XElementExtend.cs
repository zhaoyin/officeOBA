using System;
using System.Xml.Linq;
using Cooperativeness.OBA.Word.Tools;

namespace Cooperativeness.OBA.Word.ResourceGenerator
{
    /// <summary>
    /// XML扩展对象
    /// </summary>
    public static class XElementExtend
    {
        private static readonly Logger Log = new Logger(typeof(XElementExtend));
        /// <summary>
        /// 根据指定的属性名获取属性值
        /// </summary>
        /// <param name="element"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public static string AttibuteStringValue(this XElement element, string attributeName)
        {
            if (element == null || string.IsNullOrEmpty(attributeName))
                return null;
            XAttribute attribute = element.Attribute(attributeName);
            if (attribute != null) return attribute.Value;
            return null;
        }

        public static string AttibuteStringValue(this XElement element, string attributeName, string defaultValue)
        {
            if (element == null || string.IsNullOrEmpty(attributeName))
                return defaultValue;
            XAttribute attribute = element.Attribute(attributeName);
            if (attribute != null) return attribute.Value;
            return defaultValue;
        }

        /// <summary>
        /// 根据指定的属性名获取整数类型的属性值
        /// </summary>
        /// <param name="element"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public static int AttributeIntValue(this XElement element, string attributeName)
        {
            string attributeValue = element.AttibuteStringValue(attributeName);
            int value;
            if (int.TryParse(attributeValue, out value))
            {
                return value;
            }
            return default(int);
        }

        public static int AttributeIntValue(this XElement element, string attributeName, int defaultValue)
        {
            string attributeValue = element.AttibuteStringValue(attributeName);
            int value;
            if (int.TryParse(attributeValue, out value))
            {
                return value;
            }
            return defaultValue;
        }

        /// <summary>
        /// 根据指定的属性名获取长整数类型的属性值
        /// </summary>
        /// <param name="element"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public static long AttributeLongValue(this XElement element, string attributeName)
        {
            string attributeValue = element.AttibuteStringValue(attributeName);
            long value;
            if (long.TryParse(attributeValue, out value))
            {
                return value;
            }
            return default(long);
        }

        public static long AttributeLongValue(this XElement element, string attributeName, long defaultValue)
        {
            string attributeValue = element.AttibuteStringValue(attributeName);
            long value;
            if (long.TryParse(attributeValue, out value))
            {
                return value;
            }
            return defaultValue;
        }

        /// <summary>
        /// 根据指定的属性名获取布尔类型的属性值
        /// </summary>
        /// <param name="element"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public static bool AttributeBoolValue(this XElement element, string attributeName)
        {
            string attributeValue = element.AttibuteStringValue(attributeName);
            bool value;
            if (bool.TryParse(attributeValue, out value))
            {
                return value;
            }
            return default(bool);
        }

        public static bool AttributeBoolValue(this XElement element, string attributeName, bool defaultValue)
        {
            string attributeValue = element.AttibuteStringValue(attributeName);
            bool value;
            if (bool.TryParse(attributeValue, out value))
            {
                return value;
            }
            return defaultValue;
        }

        /// <summary>
        /// 根据指定的属性名获取布尔类型的属性值
        /// </summary>
        /// <param name="element"></param>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public static T AttributeEnumValue<T>(this XElement element, string attributeName)
        {
            try
            {
                string attributeValue = element.AttibuteStringValue(attributeName);
                T value = (T) Enum.Parse(typeof (T), attributeValue, true);
                return value;
            }
            catch (Exception e)
            {
                Log.Debug(e);
            }
            return default(T);
        }

        public static T AttributeEnumValue<T>(this XElement element, string attributeName, T defaultValue)
        {
            try
            {
                string attributeValue = element.AttibuteStringValue(attributeName);
                T value = (T) Enum.Parse(typeof (T), attributeValue, true);
                return value;
            }
            catch (Exception e)
            {
                Log.Debug(e);
            }
            return defaultValue;
        }
    }
}

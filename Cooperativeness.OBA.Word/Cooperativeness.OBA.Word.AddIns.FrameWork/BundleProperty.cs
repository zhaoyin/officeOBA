using System.Collections;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork
{
    /// <summary>
    /// 定义插件属性对象
    /// </summary>
    public class BundleProperty
    {
        #region 字段
        public Hashtable properties;

        #endregion

        #region 构造函数
        public BundleProperty()
        {
            this.properties = new Hashtable();
        }

        #endregion

        #region 方法
        /// <summary>
        /// 根据用户指定的属性名，获取属性值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object GetProperty(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;
            lock (properties)
            {
                if (properties.ContainsKey(name))
                    return properties[name];
            }

            return null;
        }

        /// <summary>
        /// 根据用户指定的属性名，获取属性值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public T GetProperty<T>(string name)
        {
            try
            {
                object propertyValue = GetProperty(name);
                if (propertyValue != null)
                    return (T)propertyValue;
            }
            catch { }
            return default(T);
        }

        /// <summary>
        /// 将用户指定的属性名，设置属性值
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetProperty(string name, object value)
        {
            if (string.IsNullOrEmpty(name))
                return;
            lock (properties)
            {
                if (properties.ContainsKey(name))
                    properties[name] = value;
                else
                    this.properties.Add(name, value);
            }
        }

        /// <summary>
        /// 移除指定的属性
        /// </summary>
        /// <param name="name"></param>
        public void RemoveProperty(string name)
        {
            if (string.IsNullOrEmpty(name))
                return;
            lock (properties)
            {
                if (properties.ContainsKey(name))
                    properties.Remove(name);
            }
        }

        /// <summary>
        /// 移除所有的属性
        /// </summary>
        public void RemoveAllProperties()
        {
            lock (properties)
            {
                this.properties.Clear();
            }
        }
        #endregion
    }
}

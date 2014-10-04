using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Service
{
    /// <summary>
    /// 定义服务属性对象
    /// </summary>
    public class ServiceProperties : Dictionary<string, object>
    {
        #region 构造函数
        public ServiceProperties(IDictionary<string,object> props)
        {
            // 初始化当前属性值
            foreach (string key in props.Keys)
                this.Add(key, props[key]);
        }

        #endregion

        #region 属性
        /// <summary>
        /// 获取属性键
        /// </summary>
        /// <returns></returns>
        public string[] PropertyKeys
        {
            get { return this.Keys.ToArray(); }
        }

        #endregion

        #region 方法
        /// <summary>
        /// 根据属性键设置属性名
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetProperty(string key, object value)
        {
            if (this.ContainsKey(key))
                this[key] = value;
            else
                this.Add(key, value);
        }

        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetProperty(string key)
        {
            if (this.ContainsKey(key))
                return this[key];
            else
                return null;
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string[] keys = this.PropertyKeys;
            int size = keys.Length;
            StringBuilder sb = new StringBuilder(20 * size);
            sb.Append("{");

            int n = 0;
            for (int i = 0; i < size; i++)
            {
                string key = keys[i];
                if (n > 0) sb.Append(", "); //$NON-NLS-1$
                sb.Append(key);
                sb.Append('=');
                object value = this.GetProperty(key);
                if (value.GetType().IsArray)
                {
                    var arrayValue = value as Array;
                    sb.Append('[');
                    int length = arrayValue.Length;
                    for (int j = 0; j < length; j++)
                    {
                        if (j > 0)
                            sb.Append(',');
                        sb.Append(arrayValue.GetValue(i));
                    }
                    sb.Append(']');
                }
                else
                {
                    sb.Append(value);
                }
                n++;
            }
            sb.Append('}');

            return sb.ToString();
        }

        #endregion
    }
}

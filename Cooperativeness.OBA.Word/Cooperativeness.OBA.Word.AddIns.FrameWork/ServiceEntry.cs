using System.Collections.Generic;
using System.Linq;
using Cooperativeness.OBA.Word.AddIns.FrameWork.MetaData;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork
{
    /// <summary>
    /// 定义服务条目
    /// </summary>
    internal class ServiceEntry
    {
        #region 字段
        private string type;
        private string service;
        private string property;

        #endregion

        #region 构造函数
        public ServiceEntry(XService metadata)
        {
            this.type = metadata.Type;
            this.service = metadata.Service;
            this.property = metadata.Properties;
        }

        #endregion

        #region 属性
        /// <summary>
        /// 获取服务的接口类型
        /// </summary>
        public string Type
        {
            get { return this.type; }
        }

        /// <summary>
        /// 获取服务的类型
        /// </summary>
        public string Service
        {
            get { return this.service; }
        }

        /// <summary>
        /// 获取服务的属性列表
        /// </summary>
        public string Property
        {
            get { return this.property; }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 解析服务属性
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        internal IDictionary<string, object> ParseStringProperty(string properties)
        {
            if (string.IsNullOrEmpty(properties))
                return null;
            string[] dataArray = properties.Split(",".ToArray());
            IDictionary<string, object> serviceProperties = new Dictionary<string, object>();
            foreach (var data in dataArray)
            {
                string[] property = data.Split("=".ToArray());
                if (properties.Length == 2)
                    continue;
                string propertyName = property[0].Trim();
                string propertyValue = property[1].Trim();
                if (!string.IsNullOrEmpty(propertyName) && !string.IsNullOrEmpty(propertyValue))
                    serviceProperties.Add(propertyName, propertyValue);
 
            }

            return serviceProperties.Count() > 0 ? serviceProperties : null;
        }
        #endregion
    }
}

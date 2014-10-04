using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Cooperativeness.OBA.Word.AddIns.FrameWork.MetaData;
using Cooperativeness.OBA.Word.Tools;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Core
{
    /// <summary>
    /// 定义插件扩展对象
    /// </summary>
    internal class ExtensionImpl : IExtension
    {
        private static readonly Logger Log = new Logger(typeof(ExtensionImpl));
        #region 字段
        private IBundle owner;
        private string point;
        private IList<XElement> data;
        private XExtension metadata;

        #endregion

        #region 构造函数
        public ExtensionImpl(IBundle bundle, XExtension metadata)
        {
            this.metadata = metadata;
            this.owner = bundle;
            this.point = metadata.Point;
            this.data = ParseExtensionData(metadata.ToString());
        }

        #endregion

        #region 属性
        /// <summary>
        /// 获取当前扩展对象所属的插件
        /// </summary>
        public IBundle Owner
        {
            get { return this.owner; }
            internal set { this.owner = value; }
        }

        /// <summary>
        /// 获取扩展点名称
        /// </summary>
        public string Point
        {
            get { return this.point; }
        }

        /// <summary>
        /// 获取扩展数据
        /// </summary>
        public IList<XElement> Data
        {
            get { return this.data; }
        }

        #endregion

        #region 方法
        /// <summary>
        /// 解析扩展数据
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        internal IList<XElement> ParseExtensionData(string strData)
        {
            if (string.IsNullOrEmpty(strData)) return null;
            try
            {
                XElement xExtension = XElement.Parse(strData);
                IEnumerable<XElement> xDatas = xExtension.Elements();
                IList<XElement> xExtensionData = new List<XElement>();
                foreach (var xData in xDatas)
                    xExtensionData.Add(xData);
                return xExtensionData;
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
            }
            return null;
        }

        /// <summary>
        /// 判断是否相等
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == this)
                return true;
            return  ((ExtensionImpl)obj).metadata.ToString().Equals(this.metadata.ToString());
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Extension:" + this.point;
        }

        /// <summary>
        /// 获取哈希值
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
        #endregion

    }
}

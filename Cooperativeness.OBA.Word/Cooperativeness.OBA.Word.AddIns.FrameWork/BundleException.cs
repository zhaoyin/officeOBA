using System;
using System.Runtime.Serialization;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork
{
    /// <summary>
    /// 定义插件异常
    /// </summary>
    public class BundleException : Exception
    {
        #region 字段
        private BundleExceptionType type;

        #endregion

        #region 构造函数
        public BundleException()
            : base()
        {
        }

        public BundleException(string message)
            : base(message)
        {
        }

        public BundleException(string message, BundleExceptionType type)
            : base(message)
        {
            this.type = type;
        }

        protected BundleException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public BundleException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public BundleException(string message, Exception innerException, BundleExceptionType type)
            : base(message, innerException)
        {
            this.type = type;
        }

        #endregion

        #region 属性
        /// <summary>
        /// 获取异常类型
        /// </summary>
        public BundleExceptionType Type
        {
            get { return type; }
        }
        #endregion
    }
}

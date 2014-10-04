using System;
using System.Runtime.Serialization;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork
{
    /// <summary>
    /// 定义非法状态异常
    /// </summary>
    public class IllegalStateException : Exception
    {
        public IllegalStateException() :  base()
        {
        }

        public IllegalStateException(string message)
            : base(message)
        {
        }

        protected IllegalStateException(SerializationInfo info, StreamingContext context)
            :base(info,context)
        {
        }

        protected IllegalStateException(string message, Exception innerException)
            :base(message,innerException)
        {
        }

        
    }
}

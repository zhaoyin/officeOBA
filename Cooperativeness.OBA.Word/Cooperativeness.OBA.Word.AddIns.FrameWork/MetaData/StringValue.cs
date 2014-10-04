using System;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.MetaData
{
    /// <summary>
    /// 定义字符串类型的值对象
    /// </summary>
    public class StringValue : SimpleType
    {
        #region 构造函数
        public StringValue()
        {
        }

        public StringValue(StringValue source)
            : base(source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
        }

        public StringValue(string value)
        {
            base.TextValue = value;
        }
        #endregion

        #region 方法
        internal override SimpleType CloneImp()
        {
            return new StringValue(this);
        }

        public static StringValue FromString(string value)
        {
            return new StringValue(value);
        }

        public static string ToString(StringValue xmlAttribute)
        {
            if (xmlAttribute == null)
            {
                throw new InvalidOperationException();
            }
            return xmlAttribute.Value;
        }

        internal override int CompareToImp(object obj)
        {
            return this.Value.CompareTo(obj);
        }
        #endregion

        #region 运算符重载
        public static implicit operator string(StringValue xmlAttribute)
        {
            if (xmlAttribute == null)
            {
                return null;
            }
            return ToString(xmlAttribute);
        }

        public static implicit operator StringValue(string value)
        {
            return FromString(value);
        }

        #endregion

        #region 属性
        public string Value
        {
            get
            {
                return base.TextValue;
            }
            set
            {
                base.TextValue = value;
            }
        }
        #endregion
    }
}

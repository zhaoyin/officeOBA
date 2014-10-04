using System;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.MetaData
{
    /// <summary>
    /// 定义Long类型的值对象
    /// </summary>
    public class LongValue : SimpleValue<long>
    {
        #region 构造方法
        public LongValue()
        {
        }

        public LongValue(LongValue source)
            : base(source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
        }

        public LongValue(long value)
            : base(value)
        {
        }
        #endregion

        #region 方法
        internal override SimpleType CloneImp()
        {
            return new LongValue(this);
        }

        public static LongValue FromInt64(long value)
        {
            return new LongValue(value);
        }

        internal override void Parse()
        {
            long localvalue;
            if (long.TryParse(base.TextValue, out localvalue))
                base.InnerValue = new long?(localvalue);
            else
                base.InnerValue = null;
        }

        public static long ToInt64(LongValue attribute)
        {
            if (attribute == null)
            {
                throw new InvalidOperationException();
            }
            return attribute.Value;
        }

        internal override bool TryParse()
        {
            base.InnerValue = null;
            long localValue;
            return long.TryParse(base.TextValue, out localValue);
        }

        internal override int CompareToImp(long other)
        {
            return this.Value.CompareTo(other);
        }

        internal override bool EqualsImp(long other)
        {
            return this.Value.Equals(other);
        }
       
        #endregion

        #region 运算符重载
        public static implicit operator long(LongValue attribute)
        {
            if (attribute == null)
            {
                throw new InvalidOperationException();
            }
            return ToInt64(attribute);
        }

        public static implicit operator LongValue(long value)
        {
            return FromInt64(value);
        }

        #endregion

        #region 属性
        public override string InnerText
        {
            get
            {
                if ((base.TextValue == null) && base.InnerValue.HasValue)
                {
                    base.TextValue = base.InnerValue.Value.ToString();
                }
                return base.TextValue;
            }
        }

        #endregion
    }
}

using System;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.MetaData
{
    /// <summary>
    /// 定义Long类型的值对象
    /// </summary>
    public class IntValue : SimpleValue<int>
    {
        #region 构造方法
        public IntValue()
        {
        }

        public IntValue(IntValue source)
            : base(source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
        }

        public IntValue(int value)
            : base(value)
        {
        }
        #endregion

        #region 方法
        internal override SimpleType CloneImp()
        {
            return new IntValue(this);
        }

        public static IntValue FromInt32(int value)
        {
            return new IntValue(value);
        }

        internal override void Parse()
        {
            int localvalue;
            if (int.TryParse(base.TextValue, out localvalue))
                base.InnerValue = new int?(localvalue);
            else
                base.InnerValue = null;
        }

        public static int ToInt32(IntValue attribute)
        {
            if (attribute == null)
            {
                throw new InvalidOperationException();
            }
            return attribute.Value;
        }

        internal static int? ToInt32(string value)
        {
            int local;
            if (int.TryParse(value, out local))
            {
                return local;
            }
            return null;
        }

        internal static int ToInt32(string value, int defaultValue)
        {
            int local;
            if (int.TryParse(value, out local))
            {
                return local;
            }
            return defaultValue;
        }

        internal override bool TryParse()
        {
            base.InnerValue = null;
            int localValue;
            return int.TryParse(base.TextValue, out localValue);
        }

        internal override int CompareToImp(int other)
        {
            return this.Value.CompareTo(other);
        }

        internal override bool EqualsImp(int other)
        {
            return this.Value.Equals(other);
        }
       
        #endregion

        #region 运算符重载
        public static implicit operator int(IntValue attribute)
        {
            if (attribute == null)
            {
                throw new InvalidOperationException();
            }
            return ToInt32(attribute);
        }

        public static implicit operator IntValue(int value)
        {
            return FromInt32(value);
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

using System;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.MetaData
{
    /// <summary>
    /// 定义布尔类型的值对象
    /// </summary>
    public class BoolValue : SimpleValue<bool>
    {
        #region 构造方法
        public BoolValue()
        {
        }

        public BoolValue(BoolValue source)
            : base(source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
        }

        public BoolValue(bool value)
            : base(value)
        {
        }
        #endregion

        #region 方法
        internal override SimpleType CloneImp()
        {
            return new BoolValue(this);
        }

        public static BoolValue FromBool(bool value)
        {
            return new BoolValue(value);
        }

        internal static bool? FromString(string value)
        {
            bool local;
            if (bool.TryParse(value, out local))
                return local;

            return null;
        }


        internal static bool FromString(string value,bool defaultValue)
        {
            bool local;
            if (bool.TryParse(value, out local))
                return local;

            return defaultValue;
        }

        internal override void Parse()
        {
            base.InnerValue = new bool?(Convert.ToBoolean(base.TextValue));
        }

        public static bool ToBool(BoolValue attribute)
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
            try
            {
                bool isBool = Convert.ToBoolean(base.TextValue);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
            catch (OverflowException)
            {
                return false;
            }
        }

        internal override int CompareToImp(bool other)
        {
            return this.Value.CompareTo(other);
        }

        internal override bool EqualsImp(bool other)
        {
            return this.Value.Equals(other);
        }
        #endregion

        #region 运算符重载
        public static implicit operator bool(BoolValue attribute)
        {
            if (attribute == null)
            {
                throw new InvalidOperationException();
            }
            return ToBool(attribute);
        }

        public static implicit operator BoolValue(bool value)
        {
            return FromBool(value);
        }
        #endregion

        #region 属性
        public override string InnerText
        {
            get
            {
                if ((base.TextValue == null) && base.InnerValue.HasValue)
                {
                    base.TextValue = Convert.ToString(base.InnerValue.Value);
                }
                return base.TextValue;
            }
        }
        #endregion

    }
}

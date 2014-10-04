using System;
using System.Collections.Generic;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.MetaData
{
    /// <summary>
    /// 定义枚举类型值对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EnumValue<T> : SimpleType where T : struct
    {
        #region 字段
        private T? enumValue;
        private static object lockroot;
        private static int parseCount;
        private static Dictionary<string, T> stringToValueLookupTable;
        private const int Threshold = 11;

        #endregion

        #region 构造函数
        static EnumValue()
        {
            EnumValue<T>.lockroot = new object();
        }

        public EnumValue()
        {
        }

        public EnumValue(EnumValue<T> source)
            : base(source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            this.enumValue = source.enumValue;
        }

        public EnumValue(T value)
        {
            if (!Enum.IsDefined(typeof(T), value))
            {
                throw new ArgumentOutOfRangeException("value");
            }
            this.Value = value;
        }

        #endregion

        #region 方法
        internal override SimpleType CloneImp()
        {
            return new EnumValue<T>((EnumValue<T>)this);
        }

        internal static T GetEnumValue(string stringVal)
        {
            T local;
            if (!EnumValue<T>.TryGetEnumValue(stringVal, out local))
            {
                throw new FormatException();
            }
            return local;
        }

        internal static T GetEnumValue(string stringVal,T defalutValue)
        {
            if (string.IsNullOrEmpty(stringVal)) return defalutValue;
            T local;
            if (!EnumValue<T>.TryGetEnumValue(stringVal, out local))
            {
                return defalutValue;
            }
            return local;
        }

        internal override void Parse()
        {
            this.enumValue = new T?(EnumValue<T>.GetEnumValue(base.TextValue));
        }

        internal static string ToString(T enumVal)
        {
            EnumStringAttribute customAttribute = Attribute.GetCustomAttribute(enumVal.GetType().GetField(enumVal.ToString()), typeof(EnumStringAttribute)) as EnumStringAttribute;
            if (customAttribute != null)
            {
                return customAttribute.Value;
            }
            return string.Empty;
        }

        internal static bool TryGetEnumValue(string stringVal, out T result)
        {
            if (EnumValue<T>.parseCount > 11)
            {
                if (EnumValue<T>.stringToValueLookupTable == null)
                {
                    Dictionary<string, T> dictionary = new Dictionary<string, T>();
                    foreach (T local in Enum.GetValues(typeof(T)))
                    {
                        EnumStringAttribute customAttribute = Attribute.GetCustomAttribute(local.GetType().GetField(local.ToString()), typeof(EnumStringAttribute)) as EnumStringAttribute;
                        dictionary.Add(customAttribute.Value, local);
                    }
                    EnumValue<T>.stringToValueLookupTable = dictionary;
                }
                return EnumValue<T>.stringToValueLookupTable.TryGetValue(stringVal, out result);
            }
            EnumValue<T>.parseCount++;
            foreach (T local2 in Enum.GetValues(typeof(T)))
            {
                EnumStringAttribute attribute2 = Attribute.GetCustomAttribute(local2.GetType().GetField(local2.ToString()), typeof(EnumStringAttribute)) as EnumStringAttribute;
                if ((attribute2 != null) && (attribute2.Value == stringVal))
                {
                    result = local2;
                    return true;
                }
            }
            result = default(T);
            return false;
        }

        internal override bool TryParse()
        {
            T local;
            this.enumValue = null;
            if (EnumValue<T>.TryGetEnumValue(base.TextValue, out local))
            {
                this.enumValue = new T?(local);
                return true;
            }
            return false;
        }

        internal override int CompareToImp(object obj)
        {
            throw new InvalidOperationException();
        }

        #endregion

        #region 运算符重载
        public static implicit operator EnumValue<T>(T value)
        {
            return new EnumValue<T>(value);
        }

        public static implicit operator T(EnumValue<T> attribute)
        {
            if (attribute == null)
            {
                throw new InvalidOperationException();
            }
            return attribute.Value;
        }

        public static implicit operator string(EnumValue<T> value)
        {
            if (value == null)
            {
                return null;
            }
            return value.InnerText;
        }

        #endregion

        #region 属性
        public override bool HasValue
        {
            get
            {
                if (!this.enumValue.HasValue && (base.TextValue != null))
                {
                    this.TryParse();
                }
                return this.enumValue.HasValue;
            }
        }

        public override string InnerText
        {
            get
            {
                if ((base.TextValue == null) && this.enumValue.HasValue)
                {
                    base.TextValue = EnumValue<T>.ToString(this.enumValue.Value);
                }
                return base.TextValue;
            }
            set
            {
                base.TextValue = value;
                this.enumValue = null;
            }
        }

        public T Value
        {
            get
            {
                if (!this.enumValue.HasValue && !string.IsNullOrEmpty(base.TextValue))
                {
                    this.Parse();
                }
                return this.enumValue.Value;
            }
            set
            {
                if (!Enum.IsDefined(typeof(T), value))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this.enumValue = new T?(value);
                base.TextValue = null;
            }
        }
        #endregion
    }
}

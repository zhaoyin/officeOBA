using System;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.MetaData
{
    /// <summary>
    /// 定义简单类型
    /// </summary>
    public abstract class SimpleType : ICloneable, IComparable
    {
        #region 字段
        private string textValue;

        #endregion

        #region 构造函数
        protected SimpleType()
        {
        }

        protected SimpleType(SimpleType source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            this.TextValue = source.TextValue;
        }

        #endregion

        #region 属性
        /// <summary>
        /// 获取或设置文本内容
        /// </summary>
        public string TextValue
        {
            get
            {
                return this.textValue;
            }
            set
            {
                this.textValue = value;
            }
        }

        /// <summary>
        /// 获取一个值，该值指示此类型的对象是否有值。
        /// </summary>
        /// <returns></returns>
        public virtual bool HasValue
        {
            get
            {
                return (this.textValue != null);
            }

        }

        /// <summary>
        /// 获取或设置内部文本内容
        /// </summary>
        public virtual string InnerText
        {
            get
            {
                return this.textValue;
            }
            set
            {
                this.textValue = value;
            }
        }

        #endregion

        #region 方法
        /// <summary>
        /// 字符串赋值操作重载
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static implicit operator string(SimpleType attribute)
        {
            if (attribute == null)
            {
                return null;
            }
            return attribute.TextValue;
        }

        /// <summary>
        /// 解析操作，将文本只转换为制定类型的值
        /// </summary>
        internal virtual void Parse()
        {
        }

        public override string ToString()
        {
            return this.textValue;
        }

        /// <summary>
        /// 解析操作，将文本内容转换为制定类型的值
        /// 如果转换成功则返回true，否则返回false
        /// </summary>
        /// <returns></returns>
        internal virtual bool TryParse()
        {
            return true;
        }

        #endregion

        #region 实现ICloneable接口
        /// <summary>
        /// 拷贝当前对象
        /// </summary>
        /// <returns></returns>
        internal abstract SimpleType CloneImp();

        public object Clone()
        {
            return this.CloneImp();
        }

        #endregion

        #region 实现IComparable接口
        /// <summary>
        /// 比较对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal abstract int CompareToImp(object obj);

        public int CompareTo(object obj)
        {
            return this.CompareToImp(obj);
        }

        #endregion
    }

    /// <summary>
    /// 定义简单类型值对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SimpleValue<T> : SimpleType where T : struct,IComparable<T>, IEquatable<T>
    {
        #region 字段
        private T? value;

        #endregion

        #region 构造函数
        protected SimpleValue()
        {
        }

        protected SimpleValue(T value)
        {
            this.Value = value;
        }

        protected SimpleValue(SimpleValue<T> source)
            : base(source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            this.Value = source.Value;
        }

        #endregion

        #region 操作符重载
        public static implicit operator T(SimpleValue<T> attribute)
        {
            if (attribute == null)
            {
                throw new InvalidOperationException();
            }
            return attribute.Value;
        }

        #endregion

        #region 属性
        /// <summary>
        /// 获取一个值，该值指示当前类型的对象是否有值
        /// </summary>
        public override bool HasValue
        {
            get
            {
                if (!this.value.HasValue && !string.IsNullOrEmpty(base.TextValue))
                {
                    this.TryParse();
                }
                return this.value.HasValue;
            }
        }

        /// <summary>
        /// 获取或设置内部值
        /// </summary>
        internal T? InnerValue
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
            }
        }

        /// <summary>
        /// 获取或设置值
        /// </summary>
        public T Value
        {
            get
            {
                if (!this.value.HasValue && !string.IsNullOrEmpty(base.TextValue))
                {
                    this.Parse();
                }
                return this.value.Value;
            }
            set
            {
                this.value = new T?(value);
                base.TextValue = null;
            }
        }

        public override string InnerText
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                base.TextValue = value;
                this.value = null;
            }
        }
        #endregion

        #region 实现IComparable接口
        internal abstract int CompareToImp(T other);

        internal override int CompareToImp(object obj)
        {
            return this.CompareToImp((T)obj);
        }

        public int CompareTo(T other)
        {
            return this.CompareToImp(other);
        }
        #endregion

        #region 实现IEquatable接口
        internal abstract bool EqualsImp(T other);

        public bool Equals(T other)
        {
            return this.EqualsImp(other);
        }
        #endregion

        #region 方法
        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }
        #endregion
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.MetaData
{
    /// <summary>
    /// 定义抽象的基元元素对象
    /// </summary>
    public abstract class MetaElement : IEnumerable<MetaElement>, IEnumerable, ICloneable
    {
        #region 常量
        #endregion

        #region 字段
        private SimpleType[] fixedAttributes;
        private MetaElement next;
        private MetaElement parent;

        #endregion

        #region 属性
        /// <summary>
        /// 获取或设置元素的父元素
        /// </summary>
        public MetaElement Parent
        {
            get
            {
                return this.parent;
            }
            internal set
            {
                this.parent = value;
            }
        }

        /// <summary>
        /// 获取或设置元素的下一个兄弟元素
        /// </summary>
        internal MetaElement Next
        {
            get
            {
                return this.next;
            }
            set
            {
                this.next = value;
            }
        }

        /// <summary>
        /// 获取第一个孩子元素
        /// </summary>
        public virtual MetaElement FirstChild
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// 获取最后一个孩子元素
        /// </summary>
        public virtual MetaElement LastChild
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// 获取所有子元素的集合
        /// </summary>
        public virtual MetaElementCollection ChildElements
        {
            get
            {
                if (this.HasChildren)
                {
                    return new ChildElements(this);
                }
                return EmptyElementCollection.EmptySingleton;
            }
        }

        /// <summary>
        /// 获取一个值,该值用来指示当前元素是否存在子元素
        /// </summary>
        public abstract bool HasChildren { get; }

        /// <summary>
        /// 获取当前元素属性名称集合
        /// </summary>
        internal virtual string[] AttributeNames
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// 获取当前元素属性值集合
        /// </summary>
        internal SimpleType[] Attributes
        {
            get
            {
                if (this.fixedAttributes == null && this.HasAttribute)
                {
                    this.fixedAttributes = new SimpleType[this.AttributeNames.Length];
                }
                return this.fixedAttributes;
            }
        }

        /// <summary>
        /// 获取一个值,该值用来指示当前元素是否存属性
        /// </summary>
        public bool HasAttribute
        {
            get
            {
                return this.AttributeNames != null 
                    && this.AttributeNames.Length>0;
            }
        }

        /// <summary>
        /// 获取或设置元素的文本值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 获取或设置当前元素的XML串
        /// </summary>
        public string Xml { get; set; }

        #endregion

        #region 方法
        /// <summary>
        /// 获取下一个指定类型兄弟元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T NextSibling<T>() where T : MetaElement
        {
            for (MetaElement element = this.NextSibling(); element != null; element = element.NextSibling())
            {
                if (element is T)
                {
                    return (T)element;
                }
            }
            return default(T);
        }

        /// <summary>
        /// 获取下一个兄弟元素
        /// </summary>
        /// <returns></returns>
        public MetaElement NextSibling()
        {
            MetaElement parent = this.Parent;
            if ((parent != null) && (this.Next != parent.FirstChild))
            {
                return this.Next;
            }
            return null;
        }

        /// <summary>
        /// 获取上一个指定类型的兄弟元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T PreviousSibling<T>() where T : MetaElement
        {
            for (MetaElement element = this.PreviousSibling(); element != null; element = element.PreviousSibling())
            {
                if (element is T)
                {
                    return (T)element;
                }
            }
            return default(T);
        }

        /// <summary>
        /// 获取上一个兄弟元素
        /// </summary>
        /// <returns></returns>
        public MetaElement PreviousSibling()
        {
            MetaCompositeElement parent = this.Parent as MetaCompositeElement;
            if (parent == null)
            {
                return null;
            }
            MetaElement firstChild = parent.FirstChild;
            while (firstChild != null)
            {
                MetaElement element3 = firstChild.NextSibling();
                if (element3 == this)
                {
                    return firstChild;
                }
                firstChild = element3;
            }
            return firstChild;
        }

        /// <summary>
        /// 添加新的子元素集合
        /// </summary>
        /// <param name="newChildren"></param>
        public void Append(params MetaElement[] newChildren)
        {
            if (newChildren != null)
            {
                foreach (MetaElement element in newChildren)
                {
                    this.AppendChild<MetaElement>(element);
                }
            }
        }

        /// <summary>
        /// 添加新的子元素集合
        /// </summary>
        /// <param name="newChildren"></param>
        public void Append(IEnumerable<MetaElement> newChildren)
        {
            if (newChildren == null)
            {
                throw new ArgumentNullException("newChildren");
            }
            foreach (MetaElement element in newChildren)
            {
                this.AppendChild<MetaElement>(element);
            }
        }

        /// <summary>
        /// 添加新的子元素集合
        /// </summary>
        /// <param name="newChildren"></param>
        public void Append(IEnumerable newChildren)
        {
            if (newChildren == null)
            {
                throw new ArgumentNullException("newChildren");
            }
            foreach (MetaElement element in newChildren)
            {
                this.AppendChild<MetaElement>(element);
            }
        }

        /// <summary>
        /// 添加指定类型的子元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="newChild"></param>
        /// <returns></returns>
        public virtual T AppendChild<T>(T newChild) where T : MetaElement
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// 将当前元素从所在兄弟集合中删除
        /// </summary>
        public void Remove()
        {
            if (this.Parent == null)
            {
                throw new InvalidOperationException();
            }
            this.Parent.RemoveChild<MetaElement>(this);
        }

        /// <summary>
        /// 删除所有的子元素
        /// </summary>
        public abstract void RemoveAllChildren();

        /// <summary>
        /// 删除指定类型的所有子元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RemoveAllChildren<T>() where T : MetaElement
        {
            MetaElement element2;
            for (MetaElement element = this.FirstChild; element != null; element = element2)
            {
                element2 = element.NextSibling();
                if (element is T)
                {
                    this.RemoveChild<MetaElement>(element);
                }
            }
        }

        /// <summary>
        /// 删除指定类型的子元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="oldChild"></param>
        /// <returns></returns>
        public virtual T RemoveChild<T>(T oldChild) where T : MetaElement
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// 拷贝当前元素
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return this.CloneElement(true);
        }

        /// <summary>
        /// 根据deep标示决定是否深度拷贝
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="deep"></param>
        /// <returns></returns>
        internal virtual T CloneImp<T>(bool deep) where T : MetaElement, new()
        {
            T local = Activator.CreateInstance<T>();
            local.CopyAttribute(this);
            if (deep)
            {
                local.CopyChildren(this, deep);
            }
            return local;
        }

        /// <summary>
        /// 拷贝当前元素的子元素
        /// </summary>
        /// <param name="container"></param>
        /// <param name="deep"></param>
        internal void CopyChildren(MetaElement container, bool deep)
        {
            foreach (MetaElement element in container.ChildElements)
            {
                this.Append(new MetaElement[] { element.CloneElement(deep) });
            }
        }

        /// <summary>
        /// 拷贝当前元素的所有属性
        /// </summary>
        /// <param name="container"></param>
        internal void CopyAttribute(MetaElement container)
        {
            if (container.HasAttribute)
            {
                int lenght = this.Attributes.Length;
                for (int i = 0; i < lenght; i++)
                {
                    if (container.Attributes[i] == null) this.Attributes[i] = null;
                    else this.Attributes[i] = container.Attributes[i].CloneImp();
                }
            }

        }

        /// <summary>
        /// 根据用户的XML数据设置属性值
        /// </summary>
        /// <param name="element"></param>
        protected virtual void SetProperties(XElement element) { }

        /// <summary>
        /// 克隆元素
        /// </summary>
        /// <param name="deep"></param>
        /// <returns></returns>
        public virtual MetaElement CloneElement(bool deep)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取当前元素的枚举器
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取当前元素的枚举器
        /// </summary>
        /// <returns></returns>
        IEnumerator<MetaElement> IEnumerable<MetaElement>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Xml;
        }

        #endregion

        #region 加载和解析方法
        /// <summary>
        /// 根据用户指定的XML串解析元数据
        /// </summary>
        /// <param name="xml">用户XML串</param>
        /// <returns></returns>
        public static MetaElement Parse(string xml)
        {
            try
            {
                // 参数校验
                if (string.IsNullOrEmpty(xml)) return null;
                // 构造XML Document
                XDocument document = XDocument.Parse(xml);
                // 解析节点数据
                return OnParse(document.Root);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 根据用户指定的XML路径解析元数据
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static MetaElement Load(string path)
        {
            try
            {
                // 参数校验
                if (string.IsNullOrEmpty(path)) return null;
                if (!File.Exists(path)) return null;
                // 构造XML Document
                XDocument document = XDocument.Load(path);
                // 解析节点数据
                return OnParse(document.Root);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 解析XElement对象
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        internal static MetaElement OnParse(XElement element)
        {
            try
            {
                // 检查是否属于元素类型
                if (element.NodeType != System.Xml.XmlNodeType.Element)
                    return null;
                // 获取组件类型属性
                string typeName = string.Concat(new object[] { ConfigConstant.PREFIX_NS, element.Name.LocalName });
                // 获取当元素对应的类型
                Type type = Type.GetType(typeName);
                if (type == null) return null;
                // 创建元素组件
                MetaElement meteObj = Activator.CreateInstance(type) as MetaElement;
                if (meteObj == null) return null;
                // 元素属性赋值操作
                meteObj.SetProperties(element);
                // 赋值操作
                meteObj.Value = element.Value;
                // 保存当前的XML串
                meteObj.Xml = element.ToString();
                // 循环解析子元素
                foreach (var item in element.Elements())
                {
                    MetaElement subMeteObj = OnParse(item);
                    if (subMeteObj != null)
                    {
                        meteObj.AppendChild(subMeteObj);
                    }
                }
                return meteObj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}

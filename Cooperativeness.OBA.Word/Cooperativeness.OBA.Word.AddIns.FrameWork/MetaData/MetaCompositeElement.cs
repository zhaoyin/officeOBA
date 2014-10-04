using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.MetaData
{
    /// <summary>
    /// 定义抽象的基元组合元素对象
    /// </summary>
    public abstract class MetaCompositeElement : MetaElement
    {
        #region 字段
        private MetaElement _lastChild;

        #endregion

        #region 构造函数
        protected MetaCompositeElement()
        {
        }

        protected MetaCompositeElement(IEnumerable<MetaElement> childrenElements)
            : this()
        {
            if (childrenElements == null)
            {
                throw new ArgumentNullException("childrenElements");
            }
            this.Append(childrenElements);
        }

        protected MetaCompositeElement(IEnumerable childrenElements)
            : this()
        {
            if (childrenElements == null)
            {
                throw new ArgumentNullException("childrenElements");
            }
            this.Append(childrenElements);
        }


        protected MetaCompositeElement(params MetaElement[] childrenElements)
            : this()
        {
            if (childrenElements == null)
            {
                throw new ArgumentNullException("childrenElements");
            }
            this.Append(childrenElements);
        }

        #endregion

        #region 方法
        /// <summary>
        /// 添加元素到子元素集合的队尾
        /// </summary>
        /// <param name="element"></param>
        private void AddElement(MetaElement element)
        {
            element.Parent = this;
            if (this._lastChild == null)
            {
                element.Next = element;
                this._lastChild = element;
            }
            else
            {
                element.Next = this._lastChild.Next;
                this._lastChild.Next = element;
                this._lastChild = element;
            }
        }

        /// <summary>
        /// 添加指定类型的元素到子元素集合的队尾
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="newChild"></param>
        /// <returns></returns>
        public override T AppendChild<T>(T newChild)
        {
            if (newChild == null)
            {
                return default(T);
            }
            if (newChild.Parent != null)
            {
                throw new InvalidOperationException();
            }

            MetaElement lastChild = this.LastChild;
            MetaElement element2 = newChild;
            if (lastChild == null)
            {
                element2.Next = element2;
                this._lastChild = element2;
            }
            else
            {
                element2.Next = lastChild.Next;
                lastChild.Next = element2;
                this._lastChild = element2;
            }
            newChild.Parent = this;

            return newChild;
        }

        /// <summary>
        /// 移除所有的子元素
        /// </summary>
        public override void RemoveAllChildren()
        {
            MetaElement element2;
            for (MetaElement element = this.FirstChild; element != null; element = element2)
            {
                element2 = element.NextSibling();
                this.RemoveChild<MetaElement>(element);
            }
        }

        /// <summary>
        /// 移除指定类型的子元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="oldChild"></param>
        /// <returns></returns>
        public override T RemoveChild<T>(T oldChild)
        {
            if (oldChild == null)
            {
                return default(T);
            }
            if (oldChild.Parent != this)
            {
                throw new InvalidOperationException();
            }

            T local = oldChild;
            MetaElement element = this._lastChild;
            if (local == this.FirstChild)
            {
                if (local == this._lastChild)
                {
                    this._lastChild = null;
                }
                else
                {
                    MetaElement next = local.Next;
                    element.Next = next;
                }
            }
            else if (local == this._lastChild)
            {
                MetaElement element3 = local.PreviousSibling();
                MetaElement element4 = local.Next;
                element3.Next = element4;
                this._lastChild = element3;
            }
            else
            {
                MetaElement element5 = local.PreviousSibling();
                MetaElement element6 = local.Next;
                element5.Next = element6;
            }
            local.Next = null;
            local.Parent = null;

            return local;
        }

        /// <summary>
        /// 获取指定类型的元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        internal T GetElement<T>() where T : MetaElement
        {
            T local;
            foreach (MetaElement element in this.ChildElements)
            {
                local = element as T;
                if (local != null)
                {
                    return local;
                }
            }
            return default(T);
        }

        /// <summary>
        /// 设置指定类型的元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="newChild"></param>
        internal void SetElement<T>(T newChild) where T : MetaElement
        {
            T oldChild = this.GetElement<T>();
            if (oldChild != null)
            {
                this.RemoveChild<T>(oldChild);
            }
            if (newChild != null)
            {
                this.AppendChild<T>(newChild);
            }
            return;
        }

        /// <summary>
        /// 获取指定类型的子元素集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        internal T[] GetElements<T>() where T : MetaElement
        {
            T local;
            IList<T> locals = new List<T>();
            foreach (MetaElement element in this.ChildElements)
            {
                local = element as T;
                if (local != null)
                {
                    locals.Add(local);
                }
            }
            return locals.Count == 0 ? null : locals.ToArray<T>();
        }

        /// <summary>
        /// 获取指定类型的子元素集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public T[] Query<T>() where T : MetaElement
        {
            IList<T> findElements = new List<T>();
            foreach (MetaElement element in this.ChildElements)
            {
                if (element is T)
                {
                    findElements.Add(element as T);
                }
            }
            return findElements.ToArray();
        }

        /// <summary>
        /// 获取指定类型的子元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public T QuerySingle<T>() where T : MetaElement
        {
            IList<T> findElements = new List<T>();
            foreach (MetaElement element in this.ChildElements)
            {
                if (element is T)
                {
                    return element as T;
                }
            }
            return default(T);
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取最后一个子元素
        /// </summary>
        public override MetaElement LastChild
        {
            get
            {
                return this._lastChild;
            }
        }

        /// <summary>
        /// 获取第一个子元素
        /// </summary>
        public override MetaElement FirstChild
        {
            get
            {
                MetaElement element = this._lastChild;
                if (element != null)
                {
                    return element.Next;
                }
                return null;
            }
        }

        /// <summary>
        /// 获取一个值，该值指示当前元素是否存在子元素
        /// </summary>
        public override bool HasChildren
        {
            get
            {
                return (this.LastChild != null);
            }
        }

        #endregion
    }
}

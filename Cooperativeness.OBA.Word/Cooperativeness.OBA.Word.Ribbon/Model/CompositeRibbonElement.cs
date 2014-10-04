using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace Cooperativeness.OBA.Word.Ribbon.Model
{
    /// <summary>
    /// 定义抽象的功能区元素组合对象
    /// </summary>
    public abstract class CompositeRibbonElement : RibbonElement
    {
        #region 字段
        private RibbonElement _lastChild;

        #endregion

        #region 构造函数
        protected CompositeRibbonElement()
        {
        }

        protected CompositeRibbonElement(IEnumerable<RibbonElement> childrenElements)
            : this()
        {
            if (childrenElements == null)
            {
                throw new ArgumentNullException("childrenElements");
            }
            this.Append(childrenElements);
        }

        protected CompositeRibbonElement(IEnumerable childrenElements)
            : this()
        {
            if (childrenElements == null)
            {
                throw new ArgumentNullException("childrenElements");
            }
            this.Append(childrenElements);
        }


        protected CompositeRibbonElement(params RibbonElement[] childrenElements)
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
        private void AddElement(RibbonElement element)
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

            RibbonElement lastChild = this.LastChild;
            RibbonElement element2 = newChild;
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
            RibbonElement element2;
            for (RibbonElement element = this.FirstChild; element != null; element = element2)
            {
                element2 = element.NextSibling();
                this.RemoveChild<RibbonElement>(element);
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
            RibbonElement element = this._lastChild;
            if (local == this.FirstChild)
            {
                if (local == this._lastChild)
                {
                    this._lastChild = null;
                }
                else
                {
                    RibbonElement next = local.Next;
                    element.Next = next;
                }
            }
            else if (local == this._lastChild)
            {
                RibbonElement element3 = local.PreviousSibling();
                RibbonElement element4 = local.Next;
                element3.Next = element4;
                this._lastChild = element3;
            }
            else
            {
                RibbonElement element5 = local.PreviousSibling();
                RibbonElement element6 = local.Next;
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
        internal T GetElement<T>() where T : RibbonElement
        {
            T local;
            foreach (RibbonElement element in this.ChildElements)
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
        internal void SetElement<T>(T newChild) where T : RibbonElement
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
        internal T[] GetElements<T>() where T : RibbonElement
        {
            T local;
            IList<T> locals = new List<T>();
            foreach (RibbonElement element in this.ChildElements)
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
        public T[] Query<T>() where T : RibbonElement
        {
            IList<T> findElements = new List<T>();
            foreach (RibbonElement element in this.ChildElements)
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
        public T QuerySingle<T>() where T : RibbonElement
        {
            IList<T> findElements = new List<T>();
            foreach (RibbonElement element in this.ChildElements)
            {
                if (element is T)
                {
                    return element as T;
                }
            }
            return default(T);
        }

        /// <summary>
        /// 根据页签标示查找指定功能页签
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //public T Find<T>(string key) where T : RibbonElement
        //{
        //    T xRibbonElement = this.TryGetCachedElement<T>(key);
        //    if (xRibbonElement == null)
        //    {
        //        foreach (T tab in this)
        //        {
        //            if (tab != null && tab.Id.Equals(key))
        //            {
        //                xRibbonElement = tab;
        //                this.CachedFoundElement<T>(key, tab);
        //                break;
        //            }
        //        }
        //    }
        //    return xRibbonElement;
        //}

        #endregion

        #region 属性
        /// <summary>
        /// 获取最后一个子元素
        /// </summary>
        public override RibbonElement LastChild
        {
            get
            {
                return this._lastChild;
            }
        }

        /// <summary>
        /// 获取第一个子元素
        /// </summary>
        public override RibbonElement FirstChild
        {
            get
            {
                RibbonElement element = this._lastChild;
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

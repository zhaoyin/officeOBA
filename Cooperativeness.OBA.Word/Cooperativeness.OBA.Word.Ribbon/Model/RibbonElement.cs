using System;
using System.Collections.Generic;
using System.Text;
using Cooperativeness.OBA.Word.AddIns.FrameWork;
using Microsoft.Office.Interop.Word;
using System.Xml.Linq;
using System.Collections;

namespace Cooperativeness.OBA.Word.Ribbon.Model
{
    /// <summary>
    /// 定义抽象的功能区元素对象
    /// </summary>
    public abstract class RibbonElement : IRibbonElement, IEnumerable<RibbonElement>, IEnumerable,IDisposable
    {
        #region 字段
        private AddIn addIn;
        private RibbonElement next;
        private RibbonElement parent;
        private XRibbon xRibbon;
        private bool closed = false;


        #endregion

        #region 属性
        /// <summary>
        /// 获取或设置元素的父元素
        /// </summary>
        public RibbonElement Parent
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
        internal RibbonElement Next
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
        public virtual RibbonElement FirstChild
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// 获取最后一个孩子元素
        /// </summary>
        public virtual RibbonElement LastChild
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// 获取所有子元素的集合
        /// </summary>
        public virtual ElementCollection ChildElements
        {
            get
            {
                if (this.HasChildren)
                {
                    return new ChildRibbonElements(this);
                }
                return EmptyElementCollection.EmptySingleton;
            }
        }

        /// <summary>
        /// 获取一个值,该值用来指示当前元素是否存在子元素
        /// </summary>
        public abstract bool HasChildren { get; }

        /// <summary>
        /// 获取功能区对象
        /// </summary>
        internal protected XRibbon XRibbon
        {
            get
            {
                if (xRibbon == null)
                {
                    RibbonElement elment = this;
                    while (elment != null)
                    {
                        if (elment is XRibbon)
                        {
                            this.xRibbon = (XRibbon)elment;
                            break;
                        }
                        elment = elment.parent;
                    }
                }
                return this.xRibbon;
            }
        }

        #endregion

        #region 方法
        /// <summary>
        /// 获取下一个指定类型兄弟元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T NextSibling<T>() where T : RibbonElement
        {
            for (RibbonElement element = this.NextSibling(); element != null; element = element.NextSibling())
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
        public RibbonElement NextSibling()
        {
            RibbonElement parent = this.Parent;
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
        public T PreviousSibling<T>() where T : RibbonElement
        {
            for (RibbonElement element = this.PreviousSibling(); element != null; element = element.PreviousSibling())
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
        public RibbonElement PreviousSibling()
        {
            CompositeRibbonElement parent = this.Parent as CompositeRibbonElement;
            if (parent == null)
            {
                return null;
            }
            RibbonElement firstChild = parent.FirstChild;
            while (firstChild != null)
            {
                RibbonElement element3 = firstChild.NextSibling();
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
        public void Append(params RibbonElement[] newChildren)
        {
            if (newChildren != null)
            {
                foreach (RibbonElement element in newChildren)
                {
                    this.AppendChild<RibbonElement>(element);
                }
            }
        }

        /// <summary>
        /// 添加新的子元素集合
        /// </summary>
        /// <param name="newChildren"></param>
        public void Append(IEnumerable<RibbonElement> newChildren)
        {
            if (newChildren == null)
            {
                throw new ArgumentNullException("newChildren");
            }
            foreach (RibbonElement element in newChildren)
            {
                this.AppendChild<RibbonElement>(element);
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
            foreach (RibbonElement element in newChildren)
            {
                this.AppendChild<RibbonElement>(element);
            }
        }

        /// <summary>
        /// 添加指定类型的子元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="newChild"></param>
        /// <returns></returns>
        public virtual T AppendChild<T>(T newChild) where T : RibbonElement
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
            this.Parent.RemoveChild<RibbonElement>(this);
        }

        /// <summary>
        /// 删除所有的子元素
        /// </summary>
        public abstract void RemoveAllChildren();

        /// <summary>
        /// 删除指定类型的所有子元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RemoveAllChildren<T>() where T : RibbonElement
        {
            RibbonElement element2;
            for (RibbonElement element = this.FirstChild; element != null; element = element2)
            {
                element2 = element.NextSibling();
                if (element is T)
                {
                    this.RemoveChild<RibbonElement>(element);
                }
            }
        }

        /// <summary>
        /// 删除指定类型的子元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="oldChild"></param>
        /// <returns></returns>
        public virtual T RemoveChild<T>(T oldChild) where T : RibbonElement
        {
            throw new InvalidOperationException();
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
        IEnumerator<RibbonElement> IEnumerable<RibbonElement>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 初始化元素属性
        /// </summary>
        /// <param name="element"></param>
        /// <param name="bundle"></param>
        /// <returns></returns>
        public abstract bool InitProperties(XElement element, IBundle bundle);
        #endregion

        #region 序列化与反序列化
        /// <summary>
        /// 序列化为功能区标准的字符串
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            if (this is XRibbon)
            {
                try
                {
                    StringBuilder ribbonXml = new StringBuilder();
                    ribbonXml.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>")
                        .AppendLine("<customUI xmlns=\"http://schemas.microsoft.com/office/2006/01/customui\" onLoad=\"Ribbon_Load\">")
                        .AppendLine(InternalSerialize(this).ToString())
                        .AppendLine("</customUI>");

                    //XDocument ribbonDocument = XDocument.Parse(ribbonXml.ToString());
                    //XElement xRoot = ribbonDocument.Root;

                    //XElement xRibbonElement = InternalSerialize(this);
                    //xRoot.Add(xRibbonElement);

                    return ribbonXml.ToString();

                }
                catch { }

                return string.Empty;

            }
            return string.Empty;
        }

        /// <summary>
        /// 内部的序列化事件
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private XElement InternalSerialize(RibbonElement element)
        {
            // 序列化当前功能区元素对象
            XElement xElement = element.OnSerialize();
            // 序列化子元素
            if (!element.HasChildren) return xElement;
            foreach (RibbonElement childElement in element.ChildElements)
            {
                XElement xChildElement = InternalSerialize(childElement);
                if (xChildElement != null) xElement.Add(xChildElement);
            }

            return xElement;

        }

        /// <summary>
        /// 序列化处理
        /// </summary>
        /// <returns></returns>
        protected virtual XElement OnSerialize() 
        {
            return null;
        }

        #endregion

        #region IRibbonElement 成员
        /// <summary>
        /// 获取office 插件对象
        /// </summary>
        public AddIn AddIn
        {
            get { return this.addIn; }
            internal set { this.addIn = value; }
        }

        /// <summary>
        /// 刷新整个Ribbon UI
        /// </summary>
        public void Invalidate()
        {
            XRibbon xRibbon = this.XRibbon;
            if (xRibbon != null && xRibbon.RibbonUI != null)
            {
                xRibbon.RibbonUI.Invalidate();
            }
        }

        /// <summary>
        /// 按用户指定的按钮标识，刷新该按钮
        /// </summary>
        /// <param name="ControlID"></param>
        public void InvalidateControl(string ControlID)
        {
            XRibbon xRibbon = this.XRibbon;
            if (xRibbon != null && xRibbon.RibbonUI != null)
            {
                xRibbon.RibbonUI.InvalidateControl(ControlID);
            }
        }

        #endregion

        #region IDisposable 成员

        public void Dispose()
        {
            if (!closed)
            {
                this.Close();
                closed = true;
            }
        }

        /// <summary>
        /// 关闭对象，释放资源
        /// </summary>
        public virtual void Close() { }

        #endregion
    }
}

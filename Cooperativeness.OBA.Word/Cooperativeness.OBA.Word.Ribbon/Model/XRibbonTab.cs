using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Cooperativeness.OBA.Word.AddIns.FrameWork;
using Cooperativeness.OBA.Word.Ribbon.Command;

namespace Cooperativeness.OBA.Word.Ribbon.Model
{
    /// <summary>
    /// 定义功能区页签元素对象
    /// </summary>
    public class XRibbonTab : CompositeRibbonElement
    {
        #region 常量
        private const string GetTabKeyTipString = "GetTabKeytip";
        private const string GetTabVisibleString = "GetTabVisible";
        private const string GetTabLableString = "GetTabLabel";

        #endregion

        #region 字段
        private RibbonTabCommand command;

        #endregion

        #region 属性
        /// <summary>
        /// 获取当前页签的唯一标识
        /// </summary>
        public string Id { get; internal set; }

        /// <summary>
        /// 获取快捷键提示信息
        /// </summary>
        public string Keytip
        {
            get
            {
                CheckValid();
                return command.GetKeytip();
            }
        }

        /// <summary>
        /// 获取一个值，用来指示当前页签是否可见
        /// </summary>
        public bool Visible
        {
            get
            {
                CheckValid();
                return command.GetVisible();
            }
        }

        /// <summary>
        /// 获取当前页签的显示名称
        /// </summary>
        public string Label
        {
            get
            {
                CheckValid();
                return command.GetLabel();
            }
        }

        #endregion

        #region 构造方法
        public XRibbonTab()
        {
        }

        public XRibbonTab(params RibbonElement[] childElements)
            : base(childElements)
        {
        }

        public XRibbonTab(IEnumerable<RibbonElement> childElements)
            : base(childElements)
        {
        }

        #endregion

        #region 方法
        /// <summary>
        /// 检查是否合法
        /// </summary>
        private void CheckValid()
        {
            if (command == null)
                throw new InvalidOperationException("");
        }

        /// <summary>
        /// 根据XML对象设置当前元素的属性值
        /// </summary>
        /// <param name="element"></param>
        public override bool InitProperties(XElement element, IBundle bundle)
        {
            bool success = false;
            // 获取页签的唯一标识
            this.Id = element.AttibuteStringValue("id");
            if (string.IsNullOrEmpty(this.Id)) return success;
            // 获取页签名列类型
            string className = element.AttibuteStringValue("type");
            if(string.IsNullOrEmpty(className)) return success;
            // 创建页签的命令类型
            this.command = RibbonUtils.NewInstance<RibbonTabCommand>(className, bundle);
            if (this.command == null) return success;
            this.command.XRibbonElement = this;

            return true;
        }

        /// <summary>
        /// 序列化处理
        /// </summary>
        /// <returns></returns>
        protected override XElement OnSerialize()
        {
            try
            {
                string xml = "<tab id=\"\" getVisible=\"\"  getKeytip=\"\" getLabel=\"\"/>";
                XElement element = XElement.Parse(xml);
                element.Attribute("id").SetValue(this.Id);
                element.Attribute("getVisible").SetValue(GetTabVisibleString);
                element.Attribute("getKeytip").SetValue(GetTabKeyTipString);
                element.Attribute("getLabel").SetValue(GetTabLableString);

                return element;
            }
            catch { }

            return null;
        }

        /// <summary>
        /// 关闭操作
        /// </summary>
        public override void Close()
        {
            if (this.HasChildren)
            {
                foreach (RibbonElement childElement in this.ChildElements)
                {
                    childElement.Close();
                }
            }

        }
        #endregion
    }
}

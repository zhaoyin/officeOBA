using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Cooperativeness.OBA.Word.AddIns.FrameWork;
using Cooperativeness.OBA.Word.Ribbon.Command;
using Microsoft.Office.Core;
using System.Drawing;

namespace Cooperativeness.OBA.Word.Ribbon.Model
{
    /// <summary>
    /// 定义功能区菜单按钮元素对象
    /// </summary>
    public class XRibbonMenu  : CompositeRibbonElement
    {
        #region 常量
        private const string GetMenuLabelString = "GetMenuLabel";
        private const string GetMenuKeytipString = "GetMenuKeytip";
        private const string GetMenuVisibleString = "GetMenuVisible";
        private const string GetMenuEnabledString = "GetMenuEnabled";
        private const string GetMenuScreenTipString = "GetMenuScreenTip";
        private const string GetMenuShowImageString = "GetMenuShowImage";
        private const string GetMenuImageString = "GetMenuImage";
        private const string GetMenuShowLabelString = "GetMenuShowLabel";
        private const string GetMenuSuperTipString = "GetMenuSuperTip";
        private const string GetMenuSizeString = "GetMenuSize";

        #endregion

        #region 字段
        private RibbonMenuCommand command;

        #endregion

        #region 属性
        /// <summary>
        /// 获取当前按钮的唯一标识
        /// </summary>
        public string Id { get; internal set; }

        /// <summary>
        /// 获取图片库标识
        /// </summary>
        public string ImageMso { get; internal set; }

        /// <summary>
        /// 获取用户自定义图片
        /// </summary>
        public string IconName { get; private set; }

        /// <summary>
        /// 获取用户自定义的图片对象
        /// </summary>
        public Bitmap Image { get; private set; }

        /// <summary>
        /// 获取用户自定义图片加载模式
        /// </summary>
        public ResourceLoadMode LoadMode { get; private set; }

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
        /// 获取屏幕提示信息
        /// </summary>
        public string Screentip
        {
            get
            {
                CheckValid();
                return command.GetScreentip();
            }
        }

        /// <summary>
        /// 获取上级提示信息
        /// </summary>
        public string Supertip
        {
            get
            {
                CheckValid();
                return command.GetSupertip();
            }
        }

        /// <summary>
        /// 获取一个值，用来指示当前按钮是否可见
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
        /// 获取一个值，用来指示当前按钮是否可用
        /// </summary>
        public bool Enabled
        {
            get
            {
                CheckValid();
                return command.GetEnabled();
            }
        }

        /// <summary>
        /// 获取一个值，用来指示当前按钮是否显示标题
        /// </summary>
        public bool ShowLabel
        {
            get
            {
                CheckValid();
                return command.GetShowLabel();
            }
        }

        /// <summary>
        /// 获取一个值，用来指示当前按钮是否显示图片
        /// </summary>
        public bool ShowImage
        {
            get
            {
                CheckValid();
                return command.GetShowImage();
            }
        }

        /// <summary>
        /// 获取当前按钮的显示名称
        /// </summary>
        public string Label
        {
            get
            {
                CheckValid();
                return command.GetLabel();
            }
        }

        /// <summary>
        /// 获取按钮的描述信息
        /// </summary>
        public string Description
        {
            get
            {
                CheckValid();
                return command.GetDescription();
            }
        }

        /// <summary>
        /// 获取当前按钮的大小模式
        /// </summary>
        public RibbonControlSize ControlSize
        {
            get
            {
                CheckValid();
                RibbonSizeMode sizeMode = command.GetSizeMode();
                if (sizeMode == RibbonSizeMode.SizeLarge)
                    return RibbonControlSize.RibbonControlSizeLarge;
                else
                    return RibbonControlSize.RibbonControlSizeRegular;
            }
        }

        #endregion

        #region 构造方法
        public XRibbonMenu()
        {
        }

        public XRibbonMenu(params RibbonElement[] childElements)
            : base(childElements)
        {
        }

        public XRibbonMenu(IEnumerable<RibbonElement> childElements)
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
            // 获取图片库
            this.ImageMso = element.AttibuteStringValue("imageMso");
            // 获取用户自定义图片
            this.IconName = element.AttibuteStringValue("iconName");
            // 获取用户自定义图片加载类型
            this.LoadMode = element.AttributeEnumValue<ResourceLoadMode>("loadMode", ResourceLoadMode.Local);
            // 加载用户图片资源
            if (string.IsNullOrEmpty(this.ImageMso)
                && !string.IsNullOrEmpty(this.IconName))
            {
                this.Image = RibbonUtils.LoadImage(this.IconName, this.LoadMode, bundle);
            }
            // 获取页签名列类型
            string className = element.AttibuteStringValue("type");
            if (string.IsNullOrEmpty(className)) return success;
            // 创建页签的命令类型
            this.command = RibbonUtils.NewInstance<RibbonMenuCommand>(className, bundle);
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

                string xml = "<menu id=\"\" getEnabled=\"\" getScreentip=\"\" "
                                + "getKeytip=\"\" getLabel=\"\" getSize=\"\" getVisible=\"\" getSupertip=\"\" getShowImage=\"\" getShowLabel=\"\"/>";
                XElement element = XElement.Parse(xml);
                element.Attribute("id").SetValue(this.Id);
                if (!string.IsNullOrEmpty(this.ImageMso))
                {
                    XAttribute xAttriute = new XAttribute("imageMso", this.ImageMso); ;
                    element.Add(xAttriute);
                }
                else if (!string.IsNullOrEmpty(this.IconName))
                {
                    XAttribute xAttriute = new XAttribute("getImage", GetMenuImageString);
                    element.Add(xAttriute);
                }
                element.Attribute("getKeytip").SetValue(GetMenuKeytipString);
                element.Attribute("getLabel").SetValue(GetMenuLabelString);
                element.Attribute("getScreentip").SetValue(GetMenuScreenTipString);
                element.Attribute("getSupertip").SetValue(GetMenuSuperTipString);
                element.Attribute("getVisible").SetValue(GetMenuVisibleString);
                element.Attribute("getEnabled").SetValue(GetMenuEnabledString);
                element.Attribute("getShowImage").SetValue(GetMenuShowImageString);
                element.Attribute("getShowLabel").SetValue(GetMenuShowLabelString);
                element.Attribute("getSize").SetValue(GetMenuSizeString);
       

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
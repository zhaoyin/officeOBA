using System;
using System.Xml.Linq;
using Cooperativeness.OBA.Word.AddIns.FrameWork;
using Cooperativeness.OBA.Word.Ribbon.Command;
using Microsoft.Office.Core;

namespace Cooperativeness.OBA.Word.Ribbon.Model
{
    /// <summary>
    /// 定义功能区开关按钮元素对象
    /// </summary>
    public class XRibbonToggleButton : LeafRibbonElement
    {
        #region 常量
        private const string GetToggleButtonKeytipString = "GetToggleButtonKeytip";
        private const string GetToggleButtonVisibleString = "GetToggleButtonVisible";
        private const string GetToggleButtonLabelString = "GetToggleButtonLabel";
        private const string GetToggleButtonEnabledString = "GetToggleButtonEnabled";
        private const string GetToggleButtonScreenTipString = "GetToggleButtonScreenTip";
        private const string GetToggleButtonShowImageString = "GetToggleButtonShowImage";
        private const string GetToggleButtonShowLabelString = "GetToggleButtonShowLabel";
        private const string GetToggleButtonSuperTipString = "GetToggleButtonSuperTip";
        private const string GetToggleButtonSizeString = "GetToggleButtonSize";
        private const string GetToggleButtonPressedString = "GetToggleButtonPressed";
        private const string OnToggleButtonActionString = "OnToggleButtonAction";

        #endregion

        #region 字段
        private RibbonToggleButtonCommand command;

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
        /// 获取当前按钮是否处于按下状态
        /// </summary>
        public bool Pressed
        {
            get
            {
                CheckValid();
                return command.GetPressed();
            }
        }

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
            // 获取按钮的唯一标识
            this.Id = element.AttibuteStringValue("id");
            if (string.IsNullOrEmpty(this.Id)) return success;
            // 获取图片库
            this.ImageMso = element.AttibuteStringValue("imageMso");
            // 获取按钮命令类型
            string className = element.AttibuteStringValue("type");
            if (string.IsNullOrEmpty(className)) return success;
            // 创建页签的命令类型
            this.command = RibbonUtils.NewInstance<RibbonToggleButtonCommand>(className, bundle);
            if (this.command == null) return success;
            this.command.XRibbonElement = this;

            return true;
        }

        /// <summary>
        /// 执行相应动作
        /// </summary>
        /// <param name="e"></param>
        internal void OnAction(RibbonEventArgs e)
        {
            CheckValid();

            command.OnAction(this, e);
        }

        /// <summary>
        /// 序列化处理
        /// </summary>
        /// <returns></returns>
        protected override XElement OnSerialize()
        {
            try
            {
                string xml = "<toggleButton id=\"\" imageMso=\"\" getEnabled=\"\" onAction =\"\" getScreentip=\"\" "
                                + "getKeytip=\"\" getLabel=\"\" getSize=\"\" getVisible=\"\" getSupertip=\"\" getShowImage=\"\" getShowLabel=\"\"/>";
                XElement element = XElement.Parse(xml);
                element.Attribute("id").SetValue(this.Id);
                element.Attribute("imageMso").SetValue(this.ImageMso);
                element.Attribute("getKeytip").SetValue(GetToggleButtonKeytipString);
                element.Attribute("getLabel").SetValue(GetToggleButtonLabelString);
                element.Attribute("getScreentip").SetValue(GetToggleButtonScreenTipString);
                element.Attribute("getSupertip").SetValue(GetToggleButtonSuperTipString);
                element.Attribute("getVisible").SetValue(GetToggleButtonVisibleString);
                element.Attribute("getEnabled").SetValue(GetToggleButtonEnabledString);
                element.Attribute("getShowImage").SetValue(GetToggleButtonShowImageString);
                element.Attribute("getShowLabel").SetValue(GetToggleButtonShowLabelString);
                element.Attribute("getSize").SetValue(GetToggleButtonSizeString);
                element.Attribute("onAction").SetValue(OnToggleButtonActionString);


                return element;
            }
            catch { }

            return null;
        }
        #endregion
    }
}
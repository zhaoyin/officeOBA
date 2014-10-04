using System;
using System.Xml.Linq;
using Cooperativeness.OBA.Word.AddIns.FrameWork;
using Cooperativeness.OBA.Word.Ribbon.Command;
using Cooperativeness.OBA.Word.Tools;
using Microsoft.Office.Core;
using System.Drawing;

namespace Cooperativeness.OBA.Word.Ribbon.Model
{
    /// <summary>
    /// 定义功能区按钮元素对象
    /// </summary>
    public class XRibbonButton : LeafRibbonElement
    {
        private static readonly Logger Log=new Logger(typeof(XRibbonButton));
        #region 常量
        private const string GetButtonKeyTipString = "GetButtonKeytip";
        private const string GetButtonScreenTipString = "GetButtonScreenTip";
        private const string GetButtonSuperTipString = "GetButtonSuperTip";
        private const string GetButtonVisibleString = "GetButtonVisible";
        private const string GetButtonLabelString = "GetButtonLabel";
        private const string GetButtonEnabledString = "GetButtonEnabled";
        private const string GetButtonShowImageString = "GetButtonShowImage";
        private const string GetButtonShowLabelString = "GetButtonShowLabel";       
        private const string GetButtonSizeString = "GetButtonSize";
        private const string OnButtonActionString = "OnButtonAction";
        private const string GetButtonImageString = "GetButtonImage";

        #endregion

        #region 字段
        private RibbonButtonCommand _command;

        #endregion

        #region 属性
        /// <summary>
        /// 获取当前按钮的唯一标识
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// 获取图片库标识
        /// </summary>
        public string ImageMso { get; private set; }

        /// <summary>
        /// 获取用户自定义图片
        /// </summary>
        public string IconName { get; private set; }

        /// <summary>
        /// 获取用户自定义的图片对象
        /// </summary>
        public Bitmap Image{ get; private set; }

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
                return _command.GetKeytip();
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
                return _command.GetScreentip();
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
                return _command.GetSupertip();
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
                return _command.GetVisible();
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
                return _command.GetEnabled();
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
                return _command.GetShowLabel();
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
                return _command.GetShowImage();
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
                return _command.GetLabel();
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
                return _command.GetDescription();
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
                RibbonSizeMode sizeMode = _command.GetSizeMode();
                if (sizeMode == RibbonSizeMode.SizeLarge)
                    return RibbonControlSize.RibbonControlSizeLarge;
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
            if (_command == null)
                throw new InvalidOperationException("");
        }

        /// <summary>
        /// 根据XML对象设置当前元素的属性值
        /// </summary>
        /// <param name="element"></param>
        /// <param name="bundle"></param>
        public override bool InitProperties(XElement element, IBundle bundle)
        {
            // 获取按钮的唯一标识
            Id = element.AttibuteStringValue("id");
            if (string.IsNullOrEmpty(this.Id)) return false;
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
            // 获取按钮命令类型
            string className = element.AttibuteStringValue("type");
            if (string.IsNullOrEmpty(className)) return false;
            // 创建页签的命令类型
            _command = RibbonUtils.NewInstance<RibbonButtonCommand>(className, bundle);
            if (_command == null) return false;
            _command.XRibbonElement = this;

            return true;
        }

        /// <summary>
        /// 执行相应动作
        /// </summary>
        /// <param name="e"></param>
        internal void OnAction(RibbonEventArgs e)
        {
            CheckValid();

            _command.OnAction(this, e);
        }

        /// <summary>
        /// 序列化处理
        /// </summary>
        /// <returns></returns>
        protected override XElement OnSerialize()
        {
            try
            {
                string xml = "<button id=\"\" getEnabled=\"\" onAction =\"\" getScreentip=\"\" "
                             +
                             "getKeytip=\"\" getLabel=\"\" getVisible=\"\" getSupertip=\"\" getShowImage=\"\" getShowLabel=\"\"/>";
                XElement element = XElement.Parse(xml);
                element.Attribute("id").SetValue(this.Id);
                if (!string.IsNullOrEmpty(this.ImageMso))
                {
                    var xAttriute = new XAttribute("imageMso", this.ImageMso);
                    element.Add(xAttriute);
                }
                else if (!string.IsNullOrEmpty(this.IconName))
                {
                    var xAttriute = new XAttribute("getImage", GetButtonImageString);
                    element.Add(xAttriute);
                }
                element.Attribute("getKeytip").SetValue(GetButtonKeyTipString);
                element.Attribute("getLabel").SetValue(GetButtonLabelString);
                element.Attribute("getScreentip").SetValue(GetButtonScreenTipString);
                element.Attribute("getSupertip").SetValue(GetButtonSuperTipString);
                element.Attribute("getVisible").SetValue(GetButtonVisibleString);
                element.Attribute("getEnabled").SetValue(GetButtonEnabledString);
                element.Attribute("getShowImage").SetValue(GetButtonShowImageString);
                element.Attribute("getShowLabel").SetValue(GetButtonShowLabelString);
                element.Attribute("onAction").SetValue(OnButtonActionString);
                if (!(this.Parent is XRibbonMenu))
                {
                    var xSizeAttriute = new XAttribute("getSize", GetButtonSizeString);
                    element.Add(xSizeAttriute);
                }


                return element;
            }
            catch (Exception e)
            {
                Log.Debug(e);
            }

            return null;
        }

        /// <summary>
        /// 关闭操作
        /// </summary>
        public override void Close()
        {
            base.Close();
            if (this.Image != null)
                this.Image.Dispose();
        }
        #endregion
    }
}

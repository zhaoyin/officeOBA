using System.Xml.Linq;
using Cooperativeness.OBA.Word.AddIns.FrameWork;
using Cooperativeness.OBA.Word.Ribbon.Command;

namespace Cooperativeness.OBA.Word.Ribbon.Model
{
    /// <summary>
    /// 定义功能区分隔符元素对象
    /// </summary>
    public class XRibbonSeparator : LeafRibbonElement
    {
        #region 常量
        private const string GetSeparatorVisibleString = "GetSeparatorVisible";

        #endregion

        #region 字段
        private IRibbonSeparatorCommand command;

        #endregion

        #region 属性
        /// <summary>
        /// 获取当前按钮的唯一标识
        /// </summary>
        public string Id { get; internal set; }

        /// <summary>
        /// 获取一个值，用来指示当前按钮是否可见
        /// </summary>
        public bool Visible
        {
            get
            {
                return command.GetVisible();
            }
        }
        #endregion

        #region 方法
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
            // 获取按钮命令类型
            string className = element.AttibuteStringValue("type");
            if (!string.IsNullOrEmpty(className))
            {
                // 创建页签的命令类型
                this.command = RibbonUtils.NewInstance<RibbonSeparatorCommand>(className, bundle);
                if (this.command == null)
                {
                    // 创建页签的命令类型
                    this.command = RibbonUtils.NewInstance<RibbonSeparatorCommand>();
                }
            }
            else
            {
                // 创建页签的命令类型
                this.command = RibbonUtils.NewInstance<RibbonSeparatorCommand>();
            }

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
                string xml = "<separator id=\"\" getVisible=\"\"/>";
                XElement element = XElement.Parse(xml);
                element.Attribute("id").SetValue(this.Id);
                element.Attribute("getVisible").SetValue(GetSeparatorVisibleString);

                return element;
            }
            catch { }

            return null;
        }
        #endregion
    }
}
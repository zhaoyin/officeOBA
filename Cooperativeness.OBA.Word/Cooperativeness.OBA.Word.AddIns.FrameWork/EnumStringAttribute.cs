using System;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork
{
    /// <summary>
    /// 定义枚举属性对象
    /// </summary>
    public sealed class EnumStringAttribute : Attribute
    {
        public EnumStringAttribute(string value)
        {
            this.Value = value;
        }

        public string Value { get; private set; }
    }
}

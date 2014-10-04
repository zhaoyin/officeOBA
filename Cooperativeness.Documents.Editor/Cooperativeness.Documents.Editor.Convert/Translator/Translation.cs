/*
 * 由SharpDevelop创建。
 * 用户： zhaoyin3
 * 日期: 2014-09-04
 * 时间: 13:40
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;

namespace Cooperativeness.Documents.Editor.Converter.Translator
{
	/// <summary>
	/// Description of Translation.
	/// </summary>
    public partial class Translation {
        
        private String _Text;
        
        public String Text {
            get {
                return this._Text;
            }
            set {
                this._Text = value;
            }
        }
    }
}

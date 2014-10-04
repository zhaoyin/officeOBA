/*
 * 由SharpDevelop创建。
 * 用户： zhaoyin3
 * 日期: 2014-09-05
 * 时间: 11:30
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Windows.Controls;

namespace Cooperativeness.Documents.Editor.FTP
{
	/// <summary>
	/// Description of FTPItem.
	/// </summary>
	internal class FTPItem : ListBoxItem
	{
		public string Name;
		public string FileName;
		public string FullName;
		public bool IsFile = false;
	}
}

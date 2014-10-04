using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MS.WindowsAPICodePack.Internal.CoreHelpers;
namespace Document.Editor
{
	public partial class SaveFileDialog
	{

		public string Res = null;
		public void SetFileInfo(string name, RichTextBox RTB)
		{
			Label1.Content = "Do you want to save " + name + "?";
			RichTextBox1.AppendText("");
		}

		#region "Buttons"

		private void YesButton_Click(System.Object sender, System.Windows.RoutedEventArgs e)
		{
			Res = "Yes";
			Close();
		}

		private void NoButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Res = "No";
			Close();
		}

		private void CancelButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Res = null;
			Close();
		}

		#endregion

		private void SaveFileDialog_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (this.WindowState == System.Windows.WindowState.Maximized) {
				Document.Editor.My.Settings.SaveDialog_IsMax = true;
			} else {
				Document.Editor.My.Settings.SaveDialog_IsMax = false;
			}
		}

		private void SaveFileDialog_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			if (Document.Editor.My.Computer.Info.OSVersion >= "6.0") {
				if (Document.Editor.My.Settings.Options_EnableGlass) {
					AppHelper.ExtendGlassFrame(this, new Thickness(-1, -1, -1, -1));
				}
			}
			System.IO.FileStream fs = System.IO.File.OpenRead(Document.Editor.My.Computer.FileSystem.SpecialDirectories.Temp + "\\TVPre.xaml");
			TextRange tr = new TextRange(RichTextBox1.Document.ContentStart, RichTextBox1.Document.ContentEnd);
			FlowDocument content = System.Windows.Markup.XamlReader.Load(fs) as FlowDocument;
			RichTextBox1.Document = content;
			fs.Close();
			if (Document.Editor.My.Settings.SaveDialog_IsMax) {
				this.WindowState = System.Windows.WindowState.Maximized;
			} else {
				this.WindowState = System.Windows.WindowState.Normal;
			}
		}
	}
}

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
	public partial class LinkDialog
	{
		public string Res = "Cancel";

		public string Link = null;
		private void OKButton_Click(System.Object sender, System.Windows.RoutedEventArgs e)
		{
			Link = TextBox1.Text;
			Res = "OK";
			Close();
		}

		private void TextBox1_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == Key.Enter && TextBox1.Text.Length > 0) {
				OKButton_Click(null, null);
			}
		}

		private void TextBox1_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			if (TextBox1.Text != null) {
				OKButton.IsEnabled = true;
			} else {
				OKButton.IsEnabled = false;
			}
		}

		private void LinkDialog_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			if (Document.Editor.My.Computer.Info.OSVersion >= "6.0") {
				if (Document.Editor.My.Settings.Options_EnableGlass) {
					AppHelper.ExtendGlassFrame(this, new Thickness(-1, -1, -1, -1));
				}
			}
			TextBox1.Focus();
		}
		public LinkDialog()
		{
			Loaded += LinkDialog_Loaded;
		}
	}
}

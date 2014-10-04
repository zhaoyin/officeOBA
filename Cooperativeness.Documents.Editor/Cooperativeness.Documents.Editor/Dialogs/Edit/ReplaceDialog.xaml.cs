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
namespace Document.Editor
{
	public partial class ReplaceDialog
	{

		public string Res = "Cancel";
		private void ReplaceDialog_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			if (Document.Editor.My.Computer.Info.OSVersion >= "6.0") {
				if (Document.Editor.My.Settings.Options_EnableGlass) {
					AppHelper.ExtendGlassFrame(this, new Thickness(-1, -1, -1, -1));
				}
			}
			TextBox1.Focus();
		}

		private void OKButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Res = "OK";
			Close();
		}

		private void TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == Key.Enter) {
				if (OKButton.IsEnabled) {
					OKButton_Click(null, null);
				}
			}
		}

		private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			if (TextBox1.Text.Length > 0 && TextBox2.Text.Length > 0) {
				OKButton.IsEnabled = true;
			} else {
				OKButton.IsEnabled = false;
			}
		}
		public ReplaceDialog()
		{
			Loaded += ReplaceDialog_Loaded;
		}
	}
}

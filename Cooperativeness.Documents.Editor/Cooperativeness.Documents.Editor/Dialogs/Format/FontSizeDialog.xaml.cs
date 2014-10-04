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
	public partial class FontSizeDialog
	{
		public string Res = "Cancel";

		public double Number = new double();
		private void SizeBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == Key.Enter) {
				OKButton_Click(null, null);
			}
		}

		private void OKButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Number = Convert.ToDouble(SizeBox.Text);
			Res = "OK";
			Close();
		}

		private void FontSizeDialog_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			if (Document.Editor.My.Computer.Info.OSVersion >= "6.0") {
				if (Document.Editor.My.Settings.Options_EnableGlass) {
					AppHelper.ExtendGlassFrame(this, new Thickness(-1, -1, -1, -1));
				}
			}
			SizeBox.Focus();
		}
		public FontSizeDialog()
		{
			Loaded += FontSizeDialog_Loaded;
		}
	}
}

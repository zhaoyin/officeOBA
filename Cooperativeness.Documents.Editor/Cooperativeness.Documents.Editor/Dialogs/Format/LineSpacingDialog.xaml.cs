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
	public partial class LineSpacingDialog
	{
		public string Res = "Cancel";

		public double number;
		private void TextBox1_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == Key.Enter) {
				if (OKButton.IsEnabled) {
					OKButton_Click(null, null);
				}
			}
		}

		private void OKButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Res = "OK";
			Close();
		}

		private void LineSpacingDialog_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			if (Document.Editor.My.Computer.Info.OSVersion >= "6.0") {
				if (Document.Editor.My.Settings.Options_EnableGlass) {
					AppHelper.ExtendGlassFrame(this, new Thickness(-1, -1, -1, -1));
				}
			}
			TextBox1.Value = number;
			TextBox1.Focus();
		}

		private void TextBox1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			try {
				number = TextBox1.Value;
			} catch (Exception ex) {
			}
		}
		public LineSpacingDialog()
		{
			Loaded += LineSpacingDialog_Loaded;
		}
	}
}

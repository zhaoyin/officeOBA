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
	public partial class VideoDialog
	{
		public string Res = "Cancel";
		public int w = null;

		public int h = null;
		private void Window_Loaded(System.Object sender, System.Windows.RoutedEventArgs e)
		{
			if (Document.Editor.My.Computer.Info.OSVersion >= "6.0") {
				if (Document.Editor.My.Settings.Options_EnableGlass) {
					AppHelper.ExtendGlassFrame(this, new Thickness(-1, -1, -1, -1));
				}
			}
		}

		private void OKButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Res = "OK";
			Close();
		}

		private void TextBox1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			try {
				w = Convert.ToDouble(TextBox1.Value);
			} catch (Exception ex) {
				TextBox1.Value = 1;
			}
			try {
				h = Convert.ToDouble(TextBox2.Text);
			} catch (Exception ex) {
				TextBox2.Value = 1;
			}
			if (TextBox1.Value > 0 && TextBox2.Value > 0) {
				OKButton.IsEnabled = true;
			} else {
				OKButton.IsEnabled = false;
			}
		}
		public VideoDialog()
		{
			Loaded += Window_Loaded;
		}
	}
}

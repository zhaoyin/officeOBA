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
	public partial class GoToDialog
	{
		public string Res = "Cancel";

		public int line = 1;
		private void OKButton_Click(System.Object sender, System.Windows.RoutedEventArgs e)
		{
			//line = Convert.ToInt16(LineNumberBox.Value)
			Res = "OK";
			Close();
		}

		private void GoToDialog_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			if (Document.Editor.My.Computer.Info.OSVersion >= "6.0") {
				if (Document.Editor.My.Settings.Options_EnableGlass) {
					AppHelper.ExtendGlassFrame(this, new Thickness(-1, -1, -1, -1));
				}
			}
			LineNumberBox.Focus();
			LineNumberBox_ValueChanged(null, null);
		}

		private void LineNumberBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter && OKButton.IsEnabled) {
				OKButton_Click(null, null);
			}
		}

		private void LineNumberBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			try {
				line = Convert.ToInt32(LineNumberBox.Value);
				// - 1

			} catch (Exception ex) {
			}
		}
		public GoToDialog()
		{
			Loaded += GoToDialog_Loaded;
		}
	}
}

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
	public partial class FontDialog
	{
		public string Res = "Cancel";

		public FontFamily font = new FontFamily();
		private void OKButton_Click(System.Object sender, System.Windows.RoutedEventArgs e)
		{
			font = FontListBox.SelectedItem;
			Res = "OK";
			Close();
		}

		private void FontListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			if (FontListBox.SelectedItem != null) {
				OKButton.IsEnabled = true;
			} else {
				OKButton.IsEnabled = false;
			}
		}

		private void FontDialog_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			if (Document.Editor.My.Computer.Info.OSVersion >= "6.0") {
				if (Document.Editor.My.Settings.Options_EnableGlass) {
					AppHelper.ExtendGlassFrame(this, new Thickness(-1, -1, -1, -1));
				}
			}
		}
		public FontDialog()
		{
			Loaded += FontDialog_Loaded;
		}
	}
}

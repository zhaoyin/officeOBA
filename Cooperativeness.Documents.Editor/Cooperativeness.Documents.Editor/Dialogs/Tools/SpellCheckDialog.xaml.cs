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
	public partial class SpellCheckDialog
	{

		public string Res = "Cancel";
		private void SpellCheckDialog_Loaded(object sender, System.Windows.RoutedEventArgs e)
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

		private void WordListBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == Key.Enter & WordListBox.SelectedItem != null) {
				OKButton_Click(null, null);
			}
		}

		private void WordListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			if (WordListBox.SelectedItem != null) {
				OKButton.IsEnabled = true;
			} else {
				OKButton.IsEnabled = false;
			}
		}
		public SpellCheckDialog()
		{
			Loaded += SpellCheckDialog_Loaded;
		}
	}
}

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
	public partial class TimeDialog
	{
		public string Res = "Cancel";

		public string Time = null;
		private void OKButton_Click(System.Object sender, System.Windows.RoutedEventArgs e)
		{
			Res = "OK";
			Close();
		}

		private void RadioButton12_Checked(object sender, System.Windows.RoutedEventArgs e)
		{
			if (AMPMCheckBox != null) {
				AMPMCheckBox.IsEnabled = true;
			}
		}

		private void RadioButton12_Unchecked(object sender, System.Windows.RoutedEventArgs e)
		{
			AMPMCheckBox.IsEnabled = false;
		}

		private void TimeDialog_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			if (Document.Editor.My.Computer.Info.OSVersion >= "6.0") {
				if (Document.Editor.My.Settings.Options_EnableGlass) {
					AppHelper.ExtendGlassFrame(this, new Thickness(-1, -1, -1, -1));
				}
			}
		}
		public TimeDialog()
		{
			Loaded += TimeDialog_Loaded;
		}
	}
}

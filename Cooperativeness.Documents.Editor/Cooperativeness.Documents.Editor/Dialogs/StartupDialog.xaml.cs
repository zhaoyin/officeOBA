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
	public partial class StartupDialog
	{

		private void Window_Loaded(System.Object sender, System.Windows.RoutedEventArgs e)
		{
			if (Document.Editor.My.Computer.Info.OSVersion >= "6.0") {
				if (Document.Editor.My.Settings.Options_EnableGlass) {
					AppHelper.ExtendGlassFrame(this, new Thickness(-1, -1, -1, -1));
				}
			}
			if (Document.Editor.My.Settings.Options_ShowStartupDialog) {
				ShowOnStartupCheckBox.IsChecked = true;
			} else {
				ShowOnStartupCheckBox.IsChecked = false;
			}
		}

		private void OnlineHelpButton_Click(System.Object sender, System.Windows.RoutedEventArgs e)
		{
			Process.Start("http://documenteditor.net/documentation/");
		}

		private void GetPluginsButton_Click(System.Object sender, System.Windows.RoutedEventArgs e)
		{
			Process.Start("http://documenteditor.net/plugins");
		}

		private void WebsiteButton_Click(System.Object sender, System.Windows.RoutedEventArgs e)
		{
			Process.Start("http://documenteditor.net");
		}

		private void CloseButton_Click(System.Object sender, System.Windows.RoutedEventArgs e)
		{
			if (ShowOnStartupCheckBox.IsChecked) {
				Document.Editor.My.Settings.Options_ShowStartupDialog = true;
			} else {
				Document.Editor.My.Settings.Options_ShowStartupDialog = false;
			}
			Close();
		}
		public StartupDialog()
		{
			Loaded += Window_Loaded;
		}
	}
}

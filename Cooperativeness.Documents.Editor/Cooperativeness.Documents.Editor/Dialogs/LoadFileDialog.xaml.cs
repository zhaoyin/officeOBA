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
	public partial class LoadFileDialog
	{


		public bool i = false;
		private void LoadFileDialog_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (i == false) {
				e.Cancel = true;
			}
		}

		private void LoadFileDialog_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			if (Document.Editor.My.Computer.Info.OSVersion >= "6.0") {
				if (Document.Editor.My.Settings.Options_EnableGlass) {
					AppHelper.ExtendGlassFrame(this, new Thickness(-1, -1, -1, -1));
				}
			}
		}
		public LoadFileDialog()
		{
			Loaded += LoadFileDialog_Loaded;
			Closing += LoadFileDialog_Closing;
		}
	}
}

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
	public partial class ObjectPropertiesDialog
	{

		public ObjectPropertiesDialog(int w, int h, string txt)
		{
			Loaded += ObjectPropertiesDialog_Loaded;
			InitializeComponent();
			WBox.Value = w;
			HBox.Value = h;
			TxtBox.Text = txt;
		}

		private void OKButton_Click(System.Object sender, System.Windows.RoutedEventArgs e)
		{
			Close();
		}

		private void ObjectPropertiesDialog_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			if (Document.Editor.My.Computer.Info.OSVersion >= "6.0") {
				if (Document.Editor.My.Settings.Options_EnableGlass) {
					AppHelper.ExtendGlassFrame(this, new Thickness(-1, -1, -1, -1));
				}
			}
		}
	}
}

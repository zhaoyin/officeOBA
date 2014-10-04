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
	public partial class ObjectDialog
	{
		public string Res = null;
		public int OW = 96;
		public int OH = 24;

		public string OT = "Text";
		private void CancelButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Res = null;
		}

		private void OKButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			if (O_Button.IsChecked) {
				Res = "button";
			} else if (O_RadioButton.IsChecked) {
				Res = "radiobutton";
			} else if (O_CheckBox.IsChecked) {
				Res = "checkbox";
			} else if (O_TextBlock.IsChecked) {
				Res = "textblock";
			}
			Close();
		}

		private void Button1_Click(System.Object sender, System.Windows.RoutedEventArgs e)
		{
			ObjectPropertiesDialog op = new ObjectPropertiesDialog(OW, OH, OT);
			op.Owner = this;
			op.ShowDialog();
			OW = Convert.ToInt32(op.WBox.Value);
			OH = Convert.ToInt32(op.HBox.Value);
			OT = op.TxtBox.Text;
		}

		private void ObjectDialog_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			if (Document.Editor.My.Computer.Info.OSVersion >= "6.0") {
				if (Document.Editor.My.Settings.Options_EnableGlass) {
					AppHelper.ExtendGlassFrame(this, new Thickness(-1, -1, -1, -1));
				}
			}
		}
		public ObjectDialog()
		{
			Loaded += ObjectDialog_Loaded;
		}
	}
}

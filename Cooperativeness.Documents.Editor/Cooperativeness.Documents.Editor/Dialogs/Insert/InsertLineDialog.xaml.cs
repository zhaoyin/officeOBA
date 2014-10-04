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
	public partial class InsertLineDialog
	{
		public string Res = "Cancel";

		public int h;
		//Private Sub TextBox1_TextChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TextBox1.TextChanged
		//    Try
		//        h = Convert.ToInt32(TextBox1.Text)
		//    Catch ex As Exception
		//        TextBox1.Clear()
		//    End Try
		//    If TextBox1.Text.Length > 0 Then
		//        OKButton.IsEnabled = True
		//    Else
		//        OKButton.IsEnabled = False
		//    End If
		//End Sub

		private void OKButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			h = Convert.ToInt32(TextBox1.Value);
			Res = "OK";
			Close();
		}

		private void InsertLineDialog_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			if (Document.Editor.My.Computer.Info.OSVersion >= "6.0") {
				if (Document.Editor.My.Settings.Options_EnableGlass) {
					AppHelper.ExtendGlassFrame(this, new Thickness(-1, -1, -1, -1));
				}
			}
			TextBox1.Focus();
		}
		public InsertLineDialog()
		{
			Loaded += InsertLineDialog_Loaded;
		}
	}
}

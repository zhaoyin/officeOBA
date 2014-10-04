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
	public partial class MessageBoxDialog
	{

		public string Result = "Cancel";
		public MessageBoxDialog(string text, string title, string buttons, int icon)
		{
			// This call is required by the designer.
			InitializeComponent();
			// Add any initialization after the InitializeComponent() call.
			MessageBoxText.Text = text;
			this.Title = title;
			if (buttons == "YesNo") {
				OKButton.Visibility = System.Windows.Visibility.Collapsed;
				YesButton.Visibility = System.Windows.Visibility.Visible;
				NoButton.Visibility = System.Windows.Visibility.Visible;
				YesButton.IsDefault = true;
			} else if (buttons == "YesNoCancel") {
				OKButton.Visibility = System.Windows.Visibility.Collapsed;
				YesButton.Visibility = System.Windows.Visibility.Visible;
				NoButton.Visibility = System.Windows.Visibility.Visible;
				CancelButton.Visibility = System.Windows.Visibility.Visible;
				YesButton.IsDefault = true;
			}
		}

		private void OKButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Result = "OK";
			DialogResult = true;
			Close();
		}

		private void YesButton_Click(object sender, RoutedEventArgs e)
		{
			Result = "Yes";
			DialogResult = true;
			Close();
		}

		private void NoButton_Click(object sender, RoutedEventArgs e)
		{
			Result = "No";
			DialogResult = true;
			Close();
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			Result = "Cancel";
			DialogResult = true;
			Close();
		}
	}
}

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
	public partial class TranslateDialog
	{

		private string intContent;

		public bool res = false;
		public TranslateDialog(string s)
		{
			Loaded += TranslateDialog_Loaded;
			InitializeComponent();
			intContent = s;
		}

		private void TranslateButton_Click(System.Object sender, System.Windows.RoutedEventArgs e)
		{
			try {
				if (Document.Editor.My.Computer.Network.IsAvailable) {
					Microsoft.DetectedLanguage froml = new Microsoft.DetectedLanguage();
					Microsoft.Language tol = new Microsoft.Language();
					froml.Code = FromBox.Items.Item(FromBox.SelectedIndex).Tag;
					tol.Code = ToBox.Items.Item(ToBox.SelectedIndex).Tag;
					Semagsoft.Translator.TranslatorHelper translator = new Semagsoft.Translator.TranslatorHelper();
					string transres = translator.Translate(intContent, froml, tol);
					TranslatedText.Content = transres;
					OKButton.IsEnabled = true;
				} else {
					MessageBoxDialog m = new MessageBoxDialog("No Internet Found", "Error", null, null);
					m.MessageImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Common/error32.png"));
					m.Owner = this;
					m.ShowDialog();
				}
			} catch (Exception ex) {
				MessageBoxDialog m = new MessageBoxDialog(ex.Message, ex.ToString(), null, null);
				m.MessageImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Common/error32.png"));
				m.Owner = this;
				m.ShowDialog();
			}
		}

		private void OKButton_Click(System.Object sender, System.Windows.RoutedEventArgs e)
		{
			res = true;
			Close();
		}

		private void TranslateDialog_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			if (Document.Editor.My.Computer.Info.OSVersion >= "6.0") {
				if (Document.Editor.My.Settings.Options_EnableGlass) {
					AppHelper.ExtendGlassFrame(this, new Thickness(-1, -1, -1, -1));
				}
			}
		}
	}
}

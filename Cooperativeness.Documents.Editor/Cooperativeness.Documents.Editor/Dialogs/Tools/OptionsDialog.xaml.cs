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
using System.Collections.ObjectModel;
using System.Speech.Synthesis;
namespace Document.Editor
{
	public partial class OptionsDialog
	{

		private SpeechSynthesizer speech = new SpeechSynthesizer();
		#region "Loaded"

		private void OptionsDialog_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			StartUpComboBox.SelectedIndex = Document.Editor.My.Settings.Options_StartupMode;
			ShowStartupDialogCheckBox.IsChecked = Document.Editor.My.Settings.Options_ShowStartupDialog;
			ThemeComboBox.SelectedIndex = Document.Editor.My.Settings.Options_Theme;
			EnableGlassCheckBox.IsChecked = Document.Editor.My.Settings.Options_EnableGlass;
			ComboBox1.SelectedIndex = Document.Editor.My.Settings.Options_Tabs_SizeMode;
			FontFaceComboBox.SelectedItem = Document.Editor.My.Settings.Options_DefaultFont;
			FontSizeTextBox.Value = Document.Editor.My.Settings.Options_DefaultFontSize;
			SpellCheckBox.IsChecked = Document.Editor.My.Settings.Options_SpellCheck;
			TabPlacementComboBox.SelectedIndex = Document.Editor.My.Settings.Options_TabPlacement;
			if (Document.Editor.My.Computer.Info.OSVersion >= "6.0") {
				if (Document.Editor.My.Settings.Options_EnableGlass) {
					AppHelper.ExtendGlassFrame(this, new Thickness(-1, -1, -1, -1));
				}
			}
			ReadOnlyCollection<InstalledVoice> Voices = speech.GetInstalledVoices(System.Globalization.CultureInfo.CurrentCulture);
			VoiceInfo VoiceInformation = Voices[0].VoiceInfo;
			foreach (InstalledVoice Voice in Voices) {
				VoiceInformation = Voice.VoiceInfo;
				TTSComboBox.Items.Add(VoiceInformation.Name.ToString());
			}
			TTSComboBox.SelectedIndex = Document.Editor.My.Settings.Options_TTSV;
			TTSSlider.Value = Document.Editor.My.Settings.Options_TTSS;
			CloseButtonComboBox.SelectedIndex = Document.Editor.My.Settings.Options_Tabs_CloseButtonMode;
			if (Document.Editor.My.Settings.Options_ShowRecentDocuments) {
				RecentDocumentsCheckBox.IsChecked = true;
			} else {
				RecentDocumentsCheckBox.IsChecked = false;
			}
			RulerMeasurementComboBox.SelectedIndex = Document.Editor.My.Settings.Options_RulerMeasurement;

			foreach (string f in Document.Editor.My.Computer.FileSystem.GetFiles(Document.Editor.My.Application.Info.DirectoryPath + "\\Templates\\")) {
				System.IO.FileInfo template = new System.IO.FileInfo(f);
				if (template.Extension == ".xaml") {
					ListBoxItem item = new ListBoxItem();
					item.Height = 32;
					item.Content = template.Name.Remove(template.Name.Length - 5);
					TemplatesListBox.Items.Add(item);
				}
			}

			if (Document.Editor.My.Settings.Options_EnablePlugins) {
				PluginsCheckBox.IsChecked = true;
			} else {
				PluginsCheckBox.IsChecked = false;
			}
			foreach (string f in Document.Editor.My.Computer.FileSystem.GetFiles(Document.Editor.My.Application.Info.DirectoryPath + "\\Plugins\\")) {
				System.IO.FileInfo plugin = new System.IO.FileInfo(f);
				ListBoxItem item = new ListBoxItem();
				item.Height = 32;
				item.Content = plugin.Name.Remove(plugin.Name.Length - 3);
				PluginsListBox.Items.Add(item);
			}
		}

		#endregion

		#region "DialogButtons"

		private void OKButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Document.Editor.My.Settings.Options_StartupMode = StartUpComboBox.SelectedIndex;
			Document.Editor.My.Settings.Options_ShowStartupDialog = ShowStartupDialogCheckBox.IsChecked;
			Document.Editor.My.Settings.Options_Theme = ThemeComboBox.SelectedIndex;
			Document.Editor.My.Settings.Options_EnableGlass = EnableGlassCheckBox.IsChecked;
			Document.Editor.My.Settings.Options_DefaultFont = FontFaceComboBox.SelectedItem as FontFamily;
			Document.Editor.My.Settings.Options_DefaultFontSize = Convert.ToInt16(FontSizeTextBox.Value);
			if (SpellCheckBox.IsChecked) {
				Document.Editor.My.Settings.Options_SpellCheck = true;
			} else {
				Document.Editor.My.Settings.Options_SpellCheck = false;
			}
			Document.Editor.My.Settings.Options_TabPlacement = TabPlacementComboBox.SelectedIndex;
			Document.Editor.My.Settings.Options_TTSV = TTSComboBox.SelectedIndex;
			Document.Editor.My.Settings.Options_TTSS = Convert.ToInt16(TTSSlider.Value);
			Document.Editor.My.Settings.Options_Tabs_CloseButtonMode = CloseButtonComboBox.SelectedIndex;
			if (RecentDocumentsCheckBox.IsChecked) {
				Document.Editor.My.Settings.Options_ShowRecentDocuments = true;
			} else {
				Document.Editor.My.Settings.Options_ShowRecentDocuments = false;
			}
			Document.Editor.My.Settings.Options_RulerMeasurement = RulerMeasurementComboBox.SelectedIndex;
			if (PluginsCheckBox.IsChecked) {
				Document.Editor.My.Settings.Options_EnablePlugins = true;
			} else {
				Document.Editor.My.Settings.Options_EnablePlugins = false;
			}
			Close();
		}

		private void CancelButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Close();
		}

		private void ResetButton_Click(System.Object sender, System.Windows.RoutedEventArgs e)
		{
			MessageBoxDialog m = new MessageBoxDialog(Document.Editor.My.Application.Info.ProductName + " needs to restart, restart now?", "Are You Sure?", "YesNo", null);
			m.MessageImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Common/info32.png"));
			m.Owner = this;
			m.ShowDialog();
			if (m.Result == "Yes") {
				Document.Editor.My.Settings.Reset();
				Document.Editor.My.Application.Shutdown();
			}
		}

		#endregion

		#region "Plugins"

		private void PluginAddButton_Click(System.Object sender, System.Windows.RoutedEventArgs e)
		{
			Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
			dialog.Title = "Add Plugin";
			dialog.Filter = "Plugins(*.vb)|*.vb";
			this.Hide();
			if (dialog.ShowDialog()) {
				System.IO.FileInfo fileinfo = new System.IO.FileInfo(dialog.FileName);
				Document.Editor.My.Computer.FileSystem.CopyFile(dialog.FileName, Document.Editor.My.Application.Info.DirectoryPath + "\\Plugins\\" + fileinfo.Name);
				PluginsListBox.Items.Add(fileinfo.Name.Remove(fileinfo.Name.Length - 3));
			}
			this.ShowDialog();
		}

		private void PluginRemoveButton_Click(System.Object sender, System.Windows.RoutedEventArgs e)
		{
			Document.Editor.My.Computer.FileSystem.DeleteFile(Document.Editor.My.Application.Info.DirectoryPath + "\\Plugins\\" + PluginsListBox.SelectedItem.ToString() + ".vb");
			PluginsListBox.Items.Remove(PluginsListBox.SelectedItem);
		}

		private void PluginsFolderButton_Click(System.Object sender, System.Windows.RoutedEventArgs e)
		{
			try {
				Process.Start(Document.Editor.My.Application.Info.DirectoryPath + "\\Plugins");
			} catch (Exception ex) {
				MessageBoxDialog m = new MessageBoxDialog(ex.Message, ex.ToString(), null, null);
				m.MessageImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Common/error32.png"));
				m.Owner = this;
				m.ShowDialog();
			}
		}

		private void PluginsListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			if (PluginsListBox.SelectedItem != null) {
				PluginRemoveButton.IsEnabled = true;
			} else {
				PluginRemoveButton.IsEnabled = false;
			}
		}

		#endregion

		#region "Templates"

		private void AddTemplateButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
			dialog.Title = "Add Template";
			dialog.Filter = "Templates(*.xaml)|*.xaml";
			this.Hide();
			if (dialog.ShowDialog()) {
				System.IO.FileInfo fileinfo = new System.IO.FileInfo(dialog.FileName);
				Document.Editor.My.Computer.FileSystem.CopyFile(dialog.FileName, Document.Editor.My.Application.Info.DirectoryPath + "\\Templates\\" + fileinfo.Name);
				TemplatesListBox.Items.Add(fileinfo.Name.Remove(fileinfo.Name.Length - 5));
			}
			this.ShowDialog();
		}

		private void RemoveTemplateButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Document.Editor.My.Computer.FileSystem.DeleteFile(Document.Editor.My.Application.Info.DirectoryPath + "\\Templates\\" + TemplatesListBox.SelectedItem.ToString() + ".xaml");
			TemplatesListBox.Items.Remove(TemplatesListBox.SelectedItem);
		}

		private void TemplatesFolderButton_Click(System.Object sender, System.Windows.RoutedEventArgs e)
		{
			try {
				Process.Start(Document.Editor.My.Application.Info.DirectoryPath + "\\Templates");
			} catch (Exception ex) {
				MessageBoxDialog m = new MessageBoxDialog(ex.Message, ex.ToString(), null, null);
				m.MessageImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Common/error32.png"));
				m.Owner = this;
				m.ShowDialog();
			}
		}

		private void TemplatesListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			if (TemplatesListBox.SelectedItem != null) {
				RemoveTemplateButton.IsEnabled = true;
			} else {
				RemoveTemplateButton.IsEnabled = false;
			}
		}

		#endregion

		private void ClearRecentButton_Click(System.Object sender, System.Windows.RoutedEventArgs e)
		{
			Document.Editor.My.Settings.Options_RecentFiles.Clear();
			MessageBoxDialog m = new MessageBoxDialog("Documents Cleared", "Cleared", null, null);
			m.MessageImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Common/info32.png"));
			m.Owner = this;
			m.ShowDialog();
		}
	}
}

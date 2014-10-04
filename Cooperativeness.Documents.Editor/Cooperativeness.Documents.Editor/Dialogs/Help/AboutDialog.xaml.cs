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
	public partial class AboutDialog
	{
		private BitmapImage licenseicon = new BitmapImage(new Uri("pack://application:,,,/Images/Help/license16.png"));

		private BitmapImage backicon = new BitmapImage(new Uri("pack://application:,,,/Images/Help/back16.png"));
		#region "Reuseable Code"

		public bool PathExists(string path, int timeout)
		{
			bool exists = true;
			System.Threading.Thread t = new System.Threading.Thread((System.Threading.ThreadStart)() => CheckPathFunction(path));
			t.Start();
			bool completed = t.Join(timeout);
			if (!completed) {
				exists = false;
				t.Abort();
			}
			return exists;
		}

		public bool CheckPathFunction(string path)
		{
			return System.IO.File.Exists(path);
		}

		#endregion

		#region "Loaded"

		private void AboutDialog_Loaded(System.Object sender, System.Windows.RoutedEventArgs e)
		{
			TextBox1.Text = Document.Editor.My.Resources.License;
			AppNameLabel.Content = Document.Editor.My.Application.Info.ProductName + " " + Document.Editor.My.Application.Info.Version.Major.ToString;
			VersionLabel.Content = "Version: " + Document.Editor.My.Application.Info.Version.Major.ToString + "." + Document.Editor.My.Application.Info.Version.Minor.ToString;
			CopyLabel.Content = Document.Editor.My.Application.Info.Copyright.ToString + " By Semagsoft";
			if (Document.Editor.My.Computer.Info.OSVersion >= "6.0") {
				if (Document.Editor.My.Settings.Options_EnableGlass) {
					AppHelper.ExtendGlassFrame(this, new Thickness(-1, -1, -1, -1));
				}
			}
		}

		#endregion

		#region "License"

		private void LicenseButton_Click(System.Object sender, System.Windows.RoutedEventArgs e)
		{
			if (TextBox1.IsVisible) {
				TextBox1.Visibility = System.Windows.Visibility.Hidden;
				UpdateButton.Visibility = System.Windows.Visibility.Visible;
				LicenseButton.Icon = licenseicon;
				LicenseButton.Header = "License";
			} else {
				TextBox1.Visibility = System.Windows.Visibility.Visible;
				UpdateButton.Visibility = System.Windows.Visibility.Collapsed;
				LicenseButton.Icon = backicon;
				LicenseButton.Header = "Back";
			}
		}

		#endregion

		#region "Check For Update"

		private System.ComponentModel.BackgroundWorker withEventsField_CheckForUpdateWorker = new System.ComponentModel.BackgroundWorker();
		private System.ComponentModel.BackgroundWorker CheckForUpdateWorker {
			get { return withEventsField_CheckForUpdateWorker; }
			set {
				if (withEventsField_CheckForUpdateWorker != null) {
					withEventsField_CheckForUpdateWorker.DoWork -= CheckForUpdateWorker_DoWork;
					withEventsField_CheckForUpdateWorker.RunWorkerCompleted -= CheckForUpdateWorker_RunWorkerCompleted;
				}
				withEventsField_CheckForUpdateWorker = value;
				if (withEventsField_CheckForUpdateWorker != null) {
					withEventsField_CheckForUpdateWorker.DoWork += CheckForUpdateWorker_DoWork;
					withEventsField_CheckForUpdateWorker.RunWorkerCompleted += CheckForUpdateWorker_RunWorkerCompleted;
				}
			}

		}
		private void UpdateButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			if (Document.Editor.My.Computer.Network.IsAvailable) {
				AppLogo.Visibility = System.Windows.Visibility.Hidden;
				LicenseButton.Visibility = System.Windows.Visibility.Hidden;
				OKButton.Visibility = System.Windows.Visibility.Hidden;
				UpdateButton.Visibility = System.Windows.Visibility.Hidden;
				UpdateBox.Visibility = System.Windows.Visibility.Visible;
				CheckForUpdateWorker.RunWorkerAsync();
			} else {
				MessageBoxDialog m = new MessageBoxDialog("Internet not found.", "Error", null, null);
				m.MessageImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Common/error32.png"));
				m.Owner = this;
				m.ShowDialog();
			}
		}

		private void CheckForUpdateWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
		{
			try {
				if (PathExists("http://semagsoft.com/software/updates/document.editor.update", 5000)) {
					System.Net.WebClient fileReader = new System.Net.WebClient();
					Document.Editor.My.Computer.FileSystem.CreateDirectory(Document.Editor.My.Computer.FileSystem.SpecialDirectories.Temp + "\\Semagsoft\\DocumentEditor\\");
					string filename = Document.Editor.My.Computer.FileSystem.SpecialDirectories.Temp + "\\Semagsoft\\DocumentEditor\\updatechecker.ini";
					if (System.IO.File.Exists(filename)) {
						System.IO.File.Delete(filename);
					}
					fileReader.DownloadFile(new Uri("http://semagsoft.com/software/updates/document.editor.update"), filename);
					e.Result = true;
				} else {
					e.Result = false;
				}
			} catch (Exception ex) {
				MessageBox.Show(ex.Message, ex.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
				e.Result = false;
			}
		}

		private void CheckForUpdateWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
		{
			if (e.Result == true) {
				System.IO.TextReader textreader = System.IO.File.OpenText(Document.Editor.My.Computer.FileSystem.SpecialDirectories.Temp + "\\Semagsoft\\DocumentEditor\\updatechecker.ini");
				string version = textreader.ReadLine();
				int versionyear = Convert.ToInt16(version.Substring(0, 4));
				int versionnumber = Convert.ToInt16(version.Substring(5));
				if (versionyear >= Document.Editor.My.Application.Info.Version.Major && versionnumber > Document.Editor.My.Application.Info.Version.Minor) {
					Collection whatsnew = new Collection();
					string line = null;
					do {
						line = textreader.ReadLine();
						if (line != null) {
							whatsnew.Add(line.ToString());
						}
					} while (!(line == null));
					textreader.Close();
					UpdateText.Text = "An update(" + version + ") was found, do you want to apply it?";
					ProgressBox.Visibility = System.Windows.Visibility.Collapsed;
					ApplyUpdateButton.Visibility = System.Windows.Visibility.Visible;
					CancelUpdateButton.Visibility = System.Windows.Visibility.Visible;
					WhatsNewTextBox.Clear();
					foreach (string s in whatsnew) {
						WhatsNewTextBox.AppendText(s + Constants.vbNewLine);
					}
					WhatsNewTextBlock.Visibility = System.Windows.Visibility.Visible;
					WhatsNewTextBox.Visibility = System.Windows.Visibility.Visible;
				} else {
					MessageBoxDialog m = new MessageBoxDialog("Document.Editor is already up to date", "Up To Date", null, null);
					m.MessageImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Common/info32.png"));
					m.Owner = this;
					m.ShowDialog();
					UpdateBox.Visibility = System.Windows.Visibility.Hidden;
					AppLogo.Visibility = System.Windows.Visibility.Visible;
					LicenseButton.Visibility = System.Windows.Visibility.Visible;
					OKButton.Visibility = System.Windows.Visibility.Visible;
					UpdateButton.Visibility = System.Windows.Visibility.Visible;
				}
			}
		}

		#endregion

		#region "Update"

		private System.Net.WebClient withEventsField_webmanager = new System.Net.WebClient();
		public System.Net.WebClient webmanager {
			get { return withEventsField_webmanager; }
			set {
				if (withEventsField_webmanager != null) {
					withEventsField_webmanager.DownloadProgressChanged -= webmanager_DownloadProgressChanged;
					withEventsField_webmanager.DownloadFileCompleted -= webmanager_DownloadFileCompleted;
				}
				withEventsField_webmanager = value;
				if (withEventsField_webmanager != null) {
					withEventsField_webmanager.DownloadProgressChanged += webmanager_DownloadProgressChanged;
					withEventsField_webmanager.DownloadFileCompleted += webmanager_DownloadFileCompleted;
				}
			}

		}
		private void ApplyUpdateButton_Click(object sender, RoutedEventArgs e)
		{
			UpdateProgressbar.IsIndeterminate = false;
			UpdateProgressbar.Minimum = 0;
			UpdateProgressbar.Maximum = 100;
			UpdateProgressbar.Value = 0;
			ProgressBox.Visibility = System.Windows.Visibility.Visible;
			if (System.IO.File.Exists(Document.Editor.My.Computer.FileSystem.SpecialDirectories.Temp + "\\Semagsoft\\DocumentEditor\\setup.exe")) {
				System.IO.File.Delete(Document.Editor.My.Computer.FileSystem.SpecialDirectories.Temp + "\\Semagsoft\\DocumentEditor\\setup.exe");
			}
			webmanager.DownloadFileAsync(new Uri("http://semagsoft.com/software/downloads/Document.Editor_Setup.exe"), Document.Editor.My.Computer.FileSystem.SpecialDirectories.Temp + "\\Semagsoft\\DocumentEditor\\setup.exe");
			UpdateText.Text = "Downloading Update, Please Wait...";
			ApplyUpdateButton.Visibility = System.Windows.Visibility.Collapsed;
		}

		private void CancelUpdateButton_Click(object sender, RoutedEventArgs e)
		{
			while (webmanager.IsBusy) {
				webmanager.CancelAsync();
			}

			if (System.IO.File.Exists(Document.Editor.My.Computer.FileSystem.SpecialDirectories.Temp + "\\Semagsoft\\DocumentEditor\\setup.exe")) {
				System.IO.File.Delete(Document.Editor.My.Computer.FileSystem.SpecialDirectories.Temp + "\\Semagsoft\\DocumentEditor\\setup.exe");
			}
			AppLogo.Visibility = System.Windows.Visibility.Visible;
			LicenseButton.Visibility = System.Windows.Visibility.Visible;
			OKButton.Visibility = System.Windows.Visibility.Visible;
			UpdateButton.Visibility = System.Windows.Visibility.Visible;
			UpdateBox.Visibility = System.Windows.Visibility.Collapsed;
			UpdateText.Text = "Checking for updates...";
			UpdateProgressbar.IsIndeterminate = true;
			FilesizeTextBlock.Visibility = System.Windows.Visibility.Collapsed;
			ApplyUpdateButton.Visibility = System.Windows.Visibility.Collapsed;
			CancelUpdateButton.Visibility = System.Windows.Visibility.Collapsed;
			WhatsNewTextBlock.Visibility = System.Windows.Visibility.Collapsed;
			WhatsNewTextBox.Visibility = System.Windows.Visibility.Collapsed;
		}

		public void webmanager_DownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
		{
			UpdateProgressbar.Value = e.ProgressPercentage;
			FilesizeTextBlock.Visibility = System.Windows.Visibility.Visible;
			int downloadedfilesize = e.BytesReceived / 1024;
			int totalfilesize = e.TotalBytesToReceive / 1024;
			FilesizeTextBlock.Text = downloadedfilesize.ToString() + " KB" + "/" + totalfilesize.ToString() + " KB";
		}

		public void webmanager_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
		{
			if (!e.Cancelled) {
				try {
					Process.Start(Document.Editor.My.Computer.FileSystem.SpecialDirectories.Temp + "\\Semagsoft\\DocumentEditor\\setup.exe", "/D=" + Document.Editor.My.Application.Info.DirectoryPath);
					Document.Editor.My.Application.Shutdown();
				} catch (Exception ex) {
					if (ex.Message.EndsWith("canceled by the user")) {
						MessageBoxDialog m = new MessageBoxDialog("The update has been canceled!", "Update Canceled", null, null);
						m.MessageImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Common/info32.png"));
						m.Owner = this;
						m.ShowDialog();
						CancelUpdateButton_Click(null, null);
					} else {
						MessageBoxDialog m = new MessageBoxDialog("Error running update installer", "Error", null, null);
						m.MessageImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Common/error32.png"));
						m.Owner = this;
						m.ShowDialog();
					}
				}
			}
		}
		public AboutDialog()
		{
			Loaded += AboutDialog_Loaded;
		}

		#endregion

	}
}

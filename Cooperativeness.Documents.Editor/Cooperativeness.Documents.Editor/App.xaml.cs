using System;
using System.Collections;

namespace Cooperativeness.Documents.Editor
{
	partial class Application
	{
		public ArrayList StartUpFileNames = new ArrayList();
		private void Application_Startup(object sender, System.Windows.StartupEventArgs e)
		{
			ResourceDictionary res = new ResourceDictionary();
			if (Document.Editor.My.Settings.Options_Theme == 0) {
				res.Source = new Uri("pack://application:,,,/Fluent;Component/Themes/Office2010/Blue.xaml");
				Document.Editor.My.Application.Resources = res;
			} else if (Document.Editor.My.Settings.Options_Theme == 1) {
				res.Source = new Uri("pack://application:,,,/Fluent;Component/Themes/Office2010/Silver.xaml");
				Document.Editor.My.Application.Resources = res;
			} else if (Document.Editor.My.Settings.Options_Theme == 2) {
				res.Source = new Uri("pack://application:,,,/Fluent;Component/Themes/Office2010/Black.xaml");
				Document.Editor.My.Application.Resources = res;
			}
			if (e.Args.Length > 0) {
				foreach (string s in e.Args) {
					StartUpFileNames.Add(s);
				}
			}
		}
	}
}

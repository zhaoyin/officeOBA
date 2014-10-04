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

namespace Cooperativeness.Documents.Editor
{
	public partial class SymbolPanel
	{
		public event ClickEventHandler Click;
		public delegate void ClickEventHandler(string symbol);

		#region "Currency"

		private void DollarButton_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (Click != null) {
				Click("$");
			}
		}

		private void CentButton_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (Click != null) {
				Click("¢");
			}
		}

		private void PoundButton_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (Click != null) {
				Click("£");
			}
		}

		private void YenButton_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (Click != null) {
				Click("¥");
			}
		}

		private void EuroButton_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (Click != null) {
				Click("€");
			}
		}

		#endregion

		#region "Misc"

		private void CopyrightButton_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (Click != null) {
				Click("©");
			}
		}

		private void RegisteredTrademarkButton_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (Click != null) {
				Click("®");
			}
		}

		private void TrademarkButton_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (Click != null) {
				Click("™");
			}
		}

		#endregion

		private void SuperscriptOneButton_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (Click != null) {
				Click("¹");
			}
		}

		private void SuperscriptTwoButton_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (Click != null) {
				Click("²");
			}
		}

		private void SuperscriptThreeButton_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (Click != null) {
				Click("³");
			}
		}

	}
}

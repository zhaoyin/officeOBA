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
using System.Windows.Controls.DataVisualization;
namespace Document.Editor
{
	public partial class ChartDialog
	{
		public string Res = "Cancel";
		private Charting.ColumnSeries Series;

		private Collection Items = new Collection();
		#region "Loaded"

		private void ChartDialog_Loaded(object sender, RoutedEventArgs e)
		{
			if (Document.Editor.My.Computer.Info.OSVersion >= "6.0") {
				if (Document.Editor.My.Settings.Options_EnableGlass) {
					AppHelper.ExtendGlassFrame(this, new Thickness(-1, -1, -1, -1));
				}
			}
			Items.Add(new KeyValuePair<string, int>("Item1", 1));
			Items.Add(new KeyValuePair<string, int>("Item2", 2));
			Items.Add(new KeyValuePair<string, int>("Item3", 3));
			LoadColumnData(PreviewChart.Series(0));
			LoadPieData(PreviewChart.Series(1));
			PreviewChart.Series.Remove(PieSeries);
			ItemsListBox.ItemsSource = Items;
		}

		private void ColumnSeries_Loaded(object sender, RoutedEventArgs e)
		{
			LoadColumnData(ColumnSeries);
		}

		private void PieSeries_Loaded(object sender, RoutedEventArgs e)
		{
			LoadPieData(PieSeries);
		}

		#endregion

		#region "Chart Editor"

		private void LoadColumnData(Charting.ISeries series)
		{
			((Charting.ColumnSeries)series).ItemsSource = Items;
		}

		private void LoadPieData(Charting.ISeries series)
		{
			((Charting.PieSeries)series).ItemsSource = Items;
		}

		private void UpdatePreview()
		{
			if (ChartTypeComboBox.SelectedIndex == 0) {
				PreviewChart.Series.Remove(PieSeries);
				PreviewChart.Series.Add(ColumnSeries);
				LoadColumnData(ColumnSeries);
			} else if (ChartTypeComboBox.SelectedIndex == 1) {
				PreviewChart.Series.Remove(ColumnSeries);
				PreviewChart.Series.Add(PieSeries);
				LoadPieData(PieSeries);
			}
		}

		#region "Chart"

		private void ChartTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (IsLoaded) {
				UpdatePreview();
			}
		}

		private void ChartTitleTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (IsLoaded) {
				PreviewChart.Title = ChartTitleTextBox.Text;
			}
		}

		private void ForegroundColorGallery_SelectedColorChanged(object sender, RoutedEventArgs e)
		{
			if (IsLoaded) {
				SolidColorBrush co = new SolidColorBrush();
				co.Color = ForegroundColorGallery.SelectedColor;
				PreviewChart.Foreground = co;
			}
		}

		private void BackgroundColorGallery_SelectedColorChanged(object sender, RoutedEventArgs e)
		{
			if (IsLoaded) {
				SolidColorBrush co = new SolidColorBrush();
				co.Color = BackgroundColorGallery.SelectedColor;
				PreviewChart.Background = co;
			}
		}

		private void ChartHight_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (IsLoaded) {
				PreviewChart.Height = ChartHight.Value;
			}
		}

		private void ChartWidth_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (IsLoaded) {
				PreviewChart.Width = ChartWidth.Value;
			}
		}

		#endregion

		#region "Series"

		private void SeriesTitleTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (IsLoaded) {
				Charting.ISeries s = PreviewChart.Series.Item(0);
				if (object.ReferenceEquals(s.GetType, typeof(Charting.ColumnSeries))) {
					Charting.ColumnSeries series = s;
					series.Title = SeriesTitleTextBox.Text;
				} else if (object.ReferenceEquals(s.GetType, typeof(Charting.PieSeries))) {
					Charting.PieSeries series = s;
					series.Title = SeriesTitleTextBox.Text;
				}
			}
		}

		#endregion

		#region "Items"

		private void ItemsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (ItemsListBox.SelectedItem != null) {
				RemoveItemButton.IsEnabled = true;
				ItemTitleTextBox.Visibility = System.Windows.Visibility.Visible;
				ItemValueBox.Visibility = System.Windows.Visibility.Visible;
				KeyValuePair<string, int> i = null;
				i = Items[ItemsListBox.SelectedIndex + 1];
				ItemTitleTextBox.Text = i.Key;
				IsEditing = false;
				ItemValueBox.Value = i.Value;
				IsEditing = true;
			} else {
				RemoveItemButton.IsEnabled = false;
				ItemTitleTextBox.Visibility = System.Windows.Visibility.Collapsed;
				ItemValueBox.Visibility = System.Windows.Visibility.Collapsed;
			}
		}

		private void AddItemButton_Click(object sender, RoutedEventArgs e)
		{
			Items.Add(new KeyValuePair<string, int>("Item" + Convert.ToString(Items.Count + 1), 1));
			ItemsListBox.ItemsSource = null;
			ItemsListBox.ItemsSource = Items;
			ColumnSeries.ItemsSource = null;
			LoadColumnData(ColumnSeries);
			PieSeries.ItemsSource = null;
			LoadPieData(PieSeries);
		}

		private void RemoveItemButton_Click(object sender, RoutedEventArgs e)
		{
			Items.Remove(ItemsListBox.SelectedIndex + 1);
			ItemsListBox.ItemsSource = null;
			ItemsListBox.ItemsSource = Items;
			ColumnSeries.ItemsSource = null;
			LoadColumnData(ColumnSeries);
			PieSeries.ItemsSource = null;
			LoadPieData(PieSeries);
		}

		private void ItemTitleTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter) {
				KeyValuePair<string, int> i = Items[ItemsListBox.SelectedIndex + 1];
				var newi = new KeyValuePair<string, int>(ItemTitleTextBox.Text, i.Value);
				Collection c = new Collection();
				foreach (KeyValuePair<string, int> it in Items) {
					if (object.ReferenceEquals(it.Key, i.Key)) {
						c.Add(new KeyValuePair<string, int>(newi.Key, newi.Value));
					} else {
						c.Add(new KeyValuePair<string, int>(it.Key, it.Value));
					}
				}
				Items = c;
				ItemsListBox.ItemsSource = null;
				ItemsListBox.ItemsSource = Items;
				ColumnSeries.ItemsSource = null;
				LoadColumnData(ColumnSeries);
				PieSeries.ItemsSource = null;
				LoadPieData(PieSeries);
				e.Handled = true;
			}
		}

		private bool IsEditing = true;
		private void ItemValueBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (IsLoaded && IsEditing) {
				KeyValuePair<string, int> i = Items[ItemsListBox.SelectedIndex + 1];
				Collection c = new Collection();
				foreach (KeyValuePair<string, int> it in Items) {
					if (object.ReferenceEquals(it.Key, i.Key) && it.Value == i.Value) {
						int @int = ItemValueBox.Value;
						c.Add(new KeyValuePair<string, int>(i.Key, @int));
					} else {
						c.Add(new KeyValuePair<string, int>(it.Key, it.Value));
					}
				}
				Items = c;
				ItemsListBox.ItemsSource = null;
				ItemsListBox.ItemsSource = Items;
				ColumnSeries.ItemsSource = null;
				LoadColumnData(ColumnSeries);
				PieSeries.ItemsSource = null;
				LoadPieData(PieSeries);
			}
		}

		#endregion

		#endregion

		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
			Res = "OK";
			Close();
		}
	}
}

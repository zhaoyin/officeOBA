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
	public partial class ShapeDialog
	{
		public string Res = "Cancel";

		public System.Windows.Shapes.Shape Shape = null;
		private void OKButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Res = "OK";
			Close();
		}

		private void ShapeDialog_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			if (Document.Editor.My.Computer.Info.OSVersion >= "6.0") {
				if (Document.Editor.My.Settings.Options_EnableGlass) {
					AppHelper.ExtendGlassFrame(this, new Thickness(-1, -1, -1, -1));
				}
			}
			TypeComboBox.Items.Add("Circle");
			TypeComboBox.Items.Add("Square");
			TypeComboBox.SelectedIndex = 0;
			if (TypeComboBox.SelectedIndex == 0) {
				Shape = new System.Windows.Shapes.Ellipse();
				Shape.Height = 32;
				Shape.Width = 32;
				int int2 = Convert.ToInt32(BorderSizeTextBox.Value);
				Shape.StrokeThickness = int2;
				Shape.Stroke = Brushes.Black;
			} else if (TypeComboBox.SelectedIndex == 1) {
				Shape = new System.Windows.Shapes.Rectangle();
				Shape.Height = 32;
				Shape.Width = 32;
				int int2 = Convert.ToInt32(BorderSizeTextBox.Value);
				Shape.StrokeThickness = int2;
				Shape.Stroke = Brushes.Black;
			}
			ScrollViewer1.Content = Shape;
		}

		private void TypeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			if (TypeComboBox.SelectedIndex == 0) {
				Shape = new System.Windows.Shapes.Ellipse();
				int @int = Convert.ToInt32(SizeTextBox.Value);
				Shape.Height = @int;
				Shape.Width = @int;
				int int2 = Convert.ToInt32(BorderSizeTextBox.Value);
				Shape.StrokeThickness = int2;
				Shape.Stroke = Brushes.Black;
			} else if (TypeComboBox.SelectedIndex == 1) {
				Shape = new System.Windows.Shapes.Rectangle();
				int @int = Convert.ToInt32(SizeTextBox.Value);
				Shape.Height = @int;
				Shape.Width = @int;
				int int2 = Convert.ToInt32(BorderSizeTextBox.Value);
				Shape.StrokeThickness = int2;
				Shape.Stroke = Brushes.Black;
			}
			ScrollViewer1.Content = Shape;
		}

		private void SizeTextBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			try {
				int @int = Convert.ToInt32(SizeTextBox.Value);
				Shape.Height = @int;
				Shape.Width = @int;
			} catch (Exception ex) {
				SizeTextBox.Value = 32;
			}
		}

		private void BorderSizeTextBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			try {
				int @int = Convert.ToInt32(BorderSizeTextBox.Value);
				Shape.StrokeThickness = @int;
			} catch (Exception ex) {
				BorderSizeTextBox.Value = 4;
			}
		}
		public ShapeDialog()
		{
			Loaded += ShapeDialog_Loaded;
		}
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Speech.Synthesis;
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
	partial class MainWindow
	{
		private DocumentTab SelectedDocument = null;
		private SpeechSynthesizer Speech = new SpeechSynthesizer();
		private bool IsJumpListAdded = false;
		private System.Windows.Shell.JumpList myJumpList = new System.Windows.Shell.JumpList();

		private DocumentEditor DocumentPreview = new DocumentEditor();
		private ScrollViewer withEventsField_DocPreviewScrollViewer = new ScrollViewer();
		private ScrollViewer DocPreviewScrollViewer {
			get { return withEventsField_DocPreviewScrollViewer; }
			set {
				if (withEventsField_DocPreviewScrollViewer != null) {
					withEventsField_DocPreviewScrollViewer.Loaded -= DocPreviewScrollViewer_Loaded;
				}
				withEventsField_DocPreviewScrollViewer = value;
				if (withEventsField_DocPreviewScrollViewer != null) {
					withEventsField_DocPreviewScrollViewer.Loaded += DocPreviewScrollViewer_Loaded;
				}
			}
		}
		#region "Reuseable Code"

		private void UpdateDocumentPreview()
		{
			TextRange range = new TextRange(SelectedDocument.Editor.Document.ContentStart, SelectedDocument.Editor.Document.ContentEnd);
			System.IO.MemoryStream stream = new System.IO.MemoryStream();
			System.Windows.Markup.XamlWriter.Save(range, stream);
			range.Save(stream, DataFormats.XamlPackage, true);
			FlowDocument previewdoc = new FlowDocument();
			TextRange range2 = new TextRange(previewdoc.ContentEnd, previewdoc.ContentEnd);
			range2.Load(stream, DataFormats.XamlPackage);
			//TODO: (2014.xx) set background color for preview document
			DocumentPreview.Document.PageWidth = SelectedDocument.Editor.Document.PageWidth;
			DocumentPreview.Document.PageHeight = SelectedDocument.Editor.Document.PageHeight;
			DocumentPreview.Width = DocumentPreview.Document.PageWidth;
			DocumentPreview.Height = DocumentPreview.Document.PageHeight;
			DocumentPreview.Document = previewdoc;
			DocumentPreview.Document.PagePadding = SelectedDocument.Editor.docpadding;
			DocumentPreview.InvalidateVisual();
			DocumentPreview.UpdateLayout();
			Canvas c = DocPreviewScrollViewer.Content as Canvas;
			if (c.Children.Count == 0) {
				c.Children.Add(DocumentPreview);
			}
			DocumentPreview.InvalidateVisual();
			DocumentPreview.UpdateLayout();
			DocPreviewScrollViewer.Content = c;
		}

		#region "Document"

		public void NewDocument(string title)
		{
			if (TabCell.Children.Count > 0) {
				SelectedDocument.IsSelected = false;
			}
			DocumentTab tb = new DocumentTab(title, Brushes.Transparent);
			tb.Ruler.Background = Background;
			TabCell.Children.Add(tb);
			UpdateUI();
			tb.IsSelected = true;
			SelectedDocument.Editor.Focus();
			SelectedDocument.Editor.FileChanged = false;
			UpdateButtons();
		}

		private void CloseDocument(DocumentTab file)
		{
			TabCell.Children.Remove(file);
			UpdateUI();
		}

		#endregion

		private void RunPlugin(object sender, System.Windows.RoutedEventArgs e)
		{
			Fluent.Button p = e.Source;
			Plugins plugins = new Plugins();
			object i = plugins.Build(p.Header, Document.Editor.My.Computer.FileSystem.ReadAllText(p.Tag));
			if (i.GetType().Name.ToString() == "String") {
				SelectedDocument.Editor.CaretPosition.InsertTextInRun(i);
			}
		}

		#region "RibbonBar"

		private void UpdateSelected()
		{
			UpdateContextualTabs();
			if (SelectedDocument.Editor.FileChanged) {
				Title = SelectedDocument.DocName + " * - " + Document.Editor.My.Application.Info.ProductName;
			} else {
				Title = SelectedDocument.DocName + " - " + Document.Editor.My.Application.Info.ProductName;
			}
			MainBar.Title = Title;
			if (Document.Editor.My.Computer.FileSystem.FileExists(SelectedDocument.Editor.DocumentName)) {
				double kb = Document.Editor.My.Computer.FileSystem.GetFileInfo(SelectedDocument.Editor.DocumentName).Length / 1024;
				int inter = Convert.ToInt32(kb);
				FileSizeTextBlock.Text = "Size: " + inter.ToString() + " KB";
			} else {
				FileSizeTextBlock.Text = "Size: " + "0 KB";
			}
			LinesTextBlock.Text = "Line: " + SelectedDocument.Editor.SelectedLineNumber.ToString() + " of " + SelectedDocument.Editor.LineCount.ToString();
			ColumnsTextBlock.Text = "Column: " + SelectedDocument.Editor.SelectedColumnNumber.ToString() + " of " + SelectedDocument.Editor.ColumnCount.ToString();
			WordCountTextBlock.Text = "Word Count: " + SelectedDocument.Editor.WordCount.ToString();
			if (SelectedDocument.Editor.CanUndo) {
				UndoButton.IsEnabled = true;
			} else {
				UndoButton.IsEnabled = false;
			}
			if (SelectedDocument.Editor.CanRedo) {
				RedoButton.IsEnabled = true;
			} else {
				RedoButton.IsEnabled = false;
			}
			if (Document.Editor.My.Computer.Clipboard.ContainsText || Document.Editor.My.Computer.Clipboard.ContainsImage) {
				PasteButton.IsEnabled = true;
				if (Document.Editor.My.Computer.Clipboard.ContainsText) {
					PasteTextButton.IsEnabled = true;
				} else {
					PasteTextButton.IsEnabled = false;
				}
				if (Document.Editor.My.Computer.Clipboard.ContainsImage) {
					PasteImageMenuItem.IsEnabled = true;
				} else {
					PasteImageMenuItem.IsEnabled = false;
				}
			} else {
				PasteButton.IsEnabled = false;
				PasteTextButton.IsEnabled = false;
				PasteImageMenuItem.IsEnabled = false;
			}
			if (SelectedDocument.Editor.FileChanged) {
				SaveMenuItem.IsEnabled = true;
			} else {
				SaveMenuItem.IsEnabled = false;
			}
			UpdatingFont = true;
			if (FontComboBox.IsLoaded) {
				object value = SelectedDocument.Editor.Selection.GetPropertyValue(TextElement.FontFamilyProperty);
				FontFamily currentfontfamily = value as FontFamily;
				if (currentfontfamily != null) {
					FontComboBox.SelectedItem = currentfontfamily;
				}
			}
			if (FontSizeComboBox.IsLoaded) {
				try {
					double sizevalue = SelectedDocument.Editor.Selection.GetPropertyValue(TextElement.FontSizeProperty);
					FontSizeComboBox.SelectedValue = sizevalue;
				} catch (Exception ex) {
				}
			}
			UpdatingFont = false;
			Run r = SelectedDocument.Editor.CaretPosition.Parent as Run;
			if (r != null) {
				if (r.FontWeight == System.Windows.FontWeights.Bold) {
					BoldButton.IsChecked = true;
				} else {
					BoldButton.IsChecked = false;
				}
				if (r.FontStyle == System.Windows.FontStyles.Italic) {
					ItalicButton.IsChecked = true;
				} else {
					ItalicButton.IsChecked = false;
				}
				TextDecorationCollection td = SelectedDocument.Editor.Selection.GetPropertyValue(Inline.TextDecorationsProperty) as TextDecorationCollection;
				UnderlineButton.IsChecked = false;
				StrikethroughButton.IsChecked = false;
				if (object.ReferenceEquals(td, TextDecorations.Underline)) {
					UnderlineButton.IsChecked = true;
				}
				if (object.ReferenceEquals(td, TextDecorations.Strikethrough)) {
					StrikethroughButton.IsChecked = true;
				}
				if (r.BaselineAlignment == BaselineAlignment.Subscript) {
					SubscriptButton.IsChecked = true;
					SuperscriptButton.IsChecked = false;
				} else if (r.BaselineAlignment == BaselineAlignment.Superscript) {
					SubscriptButton.IsChecked = false;
					SuperscriptButton.IsChecked = true;
				} else {
					SubscriptButton.IsChecked = false;
					SuperscriptButton.IsChecked = false;
				}
				Paragraph runparent = r.Parent as Paragraph;
				if (runparent != null) {
					if (runparent.TextAlignment == TextAlignment.Left) {
						AlignLeftButton.IsChecked = true;
						AlignCenterButton.IsChecked = false;
						AlignRightButton.IsChecked = false;
						AlignJustifyButton.IsChecked = false;
					} else if (runparent.TextAlignment == TextAlignment.Center) {
						AlignLeftButton.IsChecked = false;
						AlignCenterButton.IsChecked = true;
						AlignRightButton.IsChecked = false;
						AlignJustifyButton.IsChecked = false;
					} else if (runparent.TextAlignment == TextAlignment.Right) {
						AlignLeftButton.IsChecked = false;
						AlignCenterButton.IsChecked = false;
						AlignRightButton.IsChecked = true;
						AlignJustifyButton.IsChecked = false;
					} else if (runparent.TextAlignment == TextAlignment.Justify) {
						AlignLeftButton.IsChecked = false;
						AlignCenterButton.IsChecked = false;
						AlignRightButton.IsChecked = false;
						AlignJustifyButton.IsChecked = true;
					}
					ListItem listitem = runparent.Parent as ListItem;
					if (listitem != null) {
						List list = listitem.Parent as List;
						if (list != null) {
							if (list.MarkerStyle == TextMarkerStyle.Disc || list.MarkerStyle == TextMarkerStyle.Circle || list.MarkerStyle == TextMarkerStyle.Box || list.MarkerStyle == TextMarkerStyle.Square) {
								BulletListButton.IsChecked = true;
							} else {
								BulletListButton.IsChecked = false;
							}
							if (list.MarkerStyle == TextMarkerStyle.Decimal || list.MarkerStyle == TextMarkerStyle.UpperLatin || list.MarkerStyle == TextMarkerStyle.LowerLatin || list.MarkerStyle == TextMarkerStyle.UpperRoman || list.MarkerStyle == TextMarkerStyle.LowerRoman) {
								NumberListButton.IsChecked = true;
							} else {
								NumberListButton.IsChecked = false;
							}
						} else {
							BulletListButton.IsChecked = false;
							NumberListButton.IsChecked = false;
						}
					} else {
						BulletListButton.IsChecked = false;
						NumberListButton.IsChecked = false;
					}
				}
			} else {
				Paragraph p = SelectedDocument.Editor.CaretPosition.Parent as Paragraph;
				if (p != null) {
					if (p.FontWeight == System.Windows.FontWeights.Bold) {
						BoldButton.IsChecked = true;
					} else {
						BoldButton.IsChecked = false;
					}
					if (p.FontStyle == System.Windows.FontStyles.Italic) {
						ItalicButton.IsChecked = true;
					} else {
						ItalicButton.IsChecked = false;
					}
					TextDecorationCollection td = SelectedDocument.Editor.Selection.GetPropertyValue(Inline.TextDecorationsProperty) as TextDecorationCollection;
					UnderlineButton.IsChecked = false;
					StrikethroughButton.IsChecked = false;
					if (object.ReferenceEquals(td, TextDecorations.Underline)) {
						UnderlineButton.IsChecked = true;
					}
					if (object.ReferenceEquals(td, TextDecorations.Strikethrough)) {
						StrikethroughButton.IsChecked = true;
					}
					Paragraph runparent = p as Paragraph;
					if (runparent != null) {
						if (runparent.TextAlignment == TextAlignment.Left) {
							AlignLeftButton.IsChecked = true;
							AlignCenterButton.IsChecked = false;
							AlignRightButton.IsChecked = false;
							AlignJustifyButton.IsChecked = false;
						} else if (runparent.TextAlignment == TextAlignment.Center) {
							AlignLeftButton.IsChecked = false;
							AlignCenterButton.IsChecked = true;
							AlignRightButton.IsChecked = false;
							AlignJustifyButton.IsChecked = false;
						} else if (runparent.TextAlignment == TextAlignment.Right) {
							AlignLeftButton.IsChecked = false;
							AlignCenterButton.IsChecked = false;
							AlignRightButton.IsChecked = true;
							AlignJustifyButton.IsChecked = false;
						} else if (runparent.TextAlignment == TextAlignment.Justify) {
							AlignLeftButton.IsChecked = false;
							AlignCenterButton.IsChecked = false;
							AlignRightButton.IsChecked = false;
							AlignJustifyButton.IsChecked = true;
						}
					}
					ListItem listitem = p.Parent as ListItem;
					if (listitem != null) {
						List list = listitem.Parent as List;
						if (list != null) {
							if (list.MarkerStyle == TextMarkerStyle.Disc || list.MarkerStyle == TextMarkerStyle.Circle || list.MarkerStyle == TextMarkerStyle.Box || list.MarkerStyle == TextMarkerStyle.Square) {
								BulletListButton.IsChecked = true;
							} else {
								BulletListButton.IsChecked = false;
							}
							if (list.MarkerStyle == TextMarkerStyle.Decimal || list.MarkerStyle == TextMarkerStyle.UpperLatin || list.MarkerStyle == TextMarkerStyle.LowerLatin || list.MarkerStyle == TextMarkerStyle.UpperRoman || list.MarkerStyle == TextMarkerStyle.LowerRoman) {
								NumberListButton.IsChecked = true;
							} else {
								NumberListButton.IsChecked = false;
							}
						} else {
							BulletListButton.IsChecked = false;
							NumberListButton.IsChecked = false;
						}
					} else {
						BulletListButton.IsChecked = false;
						NumberListButton.IsChecked = false;
					}
				} else {
					BulletListButton.IsChecked = false;
					NumberListButton.IsChecked = false;
				}
			}
			if (SelectedDocument.Editor.CaretPosition.Paragraph != null) {
				LineSpacing1Point0.IsChecked = false;
				LineSpacing1Point15.IsChecked = false;
				LineSpacing1Point5.IsChecked = false;
				LineSpacing2Point0.IsChecked = false;
				LineSpacing2Point5.IsChecked = false;
				LineSpacing3Point0.IsChecked = false;
				if (SelectedDocument.Editor.CaretPosition.Paragraph.LineHeight == 1.0) {
					LineSpacing1Point0.IsChecked = true;
				} else if (SelectedDocument.Editor.CaretPosition.Paragraph.LineHeight == 1.15) {
					LineSpacing1Point15.IsChecked = true;
				} else if (SelectedDocument.Editor.CaretPosition.Paragraph.LineHeight == 1.5) {
					LineSpacing1Point5.IsChecked = true;
				} else if (SelectedDocument.Editor.CaretPosition.Paragraph.LineHeight == 2.0) {
					LineSpacing2Point0.IsChecked = true;
				} else if (SelectedDocument.Editor.CaretPosition.Paragraph.LineHeight == 2.5) {
					LineSpacing2Point5.IsChecked = true;
				} else if (SelectedDocument.Editor.CaretPosition.Paragraph.LineHeight == 3.0) {
					LineSpacing3Point0.IsChecked = true;
				}
			}
		}

		private void UpdateButtons()
		{
			if (dockingManager.Layout.ActiveContent != null) {
				if (SelectedDocument.Editor.Document.Blocks.Count == 0) {
					SelectedDocument.Editor.Document.Blocks.Add(new Paragraph());
				}
				UpdateSelected();
				if (SelectedDocument.Editor.DocumentName != null && SelectedDocument.Editor.FileChanged) {
					RevertMenuItem.IsEnabled = true;
				} else {
					RevertMenuItem.IsEnabled = false;
				}
				CloseMenuItem.IsEnabled = true;
				CloseAllButThisMenuItem.IsEnabled = true;
				CloseAllMenuItem.IsEnabled = true;
				SaveAsMenuItem.IsEnabled = true;
				SaveCopyMenuItem.IsEnabled = true;
				SaveAllMenuItem.IsEnabled = true;
				ExportMenuItem.IsEnabled = true;
				ExportXPSMenuItem.IsEnabled = true;
				ExportArchiveMenuItem.IsEnabled = true;
				ExportImageMenuItem.IsEnabled = true;
				ExportSoundMenuItem.IsEnabled = true;
				PropertiesMenuItem.IsEnabled = true;
				BackgroundColorMenuItem.IsEnabled = true;
				PrintTab.IsEnabled = true;
				PrintMenuItem.IsEnabled = true;
				PageSetupMenuItem.IsEnabled = true;
				CommonEditGroup.IsEnabled = true;
				CommonViewGroup.IsEnabled = true;
				zoomSlider.Value = SelectedDocument.Editor.ZoomLevel * 100;
				ZoomGroup.IsEnabled = true;
				ZoomInButton.IsEnabled = true;
				CommonZoomInButton.IsEnabled = true;
				if (zoomSlider.Value > 20) {
					ZoomOutButton.IsEnabled = true;
					CommonZoomOutButton.IsEnabled = true;
				} else {
					ZoomOutButton.IsEnabled = false;
					CommonZoomOutButton.IsEnabled = false;
				}
				if (zoomSlider.Value < 500) {
					ZoomInButton.IsEnabled = true;
					CommonZoomInButton.IsEnabled = true;
				} else {
					ZoomInButton.IsEnabled = false;
					CommonZoomInButton.IsEnabled = false;
				}
				ResetZoomButton.IsEnabled = true;
				CommonResetZoomButton.IsEnabled = true;
				zoomSlider.IsEnabled = true;
				InsertTab.Visibility = System.Windows.Visibility.Visible;
				TableButton.IsEnabled = true;
				DateButton.IsEnabled = true;
				TimeButton.IsEnabled = true;
				ImageButton.IsEnabled = true;
				VideoButton.IsEnabled = true;
				ObjectButton.IsEnabled = true;
				ShapeButton.IsEnabled = true;
				LinkButton.IsEnabled = true;
				TextFileButton.IsEnabled = true;
				HorizontalLineButton.IsEnabled = true;
				HeaderButton.IsEnabled = true;
				FooterButton.IsEnabled = true;
				ClearFormattingButton.IsEnabled = true;
				FontColorButton.IsEnabled = true;
				HighlightColorButton.IsEnabled = true;
				BoldButton.IsEnabled = true;
				ItalicButton.IsEnabled = true;
				UnderlineButton.IsEnabled = true;
				StrikethroughButton.IsEnabled = true;
				SubscriptButton.IsEnabled = true;
				SuperscriptButton.IsEnabled = true;
				IndentMoreButton.IsEnabled = true;
				IndentLessButton.IsEnabled = true;
				BulletListButton.IsEnabled = true;
				NumberListButton.IsEnabled = true;
				AlignLeftButton.IsEnabled = true;
				AlignCenterButton.IsEnabled = true;
				AlignRightButton.IsEnabled = true;
				AlignJustifyButton.IsEnabled = true;
				LinespacingButton.IsEnabled = true;
				LeftToRightButton.IsEnabled = true;
				RightToLeftButton.IsEnabled = true;
				PageLayoutTab.Visibility = System.Windows.Visibility.Visible;
				LeftMarginBox.Value = SelectedDocument.Editor.docpadding.Left;
				TopMarginBox.Value = SelectedDocument.Editor.docpadding.Top;
				RightMarginBox.Value = SelectedDocument.Editor.docpadding.Right;
				BottomMarginBox.Value = SelectedDocument.Editor.docpadding.Bottom;
				PageHeightBox.Value = SelectedDocument.Editor.Document.PageHeight;
				PageWidthBox.Value = SelectedDocument.Editor.Document.PageWidth;
				NavigationTab.Visibility = System.Windows.Visibility.Visible;
				LineDownButton.IsEnabled = true;
				LineUpButton.IsEnabled = true;
				LineLeftButton.IsEnabled = true;
				LineRightButton.IsEnabled = true;
				PageDownButton.IsEnabled = true;
				PageUpButton.IsEnabled = true;
				PageLeftButton.IsEnabled = true;
				PageRightButton.IsEnabled = true;
				StartButton.IsEnabled = true;
				EndButton.IsEnabled = true;
				CommonEditGroup.IsEnabled = true;
				CommonInsertGroup.IsEnabled = true;
				CommonFormatGroup.IsEnabled = true;
				CommonToolsGroup.IsEnabled = true;
				SpellCheckButton.IsEnabled = true;
				TextToSpeechButton.IsEnabled = true;
				TranslateButton.IsEnabled = true;
				DefinitionsButton.IsEnabled = true;
				if (StatusbarButton.IsChecked) {
					StatusBar.Visibility = System.Windows.Visibility.Visible;
				}
			} else if (dockingManager.Layout.ActiveContent == null) {
				SelectedDocument = null;
				Title = Document.Editor.My.Application.Info.ProductName;
				MainBar.Title = Document.Editor.My.Application.Info.ProductName;
				BackStage.SelectedIndex = 0;
				MainBar.SelectedTabIndex = 0;
				RevertMenuItem.IsEnabled = false;
				CloseMenuItem.IsEnabled = false;
				CloseAllButThisMenuItem.IsEnabled = false;
				CloseAllMenuItem.IsEnabled = false;
				SaveMenuItem.IsEnabled = false;
				SaveAsMenuItem.IsEnabled = false;
				SaveCopyMenuItem.IsEnabled = false;
				SaveAllMenuItem.IsEnabled = false;
				ExportMenuItem.IsEnabled = false;
				ExportXPSMenuItem.IsEnabled = false;
				ExportArchiveMenuItem.IsEnabled = false;
				ExportImageMenuItem.IsEnabled = false;
				ExportSoundMenuItem.IsEnabled = false;
				PropertiesMenuItem.IsEnabled = false;
				BackgroundColorMenuItem.IsEnabled = false;
				PrintTab.IsEnabled = false;
				PrintMenuItem.IsEnabled = false;
				PageSetupMenuItem.IsEnabled = false;
				CommonEditGroup.IsEnabled = false;
				CommonViewGroup.IsEnabled = false;
				ZoomGroup.IsEnabled = false;
				ZoomInButton.IsEnabled = false;
				CommonZoomInButton.IsEnabled = false;
				ZoomOutButton.IsEnabled = false;
				CommonZoomOutButton.IsEnabled = false;
				ResetZoomButton.IsEnabled = false;
				CommonResetZoomButton.IsEnabled = false;
				zoomSlider.IsEnabled = false;
				InsertTab.Visibility = System.Windows.Visibility.Collapsed;
				TableButton.IsEnabled = false;
				DateButton.IsEnabled = false;
				TimeButton.IsEnabled = false;
				ImageButton.IsEnabled = false;
				VideoButton.IsEnabled = false;
				ObjectButton.IsEnabled = false;
				ShapeButton.IsEnabled = false;
				LinkButton.IsEnabled = false;
				TextFileButton.IsEnabled = false;
				HorizontalLineButton.IsEnabled = false;
				HeaderButton.IsEnabled = false;
				FooterButton.IsEnabled = false;
				ClearFormattingButton.IsEnabled = false;
				FontColorButton.IsEnabled = false;
				HighlightColorButton.IsEnabled = false;
				BoldButton.IsEnabled = false;
				BoldButton.IsChecked = false;
				ItalicButton.IsEnabled = false;
				ItalicButton.IsChecked = false;
				UnderlineButton.IsEnabled = false;
				UnderlineButton.IsChecked = false;
				StrikethroughButton.IsEnabled = false;
				StrikethroughButton.IsChecked = false;
				SubscriptButton.IsEnabled = false;
				SubscriptButton.IsChecked = false;
				SuperscriptButton.IsEnabled = false;
				SuperscriptButton.IsChecked = false;
				IndentMoreButton.IsEnabled = false;
				IndentLessButton.IsEnabled = false;
				BulletListButton.IsEnabled = false;
				BulletListButton.IsChecked = false;
				NumberListButton.IsEnabled = false;
				NumberListButton.IsChecked = false;
				AlignLeftButton.IsEnabled = false;
				AlignLeftButton.IsChecked = false;
				AlignCenterButton.IsEnabled = false;
				AlignCenterButton.IsChecked = false;
				AlignRightButton.IsEnabled = false;
				AlignRightButton.IsChecked = false;
				AlignJustifyButton.IsEnabled = false;
				AlignJustifyButton.IsChecked = false;
				LinespacingButton.IsEnabled = false;
				LeftToRightButton.IsEnabled = false;
				RightToLeftButton.IsEnabled = false;
				PageLayoutTab.Visibility = System.Windows.Visibility.Collapsed;
				NavigationTab.Visibility = System.Windows.Visibility.Collapsed;
				LineDownButton.IsEnabled = false;
				LineUpButton.IsEnabled = false;
				LineLeftButton.IsEnabled = false;
				LineRightButton.IsEnabled = false;
				PageDownButton.IsEnabled = false;
				PageUpButton.IsEnabled = false;
				PageLeftButton.IsEnabled = false;
				PageRightButton.IsEnabled = false;
				StartButton.IsEnabled = false;
				EndButton.IsEnabled = false;
				EditTableTab.Visibility = System.Windows.Visibility.Collapsed;
				EditTableCellTab.Visibility = System.Windows.Visibility.Collapsed;
				EditListTab.Visibility = System.Windows.Visibility.Collapsed;
				EditImageTab.Visibility = System.Windows.Visibility.Collapsed;
				EditVideoTab.Visibility = System.Windows.Visibility.Collapsed;
				EditObjectTab.Visibility = System.Windows.Visibility.Collapsed;
				CommonEditGroup.IsEnabled = false;
				CommonInsertGroup.IsEnabled = false;
				CommonFormatGroup.IsEnabled = false;
				CommonToolsGroup.IsEnabled = false;
				SpellCheckButton.IsEnabled = false;
				TextToSpeechButton.IsEnabled = false;
				TranslateButton.IsEnabled = false;
				DefinitionsButton.IsEnabled = false;
				StatusBar.Visibility = System.Windows.Visibility.Collapsed;
			}
		}

		#endregion

		public void UpdateUI()
		{
			if (SelectedDocument != null) {
				if (Document.Editor.My.Settings.MainWindow_ShowRuler) {
					if (TabCell.Children.Count >= 2) {
						foreach (DocumentTab t in TabCell.Children) {
							t.Ruler.Margin = new Thickness(-23, 2, 0, 0);
							t.VSV.Margin = new Thickness(-6, 26, 0, 0);
						}
					} else {
						foreach (DocumentTab t in TabCell.Children) {
							t.Ruler.Margin = new Thickness(-23, 0, 0, 0);
							t.VSV.Margin = new Thickness(-6, 24, 0, 0);
						}
					}
				} else {
					if (TabCell.Children.Count >= 2) {
						foreach (DocumentTab t in TabCell.Children) {
							t.VSV.Margin = new Thickness(-6, 0, 0, 0);
						}
					} else {
						foreach (DocumentTab t in TabCell.Children) {
							t.VSV.Margin = new Thickness(-6, -1, 0, 0);
						}
					}
				}
			}
		}

		#endregion

		#region "MainWindow"

		#region "Add Handlers"

		private void addhandlers()
		{
			this.AddHandler(TabHeader.CloseTabEvent, new RoutedEventHandler(CloseDoc));
			this.AddHandler(TabHeader.CloseAllTabEvent, new RoutedEventHandler(CloseAllMenuItem_Click));
			this.AddHandler(TabHeader.SaveTabEvent, new RoutedEventHandler(SaveMenuItem_Click));
			this.AddHandler(TabHeader.SaveAsTabEvent, new RoutedEventHandler(SaveAsMenuItem_Click));
			this.AddHandler(TabHeader.SaveAllTabEvent, new RoutedEventHandler(SaveAllMenuItem_Click));
			this.AddHandler(DocumentTab.UpdateSelected, new RoutedEventHandler(UpdateButtons));
			this.AddHandler(DocumentTab.InsertObjectEvent, new RoutedEventHandler(ObjectButton_Click));
			this.AddHandler(DocumentTab.InsertShapeEvent, new RoutedEventHandler(ShapeButton_Click));
			this.AddHandler(DocumentTab.InsertImageEvent, new RoutedEventHandler(ImageButton_Click));
			this.AddHandler(DocumentTab.InsertLinkEvent, new RoutedEventHandler(LinkMenuItem_Click));
			this.AddHandler(DocumentTab.InsertFlowDocumentEvent, new RoutedEventHandler(InsertFlowDocumentButton_Click));
			this.AddHandler(DocumentTab.InsertRichTextFileEvent, new RoutedEventHandler(InsertRichTextDocumentButton_Click));
			this.AddHandler(DocumentTab.InsertTextFileEvent, new RoutedEventHandler(TextFileButton_Click));
			this.AddHandler(DocumentTab.InsertSymbolEvent, new RoutedEventHandler(SymbolContextMenuItem_Click));
			this.AddHandler(DocumentTab.InsertTableEvent, new RoutedEventHandler(TableMenuItem_Click));
			this.AddHandler(DocumentTab.InsertVideoEvent, new RoutedEventHandler(VideoButton_Click));
			this.AddHandler(DocumentTab.InsertHorizontalLineEvent, new RoutedEventHandler(HorizontalLineButton_Click));
			this.AddHandler(DocumentTab.InsertHeaderEvent, new RoutedEventHandler(HeaderButton_Click));
			this.AddHandler(DocumentTab.InsertFooterEvent, new RoutedEventHandler(FooterButton_Click));
			this.AddHandler(DocumentTab.InsertDateEvent, new RoutedEventHandler(DateMenuItem_Click));
			this.AddHandler(DocumentTab.InsertTimeEvent, new RoutedEventHandler(TimeMenuItem_Click));
			this.AddHandler(DocumentTab.ClearFormattingEvent, new RoutedEventHandler(ClearFormattingButton_Click));
			this.AddHandler(DocumentTab.FontEvent, new RoutedEventHandler(FontContextMenuItem_Click));
			this.AddHandler(DocumentTab.FontSizeEvent, new RoutedEventHandler(FontSizeContextMenuItem_Click));
			this.AddHandler(DocumentTab.FontColorEvent, new RoutedEventHandler(FontColorContextMenuItem_Click));
			this.AddHandler(DocumentTab.HighlightColorEvent, new RoutedEventHandler(FontHighlightColorContextMenuItem_Click));
			this.AddHandler(DocumentTab.BoldEvent, new RoutedEventHandler(BoldMenuItem_Click));
			this.AddHandler(DocumentTab.ItalicEvent, new RoutedEventHandler(ItalicMenuItem_Click));
			this.AddHandler(DocumentTab.UnderlineEvent, new RoutedEventHandler(UnderlineMenuItem_Click));
			this.AddHandler(DocumentTab.StrikethroughEvent, new RoutedEventHandler(StrikethroughButton_Click));
			this.AddHandler(DocumentTab.SubscriptEvent, new RoutedEventHandler(SubscriptButton_Click));
			this.AddHandler(DocumentTab.SuperscriptEvent, new RoutedEventHandler(SuperscriptButton_Click));
			this.AddHandler(DocumentTab.IndentMoreEvent, new RoutedEventHandler(IndentMoreButton_Click));
			this.AddHandler(DocumentTab.IndentLessEvent, new RoutedEventHandler(IndentLessButton_Click));
			this.AddHandler(DocumentTab.BulletListEvent, new RoutedEventHandler(BulletListMenuItem_Click));
			this.AddHandler(DocumentTab.NumberListEvent, new RoutedEventHandler(NumberListMenuItem_Click));
			this.AddHandler(DocumentTab.AlignLeftEvent, new RoutedEventHandler(AlignLeftMenuItem_Click));
			this.AddHandler(DocumentTab.AlignCenterEvent, new RoutedEventHandler(AlignCenterMenuItem_Click));
			this.AddHandler(DocumentTab.AlignRightEvent, new RoutedEventHandler(AlignRightMenuItem_Click));
			this.AddHandler(DocumentTab.AlignJustifyEvent, new RoutedEventHandler(AlignJustifyMenuItem_Click));
			this.AddHandler(DocumentTab.LineSpacingEvent, new RoutedEventHandler(CustomLineSpacingMenuItem_Click));
			this.AddHandler(DocumentTab.LeftToRightEvent, new RoutedEventHandler(LefttoRightButton_Click));
			this.AddHandler(DocumentTab.RightToLeftEvent, new RoutedEventHandler(RighttoLeftButton_Click));
			this.AddHandler(DocumentTab.UndoEvent, new RoutedEventHandler(UndoMenuItem_Click));
			this.AddHandler(DocumentTab.RedoEvent, new RoutedEventHandler(RedoMenuItem_Click));
			this.AddHandler(DocumentTab.CutEvent, new RoutedEventHandler(CutMenuItem_Click));
			this.AddHandler(DocumentTab.CopyEvent, new RoutedEventHandler(CopyMenuItem_Click));
			this.AddHandler(DocumentTab.PasteEvent, new RoutedEventHandler(PasteMenuItem_Click));
			this.AddHandler(DocumentTab.DeleteEvent, new RoutedEventHandler(DeleteMenuItem_Click));
			this.AddHandler(DocumentTab.SelectAllEvent, new RoutedEventHandler(SelectAllMenuItem_Click));
			this.AddHandler(DocumentTab.FindEvent, new RoutedEventHandler(FindButton_Click));
			this.AddHandler(DocumentTab.ReplaceEvent, new RoutedEventHandler(ReplaceButton_Click));
			this.AddHandler(DocumentTab.GoToEvent, new RoutedEventHandler(GoToButton_Click));
		}

		private void SymbolContextMenuItem_Click(object sender, EventArgs e)
		{
			MainBar.SelectedTabItem = InsertTab;
			InsertSymbolButton.IsDropDownOpen = true;
		}

		private void FontContextMenuItem_Click(object sender, EventArgs e)
		{
			MainBar.SelectedTabItem = HomeTabItem;
			FontComboBox.IsDropDownOpen = true;
		}

		private void FontSizeContextMenuItem_Click(object sender, EventArgs e)
		{
			MainBar.SelectedTabItem = HomeTabItem;
			FontSizeComboBox.IsDropDownOpen = true;
		}

		private void FontColorContextMenuItem_Click(object sender, EventArgs e)
		{
			MainBar.SelectedTabItem = HomeTabItem;
			FontColorButton.IsDropDownOpen = true;
		}

		private void FontHighlightColorContextMenuItem_Click(object sender, EventArgs e)
		{
			MainBar.SelectedTabItem = HomeTabItem;
			HighlightColorButton.IsDropDownOpen = true;
		}

		#endregion

		#region "Activated"

		private void MainWindow_Activated(object sender, System.EventArgs e)
		{
			if (Document.Editor.My.Computer.Info.OSVersion >= "6.1") {
				if (!IsJumpListAdded) {
					System.Windows.Shell.JumpList.SetJumpList(Application.Current, myJumpList);
					System.Windows.Shell.JumpTask OnlineHelpJumpTask = new System.Windows.Shell.JumpTask();
					var _with1 = OnlineHelpJumpTask;
					_with1.CustomCategory = "Links";
					_with1.IconResourceIndex = 0;
					_with1.Title = "Online Help";
					_with1.Description = "Get Help Online";
					_with1.ApplicationPath = "http://documenteditor.net/documentation/";
					_with1.Arguments = null;
					System.Windows.Shell.JumpTask PluginsJumpTask = new System.Windows.Shell.JumpTask();
					var _with2 = PluginsJumpTask;
					_with2.CustomCategory = "Links";
					_with2.IconResourceIndex = 0;
					_with2.Title = "Get Plugins";
					_with2.Description = "Get Plugins for Document.Editor";
					_with2.ApplicationPath = "http://documenteditor.net/plugins/";
					_with2.Arguments = null;
					System.Windows.Shell.JumpTask WebsiteJumpTask = new System.Windows.Shell.JumpTask();
					var _with3 = WebsiteJumpTask;
					_with3.CustomCategory = "Links";
					_with3.IconResourceIndex = 0;
					_with3.Title = "Website";
					_with3.Description = "Visit Website";
					_with3.ApplicationPath = "http://documenteditor.net/";
					_with3.Arguments = null;
					myJumpList.JumpItems.Add(OnlineHelpJumpTask);
					myJumpList.JumpItems.Add(PluginsJumpTask);
					myJumpList.JumpItems.Add(WebsiteJumpTask);
					myJumpList.Apply();
					IsJumpListAdded = true;
				}
			}
		}

		#endregion

		#region "Key Down"

		private void MainWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (TabCell.SelectedContent != null) {
				if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl) && !Keyboard.IsKeyDown(Key.LeftShift)) {
					if (e.Key == Key.F) {
						e.Handled = true;
						FindButton_Click(null, null);
					} else if (e.Key == Key.G) {
						e.Handled = true;
						GoToButton_Click(null, null);
					} else if (e.Key == Key.H) {
						e.Handled = true;
						ReplaceButton_Click(null, null);
					} else if (e.Key == Key.N) {
						NewMenuItem_Click(null, null);
					} else if (e.Key == Key.O) {
						e.Handled = true;
						OpenMenuItem_Click(null, null);
					} else if (e.Key == Key.P) {
						e.Handled = true;
						PrintMenuItem_Click(null, null);
					} else if (e.Key == Key.S) {
						e.Handled = true;
						if (SaveMenuItem.IsEnabled) {
							SaveMenuItem_Click(null, null);
						}
					} else if (e.Key == Key.W) {
						e.Handled = true;
						CloseMenuItem_Click(null, null);
						//ElseIf Keyboard.IsKeyDown(Key.OemPlus) Then
						//    'TODO: Add zoom in shotcut key
						//    e.Handled = True
						//    ZoomInButton_Click(Nothing, Nothing)
						//ElseIf e.Key = Key.OemMinus Then
						//    e.Handled = True
						//    If ZoomOutButton.IsEnabled Then
						//        ZoomOutButton_Click(Nothing, Nothing)
						//    End If
					}
				}
				if (Keyboard.IsKeyDown(Key.Insert)) {
					if (e.Key == Key.O) {
						e.Handled = true;
						ObjectButton_Click(null, null);
					} else if (e.Key == Key.H) {
						e.Handled = true;
						HorizontalLineButton_Click(null, null);
					} else if (e.Key == Key.I) {
						e.Handled = true;
						ImageButton_Click(null, null);
					} else if (e.Key == Key.L) {
						e.Handled = true;
						LinkMenuItem_Click(null, null);
					} else if (e.Key == Key.S) {
						e.Handled = true;
						ShapeButton_Click(null, null);
					} else if (e.Key == Key.T) {
						e.Handled = true;
						TableMenuItem_Click(null, null);
					} else if (e.Key == Key.V) {
						e.Handled = true;
						VideoButton_Click(null, null);
					} else if (e.Key == Key.X) {
						e.Handled = true;
						TextFileButton_Click(null, null);
					}
				}
			} else {
				if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl) && !Keyboard.IsKeyDown(Key.LeftShift)) {
					if (e.Key == Key.N) {
						NewMenuItem_Click(null, null);
					} else if (e.Key == Key.O) {
						e.Handled = true;
						OpenMenuItem_Click(null, null);
					}
				}
			}
		}

		#endregion

		#region "Initialized"

		private void MainWindow_Initialized(object sender, System.EventArgs e)
		{
			System.Threading.Thread t = new System.Threading.Thread(addhandlers);
			Plugins plugins = new Plugins();
			t.Start();
			this.Show();
			foreach (string file in Document.Editor.My.Computer.FileSystem.GetFiles(Document.Editor.My.Application.Info.DirectoryPath + "\\Templates\\")) {
				System.IO.FileInfo template = new System.IO.FileInfo(file);
				if (template.Extension == ".xaml") {
					Fluent.Button item = new Fluent.Button();
					if (Document.Editor.My.Settings.Options_Theme == 2) {
						item.Foreground = ReadOnlyMenuItem.Foreground;
					}
					item.CanAddToQuickAccessToolBar = false;
					string tname = template.Name.Remove(template.Name.Length - 5);
					if (Document.Editor.My.Computer.FileSystem.FileExists(Document.Editor.My.Application.Info.DirectoryPath + "\\Templates\\" + tname.ToLower() + ".png")) {
						BitmapImage bitmap = new BitmapImage(new Uri(Document.Editor.My.Application.Info.DirectoryPath + "\\Templates\\" + tname.ToLower() + ".png"));
						item.Icon = bitmap;
					}
					item.Header = tname;
					item.Tag = file;
					if (Document.Editor.My.Settings.Options_Theme == 2) {
						item.Foreground = ReadOnlyMenuItem.Foreground;
					}
					item.Click += new RoutedEventHandler(NewFromTemplate);
					NewMenuItem.Children.Add(item);
				}
			}
			if (Document.Editor.My.Settings.Options_EnablePlugins) {
				foreach (string file in Document.Editor.My.Computer.FileSystem.GetFiles(Document.Editor.My.Application.Info.DirectoryPath + "\\Plugins\\")) {
					try {
						System.IO.FileInfo plugin = new System.IO.FileInfo(file);
						string pluginname = plugin.Name.Remove(plugin.Name.Length - 3);
						bool IsStartupPlugin = plugins.IsStartupPlugin(pluginname, Document.Editor.My.Computer.FileSystem.ReadAllText(file));
						if (IsStartupPlugin) {
							object i = plugins.Build(pluginname, Document.Editor.My.Computer.FileSystem.ReadAllText(file));
							if (i.GetType().Name.ToString() == "String") {
								SelectedDocument.Editor.CaretPosition.InsertTextInRun(i);
							}
						}
						bool IsEventPlugin = plugins.IsEventPlugin(pluginname, Document.Editor.My.Computer.FileSystem.ReadAllText(file));
						if (IsEventPlugin) {
							Fluent.Button ribbonbutton = new Fluent.Button();
							ribbonbutton.Header = pluginname;
							if (Document.Editor.My.Computer.FileSystem.FileExists(Document.Editor.My.Application.Info.DirectoryPath + "\\Plugins\\" + pluginname + "32.png")) {
								BitmapImage bitmap = new BitmapImage(new Uri(Document.Editor.My.Application.Info.DirectoryPath + "\\Plugins\\" + pluginname + "32.png"));
								ribbonbutton.LargeIcon = bitmap;
							} else {
								BitmapImage bitmap = new BitmapImage(new Uri("pack://application:,,,/Images/Tools/plugins32.png"));
								ribbonbutton.LargeIcon = bitmap;
							}
							Fluent.ScreenTip tip = new Fluent.ScreenTip();
							tip.Title = pluginname;
							tip.Image = new BitmapImage(new Uri("pack://application:,,,/Images/Tools/plugins48.png"));
							ribbonbutton.ToolTip = tip;
							ribbonbutton.Tag = file;
							ribbonbutton.Click += new RoutedEventHandler(RunPlugin);
							PluginsGroup.Items.Add(ribbonbutton);
						}
					} catch (Exception ex) {
						MessageBoxDialog m = new MessageBoxDialog(ex.Message, ex.ToString(), null, null);
						m.MessageImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Common/error32.png"));
						m.Owner = this;
						m.ShowDialog();
					}
				}
			}
			if (PluginsGroup.Items.Count == 0) {
				PluginsGroup.Visibility = System.Windows.Visibility.Collapsed;
			}
		}

		#endregion

		#region "Loaded"

		private void MainWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			if (Document.Editor.My.Settings.MainWindow_IsMax) {
				this.WindowState = System.Windows.WindowState.Maximized;
			}
			DocPreviewScrollViewer.Content = new Canvas();
			MainGrid.Children.Add(DocPreviewScrollViewer);
			FontComboBox.SelectedItem = Document.Editor.My.Settings.Options_DefaultFont;
			var _with4 = FontSizeComboBox;
			_with4.Items.Add(Convert.ToDouble(8));
			_with4.Items.Add(Convert.ToDouble(9));
			_with4.Items.Add(Convert.ToDouble(10));
			_with4.Items.Add(Convert.ToDouble(11));
			for (i = 12; i <= 28; i += 2) {
				_with4.Items.Add(Convert.ToDouble(i));
			}
			_with4.Items.Add(Convert.ToDouble(36));
			_with4.Items.Add(Convert.ToDouble(48));
			_with4.Items.Add(Convert.ToDouble(72));
			_with4.SelectedItem = Document.Editor.My.Settings.Options_DefaultFontSize;
			if (Document.Editor.My.Settings.Options_Theme == 0) {
				dockingManager.Theme = new Xceed.Wpf.AvalonDock.Themes.AeroTheme();
			} else if (Document.Editor.My.Settings.Options_Theme == 1) {
				dockingManager.Theme = new Xceed.Wpf.AvalonDock.Themes.ExpressionLightTheme();
			} else if (Document.Editor.My.Settings.Options_Theme == 2) {
				dockingManager.Theme = new Xceed.Wpf.AvalonDock.Themes.ExpressionDarkTheme();
				NewButton.Foreground = ReadOnlyMenuItem.Foreground;
				ImportFTPConnect.Foreground = ReadOnlyMenuItem.Foreground;
				ImportFTPImportButton.Foreground = ReadOnlyMenuItem.Foreground;
				ImportArchiveMenuItem.Foreground = ReadOnlyMenuItem.Foreground;
				ImportImageMenuItem.Foreground = ReadOnlyMenuItem.Foreground;
				ExportWordpressExportButton.Foreground = ReadOnlyMenuItem.Foreground;
				ExportEmailExportButton.Foreground = ReadOnlyMenuItem.Foreground;
				ExportFTPExportButton.Foreground = ReadOnlyMenuItem.Foreground;
				ExportXPSMenuItem.Foreground = ReadOnlyMenuItem.Foreground;
				ExportArchiveMenuItem.Foreground = ReadOnlyMenuItem.Foreground;
				ExportImageMenuItem.Foreground = ReadOnlyMenuItem.Foreground;
				ExportSoundMenuItem.Foreground = ReadOnlyMenuItem.Foreground;
				PrintMenuItem.Foreground = ReadOnlyMenuItem.Foreground;
				PageSetupMenuItem.Foreground = ReadOnlyMenuItem.Foreground;
				LinesTextBlock.Foreground = Brushes.LightGray;
				ColumnsTextBlock.Foreground = Brushes.LightGray;
				WordCountTextBlock.Foreground = Brushes.LightGray;
				FileSizeTextBlock.Foreground = Brushes.LightGray;
				ZoomTextBlock.Foreground = Brushes.LightGray;
			}
			if (!Document.Editor.My.Settings.ShowCommonEditGroup) {
				CommonEditGroup.Visibility = System.Windows.Visibility.Collapsed;
			} else {
				ShowCommonEditGroupMenuItem.IsChecked = true;
			}
			if (!Document.Editor.My.Settings.ShowCommonViewGroup) {
				CommonViewGroup.Visibility = System.Windows.Visibility.Collapsed;
			} else {
				ShowCommonViewGroupMenuItem.IsChecked = true;
			}
			if (!Document.Editor.My.Settings.ShowCommonInsertGroup) {
				CommonInsertGroup.Visibility = System.Windows.Visibility.Collapsed;
			} else {
				ShowCommonInsertGroupMenuItem.IsChecked = true;
			}
			if (!Document.Editor.My.Settings.ShowCommonFormatGroup) {
				CommonFormatGroup.Visibility = System.Windows.Visibility.Collapsed;
			} else {
				ShowCommonFormatGroupMenuItem.IsChecked = true;
			}
			if (!Document.Editor.My.Settings.ShowCommonToolsGroup) {
				CommonToolsGroup.Visibility = System.Windows.Visibility.Collapsed;
			} else {
				ShowCommonToolsGroupMenuItem.IsChecked = true;
			}
			if (Document.Editor.My.Settings.Options_ShowRecentDocuments && Document.Editor.My.Settings.Options_RecentFiles.Count > 0) {
				StackPanel grid = new StackPanel();
				foreach (string s in Document.Editor.My.Settings.Options_RecentFiles) {
					if (Document.Editor.My.Computer.FileSystem.FileExists(s)) {
						Fluent.Button i2 = new Fluent.Button();
						System.IO.FileInfo f = new System.IO.FileInfo(s);
						ContextMenu ContMenu = new ContextMenu();
						MenuItem removemenuitem = new MenuItem();
						removemenuitem.Header = "Remove";
						removemenuitem.ToolTip = f.FullName;
						ContMenu.Items.Add(removemenuitem);
						i2.ContextMenu = ContMenu;
						Image img = new Image();
						if (f.Extension.ToLower() == ".xaml") {
							img.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Tab/xaml16.png"));
						} else if (f.Extension.ToLower() == ".rtf") {
							img.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Tab/rtf16.png"));
						} else if (f.Extension.ToLower() == ".html" || f.Extension.ToLower() == ".htm") {
							img.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Tab/html16.png"));
						} else {
							img.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Tab/txt16.png"));
						}
						i2.Icon = img;
						i2.Header = f.Name;
						i2.Tag = s;
						i2.Foreground = ReadOnlyMenuItem.Foreground;
						RecentFilesList.Children.Add(i2);
						(i2.Click) += new RoutedEventHandler(recentfile_click);
						removemenuitem.Click += new RoutedEventHandler(recentfileremove_click);
					}
				}
			} else {
				BackStage.Items.Remove(RecentFilesTabItem);
			}
			if (Document.Editor.My.Settings.Options_TabPlacement == 0) {
				//TabCell.TabStripPlacement = Dock.Top
			} else if (Document.Editor.My.Settings.Options_TabPlacement == 1) {
				//TabCell.TabStripPlacement = Dock.Left
			} else if (Document.Editor.My.Settings.Options_TabPlacement == 2) {
				//TabCell.TabStripPlacement = Dock.Right
			} else if (Document.Editor.My.Settings.Options_TabPlacement == 3) {
				//TabCell.TabStripPlacement = Dock.Bottom
			}
			if (Document.Editor.My.Settings.MainWindow_ShowRuler) {
				HRulerButton.IsChecked = true;
			}
			if (Document.Editor.My.Settings.MainWindow_ShowStatusBar) {
				StatusbarButton.IsChecked = true;
			} else {
				StatusBar.Visibility = System.Windows.Visibility.Collapsed;
				dockingManager.Margin = new Thickness(dockingManager.Margin.Left, dockingManager.Margin.Top, dockingManager.Margin.Right, 0);
				StatusbarButton.IsChecked = false;
			}
			try {
				if (Document.Editor.My.Application.StartUpFileNames.Count > 0) {
					if (Document.Editor.My.Settings.Options_StartupMode == 1 && Document.Editor.My.Settings.Options_PreviousDocuments.Count > 0) {
						CloseMenuItem_Click(this, null);
						foreach (string s in Document.Editor.My.Settings.Options_PreviousDocuments) {
							if (Document.Editor.My.Computer.FileSystem.FileExists(s)) {
								System.IO.FileInfo f = new System.IO.FileInfo(s);
								NewDocument(f.Name);
								SelectedDocument.Editor.LoadDocument(f.FullName);
								SelectedDocument.Editor.Height = SelectedDocument.Editor.Document.PageHeight;
								SelectedDocument.Editor.Width = SelectedDocument.Editor.Document.PageWidth;
								SelectedDocument.Ruler.Width = SelectedDocument.Editor.Width;
								Semagsoft.DocRuler.Ruler ch = SelectedDocument.Ruler.Children[0];
								ch.Width = SelectedDocument.Editor.Width;
							}
						}
					}
					foreach (string s in Document.Editor.My.Application.StartUpFileNames) {
						System.IO.FileInfo f = new System.IO.FileInfo(s);
						NewDocument(f.Name);
						SelectedDocument.Editor.LoadDocument(f.FullName);
						SelectedDocument.Editor.Height = SelectedDocument.Editor.Document.PageHeight;
						SelectedDocument.Editor.Width = SelectedDocument.Editor.Document.PageWidth;
						SelectedDocument.Ruler.Width = SelectedDocument.Editor.Width;
						Semagsoft.DocRuler.Ruler ch = SelectedDocument.Ruler.Children[0];
						ch.Width = SelectedDocument.Editor.Width;
					}
				} else {
					NewMenuItem_Click(this, null);
					if (Document.Editor.My.Settings.Options_StartupMode == 1 && Document.Editor.My.Settings.Options_PreviousDocuments.Count > 0) {
						CloseMenuItem_Click(this, null);
						foreach (string s in Document.Editor.My.Settings.Options_PreviousDocuments) {
							if (Document.Editor.My.Computer.FileSystem.FileExists(s)) {
								System.IO.FileInfo f = new System.IO.FileInfo(s);
								NewDocument(f.Name);
								SelectedDocument.Editor.LoadDocument(f.FullName);
								SelectedDocument.Editor.Height = SelectedDocument.Editor.Document.PageHeight;
								SelectedDocument.Editor.Width = SelectedDocument.Editor.Document.PageWidth;
								SelectedDocument.Ruler.Width = SelectedDocument.Editor.Width;
								Semagsoft.DocRuler.Ruler ch = SelectedDocument.Ruler.Children[0];
								ch.Width = SelectedDocument.Editor.Width;
							}
						}
					} else if (Document.Editor.My.Settings.Options_StartupMode == 2) {
						CloseMenuItem_Click(this, null);
						OpenMenuItem_Click(this, null);
					} else if (Document.Editor.My.Settings.Options_StartupMode == 3) {
						CloseMenuItem_Click(this, null);
					}
				}
			} catch (Exception ex) {
				MessageBoxDialog m = new MessageBoxDialog(ex.Message, ex.ToString(), null, null);
				m.MessageImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Common/error32.png"));
				m.Owner = this;
				m.ShowDialog();
			}
			if (Document.Editor.My.Settings.Options_ShowStartupDialog) {
				StartupDialog startup = new StartupDialog();
				startup.Owner = this;
				startup.Show();
			}
		}

		#endregion

		#region "Closing"

		private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (this.WindowState == System.Windows.WindowState.Maximized) {
				Document.Editor.My.Settings.MainWindow_IsMax = true;
			} else if (this.WindowState == System.Windows.WindowState.Normal) {
				Document.Editor.My.Settings.MainWindow_IsMax = false;
				int wi = 0;
				int hi = 0;
				int le = 0;
				int ti = 0;
				wi = Convert.ToInt32(this.ActualWidth);
				hi = Convert.ToInt32(this.ActualHeight);
				le = Convert.ToInt32(Left());
				ti = Convert.ToInt32(Top);
				System.Windows.Size s = new System.Windows.Size(wi, hi);
				Document.Editor.My.Settings.MainWindow_Left = Left();
				Document.Editor.My.Settings.MainWindow_Top = Top;
				Document.Editor.My.Settings.MainWindow_Height = Height;
				Document.Editor.My.Settings.MainWindow_Width = Width;
			}
			Document.Editor.My.Settings.ShowCommonEditGroup = ShowCommonEditGroupMenuItem.IsChecked;
			Document.Editor.My.Settings.ShowCommonViewGroup = ShowCommonViewGroupMenuItem.IsChecked;
			Document.Editor.My.Settings.ShowCommonInsertGroup = ShowCommonInsertGroupMenuItem.IsChecked;
			Document.Editor.My.Settings.ShowCommonFormatGroup = ShowCommonFormatGroupMenuItem.IsChecked;
			Document.Editor.My.Settings.ShowCommonToolsGroup = ShowCommonToolsGroupMenuItem.IsChecked;
			Document.Editor.My.Settings.MainWindow_ShowRuler = HRulerButton.IsChecked;
			Document.Editor.My.Settings.MainWindow_ShowStatusBar = StatusbarButton.IsChecked;
			if (Document.Editor.My.Settings.Options_StartupMode == 1 && Document.Editor.My.Settings.Options_PreviousDocuments.Count > 0) {
				Document.Editor.My.Settings.Options_PreviousDocuments.Clear();
			}
			while (TabCell.Children.Count > 0) {
				if (SelectedDocument.Editor.FileChanged) {
					SaveFileDialog save = new SaveFileDialog();
					save.Owner = this;
					save.SetFileInfo(SelectedDocument.DocName, SelectedDocument.Editor);
					System.IO.FileStream fs = System.IO.File.Open(Document.Editor.My.Computer.FileSystem.SpecialDirectories.Temp + "\\TVPre.xaml", System.IO.FileMode.Create);
					TextRange tr = new TextRange(SelectedDocument.Editor.Document.ContentStart, SelectedDocument.Editor.Document.ContentEnd);
					System.Windows.Markup.XamlWriter.Save(SelectedDocument.Editor.Document, fs);
					fs.Close();
					Document.Editor.My.Settings.Options_PreviousDocuments.Clear();
					save.ShowDialog();
					if (save.Res == "Yes") {
						if (SelectedDocument.Editor.DocumentName != null) {
							SelectedDocument.Editor.SaveDocument(SelectedDocument.Editor.DocumentName);
							Document.Editor.My.Settings.Options_PreviousDocuments.Add(SelectedDocument.Editor.DocumentName);
							CloseDocument(SelectedDocument);
						} else {
							SaveMenuItem_Click(null, null);
							if (SelectedDocument.Editor.DocumentName != null) {
								Document.Editor.My.Settings.Options_PreviousDocuments.Add(SelectedDocument.Editor.DocumentName);
							}
						}
					} else if (save.Res == "No") {
						if (SelectedDocument.Editor.DocumentName != null) {
							Document.Editor.My.Settings.Options_PreviousDocuments.Add(SelectedDocument.Editor.DocumentName);
						}
						CloseDocument(SelectedDocument);
					} else {
						e.Cancel = true;
						break; // TODO: might not be correct. Was : Exit While
					}
				} else {
					if (SelectedDocument.Editor.DocumentName != null) {
						Document.Editor.My.Settings.Options_PreviousDocuments.Add(SelectedDocument.Editor.DocumentName);
					}
					CloseDocument(SelectedDocument);
				}
			}
			Document.Editor.My.Settings.Save();
		}

		#endregion

		private void MainWindow_StateChanged(object sender, EventArgs e)
		{
			if (IsFullscreen && WindowState == System.Windows.WindowState.Normal) {
				FullscreenMenuItem.IsChecked = false;
			}
		}

		#endregion

		#region "Ribbon"

		#region "--Common"

		private void CommonViewGroup_LauncherClick()
		{
			MainBar.SelectedTabItem = ViewTab;
		}

		private void CommonInsertGroup_LauncherClick()
		{
			MainBar.SelectedTabItem = InsertTab;
		}

		private void ShowCommonEditGroupMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (ShowCommonEditGroupMenuItem.IsChecked) {
				ShowCommonEditGroupMenuItem.IsChecked = false;
				CommonEditGroup.Visibility = System.Windows.Visibility.Collapsed;
			} else {
				ShowCommonEditGroupMenuItem.IsChecked = true;
				CommonEditGroup.Visibility = System.Windows.Visibility.Visible;
			}
		}

		private void ShowCommonViewGroupMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (ShowCommonViewGroupMenuItem.IsChecked) {
				ShowCommonViewGroupMenuItem.IsChecked = false;
				CommonViewGroup.Visibility = System.Windows.Visibility.Collapsed;
			} else {
				ShowCommonViewGroupMenuItem.IsChecked = true;
				CommonViewGroup.Visibility = System.Windows.Visibility.Visible;
			}
		}

		private void ShowCommonInsertGroupMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (ShowCommonInsertGroupMenuItem.IsChecked) {
				ShowCommonInsertGroupMenuItem.IsChecked = false;
				CommonInsertGroup.Visibility = System.Windows.Visibility.Collapsed;
			} else {
				ShowCommonInsertGroupMenuItem.IsChecked = true;
				CommonInsertGroup.Visibility = System.Windows.Visibility.Visible;
			}
		}

		private void ShowCommonFormatGroupMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (ShowCommonFormatGroupMenuItem.IsChecked) {
				ShowCommonFormatGroupMenuItem.IsChecked = false;
				CommonFormatGroup.Visibility = System.Windows.Visibility.Collapsed;
			} else {
				ShowCommonFormatGroupMenuItem.IsChecked = true;
				CommonFormatGroup.Visibility = System.Windows.Visibility.Visible;
			}
		}

		private void ShowCommonToolsGroupMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (ShowCommonToolsGroupMenuItem.IsChecked) {
				ShowCommonToolsGroupMenuItem.IsChecked = false;
				CommonToolsGroup.Visibility = System.Windows.Visibility.Collapsed;
			} else {
				ShowCommonToolsGroupMenuItem.IsChecked = true;
				CommonToolsGroup.Visibility = System.Windows.Visibility.Visible;
			}
		}

		#endregion

		#region "--DocumentMenu"

		private void BackStageMenu_IsOpenChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (BackStageMenu.IsOpen && SelectedDocument != null) {
				TextRange tr = new TextRange(SelectedDocument.Editor.Document.ContentStart, SelectedDocument.Editor.Document.ContentEnd);
				int lineint = 0;
				TextPointer ls2 = SelectedDocument.Editor.Document.ContentStart.DocumentEnd.GetLineStartPosition(0);
				TextPointer p2 = SelectedDocument.Editor.Document.ContentEnd.DocumentStart.GetLineStartPosition(0);
				while (true) {
					if (ls2.CompareTo(p2) < 1) {
						break; // TODO: might not be correct. Was : Exit While
					}
					int r = 0;
					p2 = p2.GetLineStartPosition(1, out r);
					if (r == 0) {
						break; // TODO: might not be correct. Was : Exit While
					}
					lineint += 1;
				}
				int p = 0;
				foreach (Block b in SelectedDocument.Editor.Document.Blocks) {
					p += 1;
				}
				Int32 CC = tr.Text.Length - 2;
				Int32 LC = lineint + 1;
				Int32 PC = p;
				StatisticsCharacterCountTextBlock.Text = "Character Count: " + CC.ToString();
				StatisticsWordCountTextBlock.Text = "Word Count: " + SelectedDocument.Editor.GetWordCount().ToString();
				StatisticsLineCountTextBlock.Text = "Line Count: " + LC.ToString();
				StatisticsParagraphCountTextBlock.Text = "Paragraph Count: " + PC.ToString();
				UpdateDocumentPreview();
				string s = System.IO.Path.GetRandomFileName();
				System.Windows.Xps.Packaging.XpsDocument doc = new System.Windows.Xps.Packaging.XpsDocument(s, System.IO.FileAccess.ReadWrite);
				System.Windows.Xps.XpsDocumentWriter xw = System.Windows.Xps.Packaging.XpsDocument.CreateXpsDocumentWriter(doc);
				xw.Write(DocumentPreview);
				doc.Close();
				System.Windows.Xps.Packaging.XpsDocument xpsdoc = new System.Windows.Xps.Packaging.XpsDocument(s, System.IO.FileAccess.Read);
				FixedDocumentSequence fds = xpsdoc.GetFixedDocumentSequence();
				PrintPreview.Document = fds;
			}
		}

		#region "New"

		private void NewMenuItem_Click(System.Object sender, System.Windows.RoutedEventArgs e)
		{
			NewDocument("New Document");
			//SelectedDocument.HeaderContent.FileTypeImage.ToolTip = ".xaml"
		}

		private void NewFromTemplate(object sender, System.Windows.RoutedEventArgs e)
		{
			Fluent.Button t = e.Source;
			System.IO.FileInfo template = new System.IO.FileInfo(t.Tag);
			System.IO.FileStream fs = new System.IO.FileStream(template.FullName, System.IO.FileMode.Open);
			FlowDocument flow = new FlowDocument();
			flow = System.Windows.Markup.XamlReader.Load(fs) as FlowDocument;
			fs.Close();
			NewDocument("New Document");
			//SelectedDocument.HeaderContent.FileTypeImage.ToolTip = ".xaml"
			Thickness thi = flow.PagePadding;
			SelectedDocument.Editor.Document = flow;
			SelectedDocument.Editor.Focus();
			SelectedDocument.Editor.DocumentName = null;
			SelectedDocument.Editor.FileChanged = false;
			SelectedDocument.Editor.Height = SelectedDocument.Editor.Document.PageHeight;
			SelectedDocument.Editor.Width = SelectedDocument.Editor.Document.PageWidth;
			SelectedDocument.Ruler.Width = SelectedDocument.Editor.Width;
			Semagsoft.DocRuler.Ruler ch = SelectedDocument.Ruler.Children[0];
			ch.Width = SelectedDocument.Editor.Width;
			try {
				int leftmargin = thi.Left;
				int topmargin = thi.Top;
				int rightmargin = thi.Right;
				int bottommargin = thi.Bottom;
				SelectedDocument.Editor.SetPageMargins(new Thickness(leftmargin, topmargin, rightmargin, bottommargin));
			} catch (Exception ex) {
				SelectedDocument.Editor.SetPageMargins(new Thickness(0, 0, 0, 0));
			}
		}

		#endregion

		#region "Open"

		private void OpenMenuItem_Click(System.Object sender, System.Windows.RoutedEventArgs e)
		{
			Microsoft.Win32.OpenFileDialog openfile = new Microsoft.Win32.OpenFileDialog();
			openfile.Multiselect = true;
			openfile.Filter = "Supported Documents(*.xamlpackage;*.xaml,*.docx,*.html,*.htm,*.rtf,*.txt)|*.xamlpackage;*.xaml;*.docx;*.html;*.htm;*.rtf;*.txt|XAML Packages(*.xamlpackage)|*.xamlpackage|FlowDocuments(*.xaml)|*.xaml|OpenXML Documents(*.docx)|*.docx|HTML Documents(*.html;*.htm)|*.html;*.htm|Rich Text Documents(*.rtf)|*.rtf|Plan Text Documents(*.txt)|*.txt|All Files(*.*)|*.*";
			if (Document.Editor.My.Computer.Info.OSVersion >= "6.1") {
				this.TaskbarItemInfo.Overlay = (ImageSource)System.Resources("OpenOverlay");
			}
			if (openfile.ShowDialog() == true) {
				if (Document.Editor.My.Computer.Info.OSVersion >= "6.1") {
					this.TaskbarItemInfo.ProgressState = Shell().TaskbarItemProgressState.Normal;
				}
				int itemcount = 0;
				LoadFileDialog load = new LoadFileDialog();
				int @int = 0;
				foreach (string s in openfile.FileNames) {
					itemcount += 1;
				}
				this.IsEnabled = false;
				load.Show();
				foreach (string i in openfile.FileNames) {
					System.IO.FileInfo f = new System.IO.FileInfo(i);
					NewDocument(f.Name);
					SelectedDocument.Editor.LoadDocument(f.FullName);
					SelectedDocument.SetFileType(f.Extension);
					SelectedDocument.Editor.FileChanged = false;
					SelectedDocument.Editor.Height = SelectedDocument.Editor.Document.PageHeight;
					SelectedDocument.Editor.Width = SelectedDocument.Editor.Document.PageWidth;
					SelectedDocument.Ruler.Width = SelectedDocument.Editor.Width;
					Semagsoft.DocRuler.Ruler ch = SelectedDocument.Ruler.Children[0];
					ch.Width = SelectedDocument.Editor.Width;
					if (!Document.Editor.My.Settings.Options_RecentFiles.Contains(SelectedDocument.Editor.DocumentName) && Document.Editor.My.Settings.Options_RecentFiles.Count < 13) {
						Document.Editor.My.Settings.Options_RecentFiles.Add(SelectedDocument.Editor.DocumentName);
					}
					if (Document.Editor.My.Computer.Info.OSVersion >= "6.1") {
						TaskbarItemInfo.ProgressValue = Convert.ToDouble(@int) / itemcount;
						@int += 1;
					}
				}
				load.i = true;
				load.Close();
				this.IsEnabled = true;
			}
			if (Document.Editor.My.Computer.Info.OSVersion >= "6.1") {
				this.TaskbarItemInfo.Overlay = null;
				this.TaskbarItemInfo.ProgressState = Shell().TaskbarItemProgressState.None;
			}
		}

		#endregion

		#region "Recent"

		private void recentfile_click(object sender, System.Windows.RoutedEventArgs e)
		{
			Fluent.Button i = e.Source;
			System.IO.FileInfo f = new System.IO.FileInfo(i.Tag);
			NewDocument(f.Name);
			SelectedDocument.Editor.LoadDocument(Convert.ToString(f.FullName));
			SelectedDocument.SetFileType(f.Extension);
			UpdateButtons();
			SelectedDocument.Editor.Focus();
		}

		private void recentfileremove_click(object sender, System.Windows.RoutedEventArgs e)
		{
			MenuItem i = e.Source;
			Fluent.Button itemtoremove = null;
			foreach (Fluent.Button recentdoc in RecentFilesList.Children) {
				if (recentdoc.Tag == i.ToolTip) {
					itemtoremove = recentdoc;
				}
			}
			string stringtoremove = null;
			foreach (string s in Document.Editor.My.Settings.Options_RecentFiles) {
				if (s == i.ToolTip) {
					stringtoremove = s;
				}
			}
			Document.Editor.My.Settings.Options_RecentFiles.Remove(stringtoremove);
			RecentFilesList.Children.Remove(itemtoremove);
		}

		#endregion

		#region "Revert"

		private void RevertMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			SelectedDocument.Editor.LoadDocument(SelectedDocument.Editor.DocumentName);
			SelectedDocument.Editor.Height = SelectedDocument.Editor.Document.PageHeight;
			SelectedDocument.Editor.Width = SelectedDocument.Editor.Document.PageWidth;
			SelectedDocument.Ruler.Width = SelectedDocument.Editor.Width;
			Semagsoft.DocRuler.Ruler ch = SelectedDocument.Ruler.Children[0];
			ch.Width = SelectedDocument.Editor.Width;
			UpdateButtons();
		}

		#endregion

		#region "Close/Close All But This/Close All"

		private void CloseMenuItem_Click(System.Object sender, System.Windows.RoutedEventArgs e)
		{
			if (SelectedDocument.Editor.FileChanged) {
				SaveFileDialog SaveDialog = new SaveFileDialog();
				TextRange tr = new TextRange(SelectedDocument.Editor.Document.ContentStart, SelectedDocument.Editor.Document.ContentEnd);
				System.IO.FileStream fs = System.IO.File.Open(Document.Editor.My.Computer.FileSystem.SpecialDirectories.Temp + "\\TVPre.xaml", System.IO.FileMode.Create);
				SaveDialog.Owner = this;
				SaveDialog.SetFileInfo(SelectedDocument.DocName, SelectedDocument.Editor);
				System.Windows.Markup.XamlWriter.Save(SelectedDocument.Editor.Document, fs);
				fs.Close();
				SaveDialog.ShowDialog();
				if (SaveDialog.Res == "Yes") {
					SaveMenuItem_Click(null, null);
					CloseDocument(SelectedDocument);
				} else if (SaveDialog.Res == "No") {
					CloseDocument(SelectedDocument);
				}
			} else {
				CloseDocument(SelectedDocument);
			}
		}

		private void CloseAllButThisMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			while (TabCell.Children.Count > 1) {
				if (!object.ReferenceEquals(TabCell.Children.Item(0), SelectedDocument)) {
					DocumentTab t = TabCell.Children.Item(0) as DocumentTab;
					if (t.Editor.FileChanged) {
						SaveFileDialog SaveDialog = new SaveFileDialog();
						TextRange tr = new TextRange(t.Editor.Document.ContentStart, t.Editor.Document.ContentEnd);
						System.IO.FileStream fs = System.IO.File.Open(Document.Editor.My.Computer.FileSystem.SpecialDirectories.Temp + "\\TVPre.xaml", System.IO.FileMode.Create);
						SaveDialog.Owner = this;
						SaveDialog.SetFileInfo(t.DocName, t.Editor);
						System.Windows.Markup.XamlWriter.Save(t.Editor.Document, fs);
						fs.Close();
						SaveDialog.ShowDialog();
						if (SaveDialog.Res == "Yes") {
							if (t.Editor.DocumentName != null) {
								t.Editor.SaveDocument(t.Editor.DocumentName);
								TabCell.Children.RemoveAt(0);
							} else {
								Microsoft.Win32.SaveFileDialog d = new Microsoft.Win32.SaveFileDialog();
								d.Filter = "";
								if (d.ShowDialog()) {
									System.IO.FileInfo file = new System.IO.FileInfo(d.FileName);
									t.Editor.SaveDocument(d.FileName);
									TabCell.Children.RemoveAt(0);
								}
							}
						} else if (SaveDialog.Res == "No") {
							TabCell.Children.RemoveAt(0);
						} else {
							break; // TODO: might not be correct. Was : Exit While
						}
					} else {
						TabCell.Children.RemoveAt(0);
					}
				} else {
					DocumentTab t = TabCell.Children.Item(1) as DocumentTab;
					if (t.Editor.FileChanged) {
						SaveFileDialog SaveDialog = new SaveFileDialog();
						TextRange tr = new TextRange(t.Editor.Document.ContentStart, t.Editor.Document.ContentEnd);
						System.IO.FileStream fs = System.IO.File.Open(Document.Editor.My.Computer.FileSystem.SpecialDirectories.Temp + "\\TVPre.xaml", System.IO.FileMode.Create);
						SaveDialog.Owner = this;
						SaveDialog.SetFileInfo(t.DocName, t.Editor);
						System.Windows.Markup.XamlWriter.Save(SelectedDocument.Editor.Document, fs);
						fs.Close();
						SaveDialog.ShowDialog();
						if (SaveDialog.Res == "Yes") {
							if (t.Editor.DocumentName != null) {
								t.Editor.SaveDocument(t.Editor.DocumentName);
								TabCell.Children.RemoveAt(1);
							} else {
								Microsoft.Win32.SaveFileDialog d = new Microsoft.Win32.SaveFileDialog();
								d.AddExtension = true;
								d.Filter = "";
								d.FileName = t.DocName;
								if (d.ShowDialog()) {
									System.IO.FileInfo file = new System.IO.FileInfo(d.FileName);
									t.Editor.SaveDocument(d.FileName);
									TabCell.Children.RemoveAt(1);
								}
							}
						} else if (SaveDialog.Res == "No") {
							TabCell.Children.RemoveAt(1);
						} else {
							break; // TODO: might not be correct. Was : Exit While
						}
					} else {
						TabCell.Children.RemoveAt(1);
					}
				}
			}
		}

		private void CloseAllMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			while (TabCell.Children.Count > 0) {
				if (SelectedDocument.Editor.FileChanged == true) {
					SaveFileDialog save = new SaveFileDialog();
					TextRange tr = new TextRange(SelectedDocument.Editor.Document.ContentStart, SelectedDocument.Editor.Document.ContentEnd);
					System.IO.FileStream fs = System.IO.File.Open(Document.Editor.My.Computer.FileSystem.SpecialDirectories.Temp + "\\TVPre.xaml", System.IO.FileMode.Create);
					save.SetFileInfo(SelectedDocument.DocName, SelectedDocument.Editor);
					System.Windows.Markup.XamlWriter.Save(SelectedDocument.Editor.Document, fs);
					fs.Close();
					save.ShowDialog();
					if (save.Res == "Yes") {
						SaveMenuItem_Click(this, null);
						CloseDocument(SelectedDocument);
					} else if (save.Res == "No") {
						CloseDocument(SelectedDocument);
					} else {
						break; // TODO: might not be correct. Was : Exit While
					}
				} else {
					CloseDocument(SelectedDocument);
				}
			}
		}

		#endregion

		#region "Save/Save As/Save Copy/Save All"

		private void SaveMenuItem_Click(System.Object sender, System.Windows.RoutedEventArgs e)
		{
			if (SelectedDocument.Editor.DocumentName == null) {
				SaveAsMenuItem_Click(this, null);
			} else if (Document.Editor.My.Computer.FileSystem.FileExists(SelectedDocument.Editor.DocumentName)) {
				SelectedDocument.Editor.SaveDocument(SelectedDocument.Editor.DocumentName);
			}
			UpdateButtons();
		}

		private void SaveAsMenuItem_Click(System.Object sender, System.Windows.RoutedEventArgs e)
		{
			Microsoft.Win32.SaveFileDialog save = new Microsoft.Win32.SaveFileDialog();
			save.Filter = "XAML Package(*.xamlpackage)|*.xamlpackage|FlowDocument(*.xaml)|*.xaml|OpenXML Document(*.docx)|*.docx|HTML Document(*.html;*.htm)|*.html;*.htm|Rich Text Document(*.rtf)|*.rtf|Text Document(*.txt)|*.txt|All Files(*.*)|*.*";
			if (save.ShowDialog()) {
				SelectedDocument.Editor.SaveDocument(save.FileName);
				SelectedDocument.DocName = new System.IO.FileInfo(save.FileName).Name;
				SelectedDocument.Title = SelectedDocument.DocName;
				SelectedDocument.SetFileType(new System.IO.FileInfo(save.FileName).Extension);
			}
			UpdateButtons();
		}

		private void SaveCopyMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Microsoft.Win32.SaveFileDialog save = new Microsoft.Win32.SaveFileDialog();
			save.AddExtension = true;
			save.Title = "Save Copy";
			save.Filter = "XAML Package(*.xamlpackage)|*.xamlpackage|FlowDocument(*.xaml)|*.xaml|OpenXML Document(*.docx)|*.docx|HTML Document(*.html;*.htm)|*.html;*.htm|Rich Text Document(*.rtf)|*.rtf|Text Document(*.txt)|*.txt|All Files(.)|*.*";
			if (save.ShowDialog()) {
				System.IO.FileInfo file = new System.IO.FileInfo(save.FileName);
				System.IO.FileStream fs = System.IO.File.Open(save.FileName, System.IO.FileMode.OpenOrCreate);
				TextRange tr = new TextRange(SelectedDocument.Editor.Document.ContentStart, SelectedDocument.Editor.Document.ContentEnd);
				DocumentTab p = Parent;
				if (file.Extension == ".xaml") {
					System.Windows.Markup.XamlWriter.Save(SelectedDocument.Editor.Document, fs);
				} else if (file.Extension == ".docx") {
					fs.Close();
					fs = null;
					FlowDocumenttoOpenXML converter = new FlowDocumenttoOpenXML();
					converter.Convert(SelectedDocument.Editor.Document, save.FileName);
					converter.Close();
				} else if (file.Extension == ".html") {
					fs.Close();
					fs = null;
					string s = System.Windows.Markup.XamlWriter.Save(SelectedDocument.Editor.Document);
					try {
						Document.Editor.My.Computer.FileSystem.WriteAllText(file.FullName, HTMLConverter.HtmlFromXamlConverter.ConvertXamlToHtml(s), false);
					} catch (Exception ex) {
						MessageBoxDialog m = new MessageBoxDialog("Error saving document!", "Error", null, null);
						m.MessageImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Common/error32.png"));
						m.Owner = this;
						m.ShowDialog();
					}
				} else if (file.Extension == ".rtf") {
					tr.Save(fs, System.Windows.DataFormats.Rtf);
				} else if (file.Extension == ".txt") {
					tr.Save(fs, System.Windows.DataFormats.Text);
				} else {
					tr.Save(fs, System.Windows.DataFormats.Text);
				}
				fs.Close();
			}
		}

		private void SaveAllMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			int @int = 0;
			if (Document.Editor.My.Computer.Info.OSVersion >= "6.1") {
				this.TaskbarItemInfo.Overlay = (ImageSource)System.Resources("SaveOverlay");
				TaskbarItemInfo.ProgressState = Shell().TaskbarItemProgressState.Normal;
			}
			foreach (DocumentTab i in TabCell.Children) {
				if (i.Editor.DocumentName != null) {
					i.Editor.SaveDocument(i.Editor.DocumentName);
				} else {
					SaveAsMenuItem_Click(i, null);
				}
				if (Document.Editor.My.Computer.Info.OSVersion >= "6.1") {
					TaskbarItemInfo.ProgressValue = Convert.ToDouble(@int) / TabCell.Children.Count;
					@int += 1;
				}
			}
			if (Document.Editor.My.Computer.Info.OSVersion >= "6.1") {
				this.TaskbarItemInfo.Overlay = null;
				TaskbarItemInfo.ProgressState = Shell().TaskbarItemProgressState.None;
			}
		}

		#endregion

		#region "Import"

		#region "FTP"

		private bool ImportFTPIsConnected = false;
		private string ImportFTPWorkingDir = "/";
		private string ImportFTPFileToLoad = null;
		private void ImportFTPHostBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			try {
				if (ImportFTPHostBox.Text.Length > 0 && ImportFTPUsernameBox.Text.Length > 0 && ImportFTPPasswordBox.Text.Length > 0) {
					ImportFTPConnect.IsEnabled = true;
				}
			} catch (Exception ex) {
			}
		}

		private void ImportFTPConnect_Click(object sender, RoutedEventArgs e)
		{
			if (ImportFTPIsConnected) {
				ImportFTPIsConnected = false;
				ImportFTPHostBox.IsEnabled = true;
				ImportFTPUsernameBox.IsEnabled = true;
				ImportFTPPasswordBox.IsEnabled = true;
				ImportFTPConnect.Header = "Connect";
				ImportFTPConnect.Icon = new BitmapImage(new Uri("pack://application:,,,/Images/Document/Export/connect16.png"));
				//Dim ttip As Fluent.ScreenTip = ImportFTPConnect.ToolTip
				//ttip.Title = "Connect"
				//ttip.Text = "Connect to FTP Server"
				ImportFTPListBox.Items.Clear();
			} else {
				//Try
				Utilities.FTP.FTPclient myFtp = new Utilities.FTP.FTPclient(ImportFTPHostBox.Text, ImportFTPUsernameBox.Text, ImportFTPPasswordBox.Text);
				ImportFTPIsConnected = true;
				ImportFTPHostBox.IsEnabled = false;
				ImportFTPUsernameBox.IsEnabled = false;
				ImportFTPPasswordBox.IsEnabled = false;
				ImportFTPConnect.Header = "Disconnect";
				ImportFTPConnect.Icon = new BitmapImage(new Uri("pack://application:,,,/Images/Document/Export/disconnect16.png"));
				//Dim ttip As Fluent.ScreenTip = ImportFTPConnect.ToolTip
				//ttip.Title = "Disconnect"
				//ttip.Text = "Disconnect from FTP Server"
				foreach (Utilities.FTP.FTPfileInfo i in myFtp.ListDirectoryDetail(ImportFTPWorkingDir)) {
					if (!(i.NameOnly == ".")) {
						FTPItem item = new FTPItem();
						item.Content = i.Filename;
						item.Name = i.NameOnly;
						item.FileName = i.Filename;
						item.FullName = i.FullName;
						ImportFTPListBox.Items.Add(item);
					}
				}
				//Catch ex As Exception
				//Dim m As New MessageBoxDialog(ex.message, "Error", Nothing, Nothing)
				//m.MessageImage.Source = New BitmapImage(New Uri("pack://application:,,,/Images/Common/error32.png"))
				//m.Owner = Me
				//m.ShowDialog()
				//End Try
			}
			BackStageMenu.IsOpen = true;
		}

		private void ImportFTPListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (ImportFTPListBox.SelectedItem != null) {
				FTPItem i = ImportFTPListBox.SelectedItem;
				if (i.IsFile) {
					ImportFTPImportButton.IsEnabled = true;
				} else {
					ImportFTPImportButton.IsEnabled = false;
				}
			}
		}

		private void ImportFTPListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (ImportFTPListBox.SelectedItem != null) {
				FTPItem i = ImportFTPListBox.SelectedItem;
				if (i.IsFile) {
					ImportFTPImportButton_Click(null, null);
				} else {
					Utilities.FTP.FTPclient myFtp = new Utilities.FTP.FTPclient(ImportFTPHostBox.Text, ImportFTPUsernameBox.Text, ImportFTPPasswordBox.Text);
					if (i.IsFile) {
						ImportFTPWorkingDir += i.FileName;
					} else {
						ImportFTPWorkingDir += i.FileName + "/";
					}
					ImportFTPListBox.Items.Clear();
					foreach (Utilities.FTP.FTPfileInfo i2 in myFtp.ListDirectoryDetail(ImportFTPWorkingDir)) {
						FTPItem item = new FTPItem();
						item.Content = i2.Filename;
						item.Name = i2.NameOnly;
						item.FileName = i2.Filename;
						item.FullName = i2.FullName;
						if (i2.FileType == Document.Editor.Utilities.FTP.FTPfileInfo.DirectoryEntryTypes.File) {
							item.IsFile = true;
						}
						ImportFTPListBox.Items.Add(item);
					}
				}
			}
		}

		private void ImportFTPImportButton_Click(object sender, RoutedEventArgs e)
		{
			try {
				FTPItem i = ImportFTPListBox.SelectedItem;
				Utilities.FTP.FTPclient myFtp = new Utilities.FTP.FTPclient(ImportFTPHostBox.Text, ImportFTPUsernameBox.Text, ImportFTPPasswordBox.Text);
				string file = Document.Editor.My.Computer.FileSystem.SpecialDirectories.Temp + "\\" + i.FileName;
				if (Document.Editor.My.Computer.FileSystem.FileExists(file)) {
					Document.Editor.My.Computer.FileSystem.DeleteFile(file);
				}
				myFtp.Download(ImportFTPWorkingDir + i.FileName, file);
				ImportFTPFileToLoad = file;
				NewMenuItem_Click(null, null);
				SelectedDocument.Editor.LoadDocument(ImportFTPFileToLoad);
				SelectedDocument.DocName = "New Document";
				SelectedDocument.Editor.DocumentName = null;
				SelectedDocument.SetDocumentTitle("New Document");
				Document.Editor.My.Computer.FileSystem.DeleteFile(ImportFTPFileToLoad);
			} catch (Exception ex) {
				MessageBoxDialog m = new MessageBoxDialog(ex.Message, "Error", null, null);
				m.MessageImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Common/error32.png"));
				m.Owner = this;
				m.ShowDialog();
			}
		}

		#endregion

		private void ImportArchiveMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Microsoft.Win32.OpenFileDialog import = new Microsoft.Win32.OpenFileDialog();
			import.Title = "Import Archive";
			import.Filter = "Zip Archive(*.zip)|*.zip|All Files(*.*)|*.*";
			if (import.ShowDialog()) {
				Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile(import.FileName);
				foreach (Ionic.Zip.ZipEntry item in zip.Entries) {
					item.Extract(Document.Editor.My.Computer.FileSystem.SpecialDirectories.CurrentUserApplicationData + "\\Semagsoft", Ionic.Zip.ExtractExistingFileAction.OverwriteSilently);
					System.IO.FileInfo file = new System.IO.FileInfo(Document.Editor.My.Computer.FileSystem.SpecialDirectories.CurrentUserApplicationData + "\\Semagsoft\\" + item.FileName);
					DocumentTab tab = new DocumentTab(file.Name, Background);
					tab.Editor.LoadDocument(file.FullName);
					tab.IsSelected = true;
				}
			}
		}

		private void ImportImageMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Microsoft.Win32.OpenFileDialog import = new Microsoft.Win32.OpenFileDialog();
			import.Title = "Import Image";
			import.Filter = "Supported Images(*.bmp;*.gif;*.jpeg;*.jpg;*.png)|*.bmp;*.gif;*.jpeg;*.jpg;*.png|BMP Images(*.bmp)|*.bmp|GIF Images(*.gif)|*.gif|JPEG Images(*.jpeg;*.jpg)|*.jpeg;*.jpg|PNG Images(*.png)|*.png|All Files(*.*)|*.*";
			if (import.ShowDialog()) {
				DocumentTab tab = new DocumentTab("New Document", Background);
				Image img = new Image();
				BitmapImage b = new BitmapImage();
				TabCell.Children.Add(tab);
				tab.IsSelected = true;
				SelectedDocument = tab;
				TextPointer t = SelectedDocument.Editor.CaretPosition;
				b.BeginInit();
				b.UriSource = new Uri(import.FileName);
				b.EndInit();
				img.Height = b.Height;
				img.Width = b.Width;
				img.Source = b;
				InlineUIContainer inline = new InlineUIContainer(img);
				t.Paragraph.Inlines.Add(inline);
				SelectedDocument.Editor.FileChanged = false;
				UpdateButtons();
			}
		}

		#endregion

		#region "Export"

		#region "Export Xps"

		private void ExportXPSMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			UpdateDocumentPreview();
			Microsoft.Win32.SaveFileDialog save = new Microsoft.Win32.SaveFileDialog();
			save.Title = "Export XPS";
			save.Filter = "XPS Document(*.xps)|*.xps|All Files(*.*)|*.*";
			save.AddExtension = true;
			if (save.ShowDialog()) {
				System.Windows.Xps.Packaging.XpsDocument NewXpsDocument = new System.Windows.Xps.Packaging.XpsDocument(save.FileName, System.IO.FileAccess.ReadWrite);
				System.Windows.Xps.XpsDocumentWriter xpsw = System.Windows.Xps.Packaging.XpsDocument.CreateXpsDocumentWriter(NewXpsDocument);
				xpsw.Write(DocumentPreview);
				NewXpsDocument.Close();
				xpsw = null;
			}
		}

		#endregion

		#region "Export Wordpress"

		private void ExportWordpressBlogBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			try {
				if (ExportWordpressBlogBox.Text.Length > 0 && ExportWordpressTitleBox.Text.Length > 0 && ExportWordpressUsernameBox.Text.Length > 0 && ExportWordpressPasswordBox.Text.Length > 0) {
					ExportWordpressExportButton.IsEnabled = true;
				} else {
					ExportWordpressExportButton.IsEnabled = false;
				}
			} catch (Exception ex) {
			}
		}

		private void ExportWordpressExportButton_Click(object sender, RoutedEventArgs e)
		{
			BackStageMenu.IsOpen = true;
			AppHelper.blogInfo newBlogPost = null;
			newBlogPost.title = ExportWordpressTitleBox.Text;
			TextRange t = new TextRange(SelectedDocument.Editor.Document.ContentStart, SelectedDocument.Editor.Document.ContentEnd);
			newBlogPost.description = t.Text;
			AppHelper.IgetCatList categories = (AppHelper.IgetCatList)CookComputing.XmlRpc.XmlRpcProxyGen.Create(typeof(AppHelper.IgetCatList));
			CookComputing.XmlRpc.XmlRpcClientProtocol clientProtocol = (CookComputing.XmlRpc.XmlRpcClientProtocol)categories;
			clientProtocol.Url = ExportWordpressBlogBox.Text + "/xmlrpc.php";
			string result = null;
			result = "";
			try {
				result = categories.NewPage(1, ExportWordpressUsernameBox.Text, ExportWordpressPasswordBox.Text, newBlogPost, 1);
				MessageBoxDialog m = new MessageBoxDialog("Posted to Blog successfullly! Post ID : " + result, "Success", null, null);
				m.MessageImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Common/info32.png"));
				m.Owner = this;
				m.ShowDialog();
			} catch (Exception ex) {
				MessageBoxDialog m = new MessageBoxDialog(ex.Message, "Error", null, null);
				m.MessageImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Common/error32.png"));
				m.Owner = this;
				m.ShowDialog();
			}
		}

		#endregion

		#region "Export Email"

		private void ExportEmailFromBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			try {
				if (ExportEmailFromBox.Text.Length > 0 && ExportEmailToBox.Text.Length > 0 && ExportEmailSubjectBox.Text.Length > 0 && ExportEmailBodyBox.Text.Length > 0 && ExportEmailPasswordBox.Text.Length > 0) {
					ExportEmailExportButton.IsEnabled = true;
				} else {
					ExportEmailExportButton.IsEnabled = false;
				}

				if (ExportEmailSMTPBox.SelectedItem == null) {
					ExportEmailExportButton.IsEnabled = false;
				}
			} catch (Exception ex) {
			}
		}

		private void ExportEmailExportButton_Click(object sender, RoutedEventArgs e)
		{
			System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
			client.UseDefaultCredentials = false;
			client.Credentials = new System.Net.NetworkCredential(ExportEmailFromBox.Text, ExportEmailPasswordBox.Text);
			client.Port = Convert.ToInt32(ExportEmailPortBox.Value);
			client.Host = ExportEmailSMTPBox.Text;
			client.EnableSsl = true;
			System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
			string[] addr = ExportEmailToBox.Text.Split(",");
			try {
				mail.From = new System.Net.Mail.MailAddress(ExportEmailFromBox.Text, ExportEmailFromBox.Text, System.Text.Encoding.UTF8);
				byte i = 0;
				for (i = 0; i <= addr.Length - 1; i++) {
					mail.To.Add(addr[i]);
				}
				mail.Subject = ExportEmailSubjectBox.Text;
				mail.Body = ExportEmailBodyBox.Text;
				System.Net.Mail.Attachment attach = null;
				if (SelectedDocument.Editor.DocumentName != null) {
					attach = new System.Net.Mail.Attachment(SelectedDocument.Editor.DocumentName);
				} else {
					if (!Document.Editor.My.Computer.FileSystem.DirectoryExists(Document.Editor.My.Application.Info.DirectoryPath + "\\Temp")) {
						Document.Editor.My.Computer.FileSystem.CreateDirectory(Document.Editor.My.Application.Info.DirectoryPath + "\\Temp");
					}
					System.IO.FileInfo file = new System.IO.FileInfo(Document.Editor.My.Application.Info.DirectoryPath + "\\Temp\\document.xaml");
					System.IO.FileStream fs = System.IO.File.Open(file.FullName, System.IO.FileMode.OpenOrCreate);
					TextRange tr = new TextRange(SelectedDocument.Editor.Document.ContentStart, SelectedDocument.Editor.Document.ContentEnd);
					tr.Save(fs, System.Windows.DataFormats.Xaml);
					fs.Close();
					attach = new System.Net.Mail.Attachment(file.FullName);
				}
				mail.Attachments.Add(attach);
				mail.DeliveryNotificationOptions = System.Net.Mail.DeliveryNotificationOptions.OnFailure;
				client.Send(mail);
				MessageBoxDialog m = new MessageBoxDialog("Message Sent", "Export", null, null);
				m.MessageImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Common/info32.png"));
				m.Owner = this;
				m.ShowDialog();
			} catch (Exception ex) {
				MessageBoxDialog m = new MessageBoxDialog(ex.Message, ex.ToString(), null, null);
				m.MessageImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Common/error32.png"));
				m.Owner = this;
				m.ShowDialog();
			}
		}

		#endregion

		#region "Export FTP"

		private string ExportFTPFileName = null;
		private System.ComponentModel.BackgroundWorker withEventsField_UploadWorker = new System.ComponentModel.BackgroundWorker();
		private System.ComponentModel.BackgroundWorker UploadWorker {
			get { return withEventsField_UploadWorker; }
			set {
				if (withEventsField_UploadWorker != null) {
					withEventsField_UploadWorker.DoWork -= UploadWorker_DoWork;
					withEventsField_UploadWorker.RunWorkerCompleted -= UploadWorker_RunWorkerCompleted;
				}
				withEventsField_UploadWorker = value;
				if (withEventsField_UploadWorker != null) {
					withEventsField_UploadWorker.DoWork += UploadWorker_DoWork;
					withEventsField_UploadWorker.RunWorkerCompleted += UploadWorker_RunWorkerCompleted;
				}
			}
		}
		private void UploadWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
		{
			try {
				Collection info = e.Argument as Collection;
				Utilities.FTP.FTPclient ftp = new Utilities.FTP.FTPclient(info[1] as string, info[2] as string, info[3] as string);
				System.IO.FileInfo fileinfo = new System.IO.FileInfo(ExportFTPFileName);
				ftp.Upload(ExportFTPFileName, "/" + fileinfo.Name);
				MessageBoxDialog m = new MessageBoxDialog("Document Uploaded", "Uploaded", null, null);
				m.MessageImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Common/info32.png"));
				m.Owner = this;
				m.ShowDialog();
				e.Result = true;
			} catch (Exception ex) {
				MessageBoxDialog m = new MessageBoxDialog(ex.Message, ex.ToString(), null, null);
				m.MessageImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Common/error32.png"));
				m.Owner = this;
				m.ShowDialog();
				e.Result = false;
			}
		}

		private void UploadWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
		{
			ExportFTPUploadPanel.Visibility = System.Windows.Visibility.Collapsed;
		}

		private void ExportFTPHostBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			try {
				if (ExportFTPHostBox.Text.Length > 0 && ExportFTPUsernameBox.Text.Length > 0 && ExportFTPPasswordBox.Text.Length > 0) {
					ExportFTPExportButton.IsEnabled = true;
				} else {
					ExportFTPExportButton.IsEnabled = false;
				}
			} catch (Exception ex) {
			}
		}

		private void ExportFTPExportButton_Click(object sender, RoutedEventArgs e)
		{
			try {
				ExportFTPUploadPanel.Visibility = System.Windows.Visibility.Visible;
				if (SelectedDocument.Editor.DocumentName != null) {
					ExportFTPFileName = SelectedDocument.Editor.DocumentName;
				} else {
					if (!Document.Editor.My.Computer.FileSystem.DirectoryExists(Document.Editor.My.Application.Info.DirectoryPath + "\\Temp")) {
						Document.Editor.My.Computer.FileSystem.CreateDirectory(Document.Editor.My.Application.Info.DirectoryPath + "\\Temp");
					}
					System.IO.FileInfo file = new System.IO.FileInfo(Document.Editor.My.Application.Info.DirectoryPath + "\\Temp\\document.xaml");
					System.IO.FileStream fs = System.IO.File.Open(file.FullName, System.IO.FileMode.OpenOrCreate);
					TextRange tr = new TextRange(SelectedDocument.Editor.Document.ContentStart, SelectedDocument.Editor.Document.ContentEnd);
					tr.Save(fs, System.Windows.DataFormats.Xaml);
					fs.Close();
					ExportFTPFileName = file.FullName;
				}
				if (Document.Editor.My.Computer.Network.IsAvailable) {
					Collection pram = new Collection();
					pram.Add(ExportFTPHostBox.Text);
					pram.Add(ExportFTPUsernameBox.Text);
					pram.Add(ExportFTPPasswordBox.Text);
					UploadWorker.RunWorkerAsync(pram);
					BackStageMenu.IsOpen = true;
				} else {
					MessageBoxDialog m = new MessageBoxDialog("Internet not found!", "Error", null, null);
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

		#endregion

		#region "Export Archive"

		private void ExportArchiveMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Microsoft.Win32.SaveFileDialog export = new Microsoft.Win32.SaveFileDialog();
			export.Title = "Export Archive";
			export.Filter = "Zip Arcive(*.zip)|*.zip|All Files(*.*)|*.*";
			if (export.ShowDialog() == true) {
				string n = null;
				if (SelectedDocument.DocName == null) {
					n = "document.xaml";
				} else {
					n = "document.xaml";
				}
				System.IO.FileInfo file = new System.IO.FileInfo(Document.Editor.My.Application.Info.DirectoryPath + "\\" + n);
				System.IO.FileStream fs = System.IO.File.Open(file.FullName, System.IO.FileMode.OpenOrCreate);
				TextRange tr = new TextRange(SelectedDocument.Editor.Document.ContentStart, SelectedDocument.Editor.Document.ContentEnd);
				tr.Save(fs, System.Windows.DataFormats.Xaml);
				fs.Close();
				Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile(export.FileName + ".zip");
				zip.AddFile(file.FullName, "");
				zip.Save();
				Document.Editor.My.Computer.FileSystem.DeleteFile(file.FullName);
			}
		}

		#endregion

		#region "Export Image"

		private void ExportImageMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			UpdateDocumentPreview();
			Microsoft.Win32.SaveFileDialog save = new Microsoft.Win32.SaveFileDialog();
			save.Title = "Export Image";
			save.Filter = "PNG Image(*.png)|*.png|All Files(*.*)|*.*";
			save.AddExtension = true;
			if (save.ShowDialog()) {
				DocPreviewScrollViewer.Visibility = System.Windows.Visibility.Visible;
				Image image = new Image();
				Visual v = DocumentPreview as Visual;
				RenderTargetBitmap bmp = new RenderTargetBitmap(Convert.ToInt32(DocumentPreview.Width), Convert.ToInt32(DocumentPreview.Height), 96, 96, PixelFormats.Pbgra32);
				bmp.Render(v);
				image.Source = bmp;
				using (fileStream == new System.IO.FileStream(save.FileName, System.IO.FileMode.Create)) {
					BitmapEncoder encoder = new PngBitmapEncoder();
					encoder.Frames.Add(BitmapFrame.Create(image.Source));
					encoder.Save(fileStream);
				}
				DocPreviewScrollViewer.Visibility = System.Windows.Visibility.Collapsed;
			}
		}

		#endregion

		#region "Export Sound"

		private void ExportSoundMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Speech.Rate = Document.Editor.My.Settings.Options_TTSS;
			Microsoft.Win32.SaveFileDialog save = new Microsoft.Win32.SaveFileDialog();
			save.Title = "Export Sound";
			save.Filter = "Wave Sound(*.wav;*.wave)|*.wav;*.wave|All Files(*.*)|*.*";
			if (save.ShowDialog() == true) {
				Speech.SetOutputToWaveFile(save.FileName);
				Speech.Speak(SelectedDocument.Editor.Selection.Text);
				Speech.SetOutputToDefaultAudioDevice();
				MessageBoxDialog m = new MessageBoxDialog("Done", "Success", null, null);
				m.MessageImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Common/info32.png"));
				m.Owner = this;
				m.ShowDialog();
			}
		}

		#endregion

		#endregion

		#region "Properties"

		private void ReadOnlyMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (SelectedDocument.Editor.IsReadOnly) {
				if (SelectedDocument.Editor.DocumentName != null) {
					System.IO.FileInfo f = new System.IO.FileInfo(SelectedDocument.Editor.DocumentName);
					f.IsReadOnly = false;
				}
				SelectedDocument.Editor.IsReadOnly = false;
			} else {
				if (SelectedDocument.Editor.DocumentName != null) {
					System.IO.FileInfo f = new System.IO.FileInfo(SelectedDocument.Editor.DocumentName);
					f.IsReadOnly = true;
				}
				SelectedDocument.Editor.IsReadOnly = true;
			}
		}

		#endregion

		#region "Print"

		private void PrintMenuItem_Click(System.Object sender, System.Windows.RoutedEventArgs e)
		{
			BackStageMenu.IsOpen = true;
			UpdateDocumentPreview();
			try {
				DocPreviewScrollViewer.Visibility = System.Windows.Visibility.Visible;
				PrintDialog pd = new PrintDialog();
				pd.PrintVisual(DocumentPreview, "test");
				DocPreviewScrollViewer.Visibility = System.Windows.Visibility.Collapsed;
			} catch (Exception ex) {
				MessageBoxDialog m = new MessageBoxDialog("No printers found!", "Warning!", null, null);
				m.MessageImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Common/info32.png"));
				m.Owner = this;
				m.ShowDialog();
				DocPreviewScrollViewer.Visibility = System.Windows.Visibility.Collapsed;
			}
		}

		private void PageSetupMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			BackStageMenu.IsOpen = true;
			UpdateDocumentPreview();
			PrintDialog pd = new PrintDialog();
			if (pd.ShowDialog() == true) {
				DocPreviewScrollViewer.Visibility = System.Windows.Visibility.Visible;
				pd.PrintVisual(DocumentPreview, SelectedDocument.DocName);
				DocPreviewScrollViewer.Visibility = System.Windows.Visibility.Collapsed;
			}
		}

		private void PrintPreview_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			PrintPreview.FitToMaxPagesAcross(1);
			PrintPreview.FitToMaxPagesAcrossCommand.Execute("1", PrintPreview);
		}

		#endregion

		private void ExitMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Close();
		}

		#endregion

		#region "-Edit"

		#region "Undo/Redo/Cut/Copy/Paste/Delete/Select All"

		private void UndoMenuItem_Click(System.Object sender, System.Windows.RoutedEventArgs e)
		{
			SelectedDocument.Editor.Focus();
			SelectedDocument.Editor.Undo();
		}

		private void RedoMenuItem_Click(System.Object sender, System.Windows.RoutedEventArgs e)
		{
			SelectedDocument.Editor.Focus();
			SelectedDocument.Editor.Redo();
		}

		private void CutMenuItem_Click(System.Object sender, System.Windows.RoutedEventArgs e)
		{
			SelectedDocument.Editor.Focus();
			SelectedDocument.Editor.Cut();
		}

		private void CutParagraphMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			try {
				if (SelectedDocument.Editor.Selection.IsEmpty) {
					TextRange TRange = new TextRange(SelectedDocument.Editor.CaretPosition.Paragraph.ElementStart, SelectedDocument.Editor.CaretPosition.Paragraph.ElementEnd);
					if (!TRange.IsEmpty) {
						SelectedDocument.Editor.Selection.Select(TRange.Start, TRange.End);
						SelectedDocument.Editor.Cut();
					}
				} else {
					EditingCommands.MoveToLineStart.Execute(null, SelectedDocument.Editor);
					TextRange TRange = new TextRange(SelectedDocument.Editor.CaretPosition.Paragraph.ElementStart, SelectedDocument.Editor.CaretPosition.Paragraph.ElementEnd);
					if (!TRange.IsEmpty) {
						SelectedDocument.Editor.Selection.Select(TRange.Start, TRange.End);
						SelectedDocument.Editor.Cut();
					}
				}
			} catch (Exception ex) {
				MessageBoxDialog m = new MessageBoxDialog(ex.Message, ex.ToString(), null, null);
				m.MessageImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Common/error32.png"));
				m.Owner = this;
				m.ShowDialog();
			}
			e.Handled = true;
		}

		private void CutLineMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			EditingCommands.MoveToLineStart.Execute(null, SelectedDocument.Editor);
			EditingCommands.SelectToLineEnd.Execute(null, SelectedDocument.Editor);
			SelectedDocument.Editor.Cut();
			e.Handled = true;
		}

		private void CopyMenuItem_Click(System.Object sender, System.Windows.RoutedEventArgs e)
		{
			SelectedDocument.Editor.Focus();
			SelectedDocument.Editor.Copy();
		}

		private void CopyParagraphMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			try {
				if (SelectedDocument.Editor.Selection.IsEmpty) {
					TextRange TRange = new TextRange(SelectedDocument.Editor.CaretPosition.Paragraph.ElementStart, SelectedDocument.Editor.CaretPosition.Paragraph.ElementEnd);
					if (!TRange.IsEmpty) {
						SelectedDocument.Editor.Selection.Select(TRange.Start, TRange.End);
						SelectedDocument.Editor.Copy();
					}
				} else {
					EditingCommands.MoveToLineStart.Execute(null, SelectedDocument.Editor);
					TextRange TRange = new TextRange(SelectedDocument.Editor.CaretPosition.Paragraph.ElementStart, SelectedDocument.Editor.CaretPosition.Paragraph.ElementEnd);
					if (!TRange.IsEmpty) {
						SelectedDocument.Editor.Selection.Select(TRange.Start, TRange.End);
						SelectedDocument.Editor.Copy();
					}
				}
			} catch (Exception ex) {
				MessageBoxDialog m = new MessageBoxDialog(ex.Message, ex.ToString(), null, null);
				m.MessageImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Common/error32.png"));
				m.Owner = this;
				m.ShowDialog();
			}
			e.Handled = true;
		}

		private void CopyLineMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			EditingCommands.MoveToLineStart.Execute(null, SelectedDocument.Editor);
			EditingCommands.SelectToLineEnd.Execute(null, SelectedDocument.Editor);
			SelectedDocument.Editor.Copy();
			e.Handled = true;
		}

		#region "Paste"

		private void PasteMenuItem_Click(System.Object sender, System.Windows.RoutedEventArgs e)
		{
			SelectedDocument.Editor.Focus();
			SelectedDocument.Editor.Paste();
		}

		private void PasteTextButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			SelectedDocument.Editor.CaretPosition.InsertTextInRun(Clipboard.GetText());
			e.Handled = true;
		}

		private void PasteImageMenuItem_Click(object sender, RoutedEventArgs e)
		{
			TextPointer t = SelectedDocument.Editor.CaretPosition;
			Image img = new Image();
			BitmapImage b = new BitmapImage();
			System.IO.MemoryStream ms = new System.IO.MemoryStream();
			// no using here! BitmapImage will dispose the stream after loading
			Document.Editor.My.Computer.Clipboard.GetImage.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
			b.BeginInit();
			b.CacheOption = BitmapCacheOption.OnLoad;
			b.StreamSource = ms;
			b.EndInit();
			img.Tag = new Thickness(0, 1, 1, 0);
			TransformGroup trans = new TransformGroup();
			trans.Children.Add(new RotateTransform(0));
			trans.Children.Add(new ScaleTransform(1, 1));
			img.LayoutTransform = trans;
			img.Stretch = Stretch.Fill;
			img.Height = b.Height;
			img.Width = b.Width;
			img.Source = b;
			InlineUIContainer inline = new InlineUIContainer(img);
			if (object.ReferenceEquals(t.Parent.GetType(), typeof(TableCell))) {
				TableCell cell = t.Parent;
				cell.Blocks.Add(new Paragraph(inline));
			} else {
				t.Paragraph.Inlines.Add(inline);
			}
			e.Handled = true;
		}

		#endregion

		private void DeleteMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			EditingCommands.Delete.Execute(null, SelectedDocument.Editor);
		}

		private void DeleteParagraphMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			try {
				if (SelectedDocument.Editor.Selection.IsEmpty) {
					TextRange TRange = new TextRange(SelectedDocument.Editor.CaretPosition.Paragraph.ElementStart, SelectedDocument.Editor.CaretPosition.Paragraph.ElementEnd);
					if (!TRange.IsEmpty) {
						SelectedDocument.Editor.Selection.Select(TRange.Start, TRange.End);
						EditingCommands.Delete.Execute(null, SelectedDocument.Editor);
					}
				} else {
					EditingCommands.MoveToLineStart.Execute(null, SelectedDocument.Editor);
					TextRange TRange = new TextRange(SelectedDocument.Editor.CaretPosition.Paragraph.ElementStart, SelectedDocument.Editor.CaretPosition.Paragraph.ElementEnd);
					if (!TRange.IsEmpty) {
						SelectedDocument.Editor.Selection.Select(TRange.Start, TRange.End);
						EditingCommands.Delete.Execute(null, SelectedDocument.Editor);
					}
				}
			} catch (Exception ex) {
				MessageBoxDialog m = new MessageBoxDialog(ex.Message, ex.ToString(), null, null);
				m.MessageImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Common/error32.png"));
				m.Owner = this;
				m.ShowDialog();
			}
			e.Handled = true;
		}

		private void DeleteLineMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			EditingCommands.MoveToLineStart.Execute(null, SelectedDocument.Editor);
			EditingCommands.SelectToLineEnd.Execute(null, SelectedDocument.Editor);
			EditingCommands.Delete.Execute(null, SelectedDocument.Editor);
			e.Handled = true;
		}

		private void SelectAllMenuItem_Click(System.Object sender, System.Windows.RoutedEventArgs e)
		{
			SelectedDocument.Editor.Focus();
			SelectedDocument.Editor.SelectAll();
		}

		private void SelectParagraphMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			try {
				if (SelectedDocument.Editor.Selection.IsEmpty) {
					TextRange TRange = new TextRange(SelectedDocument.Editor.CaretPosition.Paragraph.ElementStart, SelectedDocument.Editor.CaretPosition.Paragraph.ElementEnd);
					if (!TRange.IsEmpty) {
						SelectedDocument.Editor.Selection.Select(TRange.Start, TRange.End);
					}
				} else {
					EditingCommands.MoveToLineStart.Execute(null, SelectedDocument.Editor);
					TextRange TRange = new TextRange(SelectedDocument.Editor.CaretPosition.Paragraph.ElementStart, SelectedDocument.Editor.CaretPosition.Paragraph.ElementEnd);
					if (!TRange.IsEmpty) {
						SelectedDocument.Editor.Selection.Select(TRange.Start, TRange.End);
					}
				}
			} catch (Exception ex) {
				MessageBoxDialog m = new MessageBoxDialog(ex.Message, ex.ToString(), null, null);
				m.MessageImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Common/error32.png"));
				m.Owner = this;
				m.ShowDialog();
			}
			e.Handled = true;
		}

		private void SelectLineMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			EditingCommands.MoveToLineStart.Execute(null, SelectedDocument.Editor);
			EditingCommands.SelectToLineEnd.Execute(null, SelectedDocument.Editor);
			e.Handled = true;
		}

		#endregion

		#region "Find/Replace/Go To"

		private void FindButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			FindDialog findDialog = new FindDialog(SelectedDocument.Editor.Document);
			findDialog.Owner = this;
			findDialog.ShowDialog();
			if (findDialog.Res == "OK") {
				TextRange p = SelectedDocument.Editor.FindWordFromPosition(SelectedDocument.Editor.CaretPosition, findDialog.TextBox1.Text);
				try {
					SelectedDocument.Editor.Selection.Select(p.Start, p.End);
				} catch (Exception ex) {
					MessageBoxDialog m = new MessageBoxDialog("Word not found.", "Warning!", null, null);
					m.MessageImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Common/info32.png"));
					m.Owner = this;
					m.ShowDialog();
				}
			}
		}

		private void ReplaceButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			ReplaceDialog replaceDialog = new ReplaceDialog();
			replaceDialog.Owner = this;
			replaceDialog.ShowDialog();
			if (replaceDialog.Res == "OK") {
				TextRange p = SelectedDocument.Editor.FindWordFromPosition(SelectedDocument.Editor.Document.ContentStart.DocumentStart, replaceDialog.TextBox1.Text);
				try {
					SelectedDocument.Editor.Selection.Select(p.Start, p.End);
					SelectedDocument.Editor.Selection.Text = replaceDialog.TextBox2.Text;
				} catch (Exception ex) {
					MessageBoxDialog m = new MessageBoxDialog("Word not found.", "Warning!", null, null);
					m.MessageImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Common/info32.png"));
					m.Owner = this;
					m.ShowDialog();
				}
			}
		}

		private void GoToButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			GoToDialog gotodialog = new GoToDialog();
			gotodialog.Owner = this;
			gotodialog.ShowDialog();
			if (gotodialog.Res == "OK") {
				SelectedDocument.Editor.GoToLine(gotodialog.line);
			}
		}

		#endregion

		private void UppercaseMenuItem_Click(object sender, RoutedEventArgs e)
		{
			SelectedDocument.Editor.Selection.Text = SelectedDocument.Editor.Selection.Text.ToUpper();
		}

		private void LowercaseMenuItem_Click(object sender, RoutedEventArgs e)
		{
			SelectedDocument.Editor.Selection.Text = SelectedDocument.Editor.Selection.Text.ToLower();
		}

		#endregion

		#region "--ViewMenu"

		private void HRulerButton_Checked(object sender, System.Windows.RoutedEventArgs e)
		{
			foreach (DocumentTab t in TabCell.Children) {
				t.Ruler.Visibility = System.Windows.Visibility.Visible;
			}
			Document.Editor.My.Settings.MainWindow_ShowRuler = true;
			UpdateUI();
		}

		private void HRulerButton_Unchecked(object sender, System.Windows.RoutedEventArgs e)
		{
			foreach (DocumentTab t in TabCell.Children) {
				t.Ruler.Visibility = System.Windows.Visibility.Collapsed;
			}
			Document.Editor.My.Settings.MainWindow_ShowRuler = false;
			UpdateUI();
		}

		private void StatusbarButton_Checked(object sender, System.Windows.RoutedEventArgs e)
		{
			StatusBar.Visibility = System.Windows.Visibility.Visible;
			dockingManager.Margin = new Thickness(dockingManager.Margin.Left, dockingManager.Margin.Top, dockingManager.Margin.Right, 21);
		}

		private void StatusbarButton_Unchecked(object sender, System.Windows.RoutedEventArgs e)
		{
			StatusBar.Visibility = System.Windows.Visibility.Collapsed;
			dockingManager.Margin = new Thickness(dockingManager.Margin.Left, dockingManager.Margin.Top, dockingManager.Margin.Right, 0);
		}

		private void ZoomInButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			zoomSlider.Value += 10;
		}

		private void ZoomOutButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			zoomSlider.Value -= 10;
		}

		private void ResetZoomButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			zoomSlider.Value = 100;
		}

		private WindowState fullscreensetting = System.Windows.WindowState.Normal;
		private bool IsFullscreen = false;
		private void FullscreenMenuItem_Checked(object sender, RoutedEventArgs e)
		{
			fullscreensetting = WindowState;
			if (WindowState == System.Windows.WindowState.Maximized) {
				WindowState = System.Windows.WindowState.Normal;
			}
			WindowStyle = System.Windows.WindowStyle.None;
			Topmost = true;
			WindowState = System.Windows.WindowState.Maximized;
			IsFullscreen = true;
		}

		private void FullscreenMenuItem_Unchecked(object sender, RoutedEventArgs e)
		{
			Topmost = false;
			WindowStyle = System.Windows.WindowStyle.SingleBorderWindow;
			WindowState = System.Windows.WindowState.Normal;
			WindowState = fullscreensetting;
			IsFullscreen = false;
		}

		#endregion

		#region "Insert"

		#region "Tables"

		private void TableGrid_Click(int y, int x)
		{
			Table t = new Table();
			int @int = Convert.ToInt32(y);
			int int2 = Convert.ToInt32(x);
			while (!(@int == 0)) {
				TableRowGroup trg = new TableRowGroup();
				TableRow tr = new TableRow();
				while (!(int2 == 0)) {
					TableCell tc = new TableCell();
					tc.BorderBrush = Brushes.Black;
					tc.BorderThickness = new Thickness(1, 1, 1, 1);
					tr.Cells.Add(tc);
					int2 -= 1;
				}
				int2 = Convert.ToInt32(x);
				trg.Rows.Add(tr);
				t.RowGroups.Add(trg);
				@int -= 1;
			}
			t.BorderThickness = new Thickness(1, 1, 1, 1);
			if (SelectedDocument.Editor.CaretPosition.Paragraph != null) {
				TableCell tc = SelectedDocument.Editor.CaretPosition.Paragraph.Parent as TableCell;
				if (tc != null) {
					tc.Blocks.InsertBefore(SelectedDocument.Editor.CaretPosition.Paragraph, t);
				} else {
					ListItem listitem = SelectedDocument.Editor.CaretPosition.Paragraph.Parent as ListItem;
					if (listitem != null) {
						List list = listitem.Parent as List;
						if (list != null) {
							SelectedDocument.Editor.Document.Blocks.InsertAfter(list, t);
						}
					} else {
						SelectedDocument.Editor.Document.Blocks.InsertBefore(SelectedDocument.Editor.CaretPosition.Paragraph, t);
					}
				}
			} else {
				TableCell tc = SelectedDocument.Editor.CaretPosition.Parent as TableCell;
				if (tc != null) {
					tc.Blocks.Add(t);
				} else {
					TableRow tr = SelectedDocument.Editor.CaretPosition.Parent as TableRow;
					if (tr != null) {
						TableRowGroup trg = tr.Parent;
						Table table = trg.Parent;
						SelectedDocument.Editor.Document.Blocks.InsertAfter(table as Block, t);
					} else {
						FlowDocument flow = SelectedDocument.Editor.CaretPosition.Parent as FlowDocument;
						if (flow != null) {
							SelectedDocument.Editor.Document.Blocks.InsertBefore(SelectedDocument.Editor.Document.Blocks.FirstBlock, t);
						}
					}
				}
			}
			UpdateButtons();
		}

		private void TableMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			TableDialog tableDialog = new TableDialog();
			tableDialog.Owner = this;
			tableDialog.ShowDialog();
			if (tableDialog.Res == "OK") {
				Table t = new Table();
				int @int = Convert.ToInt32(tableDialog.RowsTextBox.Value);
				int int2 = Convert.ToInt32(tableDialog.CellsTextBox.Value);
				while (!(@int == 0)) {
					TableRowGroup trg = new TableRowGroup();
					TableRow tr = new TableRow();
					while (!(int2 == 0)) {
						TableCell tc = new TableCell();
						tc.Background = tableDialog.CellBackgroundColor;
						tc.BorderBrush = tableDialog.CellBorderColor;
						tc.BorderThickness = new Thickness(1, 1, 1, 1);
						tr.Cells.Add(tc);
						int2 -= 1;
					}
					int2 = Convert.ToInt32(tableDialog.CellsTextBox.Value);
					trg.Rows.Add(tr);
					t.RowGroups.Add(trg);
					@int -= 1;
				}
				t.Background = tableDialog.BackgroundColor;
				t.BorderBrush = tableDialog.BorderColor;
				t.BorderThickness = new Thickness(1, 1, 1, 1);
				if (SelectedDocument.Editor.CaretPosition.Paragraph != null) {
					TableCell tc = SelectedDocument.Editor.CaretPosition.Paragraph.Parent as TableCell;
					if (tc != null) {
						tc.Blocks.InsertBefore(SelectedDocument.Editor.CaretPosition.Paragraph, t);
					} else {
						ListItem listitem = SelectedDocument.Editor.CaretPosition.Paragraph.Parent as ListItem;
						if (listitem != null) {
							List list = listitem.Parent as List;
							if (list != null) {
								SelectedDocument.Editor.Document.Blocks.InsertAfter(list, t);
							}
						} else {
							SelectedDocument.Editor.Document.Blocks.InsertBefore(SelectedDocument.Editor.CaretPosition.Paragraph, t);
						}
					}
				} else {
					TableCell tc = SelectedDocument.Editor.CaretPosition.Parent as TableCell;
					if (tc != null) {
						tc.Blocks.Add(t);
					} else {
						TableRow tr = SelectedDocument.Editor.CaretPosition.Parent as TableRow;
						if (tr != null) {
							TableRowGroup trg = tr.Parent;
							Table table = trg.Parent;
							SelectedDocument.Editor.Document.Blocks.InsertAfter(table as Block, t);
						} else {
							FlowDocument flow = SelectedDocument.Editor.CaretPosition.Parent as FlowDocument;
							if (flow != null) {
								SelectedDocument.Editor.Document.Blocks.InsertBefore(SelectedDocument.Editor.Document.Blocks.FirstBlock, t);
							}
						}
					}
				}
				UpdateButtons();
			}
		}

		#endregion

		#region "Image"

		private void ImageButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Microsoft.Win32.OpenFileDialog open = new Microsoft.Win32.OpenFileDialog();
			open.Multiselect = true;
			open.Title = "Add Images";
			open.Filter = "Supported Images(*.bmp;*.gif;*.jpeg;*.jpg;*.png)|*.bmp;*.gif;*.jpeg;*.jpg;*.png|Bitmap Images(*.bmp)|*.bmp|GIF Images(*.gif)|*.gif|JPEG Images(*.jpeg;*.jpg)|*.jpeg;*.jpg|PNG Images(*.png)|*.png|All Files(*.*)|*.*";
			if (open.ShowDialog() == true) {
				SelectedDocument.Editor.Focus();
				foreach (string i in open.FileNames) {
					TextPointer t = SelectedDocument.Editor.CaretPosition;
					Image img = new Image();
					BitmapImage b = new BitmapImage();
					b.BeginInit();
					b.UriSource = new Uri(i);
					b.EndInit();
					img.Tag = new Thickness(0, 1, 1, 0);
					TransformGroup trans = new TransformGroup();
					trans.Children.Add(new RotateTransform(0));
					trans.Children.Add(new ScaleTransform(1, 1));
					img.LayoutTransform = trans;
					img.Stretch = Stretch.Fill;
					img.Height = b.Height;
					img.Width = b.Width;
					img.Source = b;
					InlineUIContainer inline = new InlineUIContainer(img);
					if (SelectedDocument.Editor.CaretPosition.Paragraph != null) {
						t.Paragraph.Inlines.Add(inline);
					} else {
						TableCell tc = SelectedDocument.Editor.CaretPosition.Parent as TableCell;
						if (tc != null) {
							tc.Blocks.Add(new Paragraph(inline));
						} else {
							TableRow tr = SelectedDocument.Editor.CaretPosition.Parent as TableRow;
							if (tr != null) {
								TableRowGroup trg = tr.Parent;
								Table table = trg.Parent;
								SelectedDocument.Editor.Document.Blocks.InsertAfter(table as Block, new Paragraph(inline));
							} else {
								FlowDocument flow = SelectedDocument.Editor.CaretPosition.Parent as FlowDocument;
								if (flow != null) {
									SelectedDocument.Editor.Document.Blocks.InsertBefore(SelectedDocument.Editor.Document.Blocks.FirstBlock, new Paragraph(inline));
								}
							}
						}
					}
				}
				UpdateSelected();
			}
		}

		#endregion

		#region "Object"

		private void ObjectButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			ObjectDialog od = new ObjectDialog();
			od.Owner = this;
			od.ShowDialog();
			if (SelectedDocument.Editor.CaretPosition.Paragraph != null) {
				if (od.Res == "button") {
					TextPointer t = SelectedDocument.Editor.CaretPosition;
					Button b = new Button();
					InlineUIContainer i = new InlineUIContainer(b);
					b.Width = od.OW;
					b.Height = od.OH;
					b.Content = od.OT;
					t.Paragraph.Inlines.Add(i);
				} else if (od.Res == "radiobutton") {
					TextPointer t = SelectedDocument.Editor.CaretPosition;
					RadioButton b = new RadioButton();
					InlineUIContainer i = new InlineUIContainer(b);
					b.Width = od.OW;
					b.Height = od.OH;
					b.Content = od.OT;
					t.Paragraph.Inlines.Add(i);
				} else if (od.Res == "checkbox") {
					TextPointer t = SelectedDocument.Editor.CaretPosition;
					CheckBox b = new CheckBox();
					InlineUIContainer i = new InlineUIContainer(b);
					b.Width = od.OW;
					b.Height = od.OH;
					b.Content = od.OT;
					t.Paragraph.Inlines.Add(i);
				} else if (od.Res == "textblock") {
					TextPointer t = SelectedDocument.Editor.CaretPosition;
					TextBlock b = new TextBlock();
					InlineUIContainer i = new InlineUIContainer(b);
					b.Width = od.OW;
					b.Height = od.OH;
					b.Text = od.OT;
					t.Paragraph.Inlines.Add(i);
				}
			} else {
				TableCell tc = SelectedDocument.Editor.CaretPosition.Parent as TableCell;
				if (tc != null) {
					if (od.Res == "button") {
						TextPointer t = SelectedDocument.Editor.CaretPosition;
						Button b = new Button();
						InlineUIContainer i = new InlineUIContainer(b);
						b.Width = od.OW;
						b.Height = od.OH;
						b.Content = od.OT;
						tc.Blocks.Add(new Paragraph(i));
					} else if (od.Res == "radiobutton") {
						TextPointer t = SelectedDocument.Editor.CaretPosition;
						RadioButton b = new RadioButton();
						InlineUIContainer i = new InlineUIContainer(b);
						b.Width = od.OW;
						b.Height = od.OH;
						b.Content = od.OT;
						tc.Blocks.Add(new Paragraph(i));
					} else if (od.Res == "checkbox") {
						TextPointer t = SelectedDocument.Editor.CaretPosition;
						CheckBox b = new CheckBox();
						InlineUIContainer i = new InlineUIContainer(b);
						b.Width = od.OW;
						b.Height = od.OH;
						b.Content = od.OT;
						tc.Blocks.Add(new Paragraph(i));
					} else if (od.Res == "textblock") {
						TextPointer t = SelectedDocument.Editor.CaretPosition;
						TextBlock b = new TextBlock();
						InlineUIContainer i = new InlineUIContainer(b);
						b.Width = od.OW;
						b.Height = od.OH;
						b.Text = od.OT;
						tc.Blocks.Add(new Paragraph(i));
					}
				} else {
					TableRow tr = SelectedDocument.Editor.CaretPosition.Parent as TableRow;
					if (tr != null) {
						TableRowGroup trg = tr.Parent;
						Table table = trg.Parent;
						if (od.Res == "button") {
							TextPointer t = SelectedDocument.Editor.CaretPosition;
							Button b = new Button();
							InlineUIContainer i = new InlineUIContainer(b);
							b.Width = od.OW;
							b.Height = od.OH;
							b.Content = od.OT;
							SelectedDocument.Editor.Document.Blocks.InsertAfter(table as Block, new Paragraph(i));
						} else if (od.Res == "radiobutton") {
							TextPointer t = SelectedDocument.Editor.CaretPosition;
							RadioButton b = new RadioButton();
							InlineUIContainer i = new InlineUIContainer(b);
							b.Width = od.OW;
							b.Height = od.OH;
							b.Content = od.OT;
							SelectedDocument.Editor.Document.Blocks.InsertAfter(table as Block, new Paragraph(i));
						} else if (od.Res == "checkbox") {
							TextPointer t = SelectedDocument.Editor.CaretPosition;
							CheckBox b = new CheckBox();
							InlineUIContainer i = new InlineUIContainer(b);
							b.Width = od.OW;
							b.Height = od.OH;
							b.Content = od.OT;
							SelectedDocument.Editor.Document.Blocks.InsertAfter(table as Block, new Paragraph(i));
						} else if (od.Res == "textblock") {
							TextPointer t = SelectedDocument.Editor.CaretPosition;
							TextBlock b = new TextBlock();
							InlineUIContainer i = new InlineUIContainer(b);
							b.Width = od.OW;
							b.Height = od.OH;
							b.Text = od.OT;
							SelectedDocument.Editor.Document.Blocks.InsertAfter(table as Block, new Paragraph(i));
						}
					} else {
						FlowDocument fd = SelectedDocument.Editor.CaretPosition.Parent as FlowDocument;
						if (fd != null) {
							if (od.Res == "button") {
								TextPointer t = SelectedDocument.Editor.CaretPosition;
								Button b = new Button();
								InlineUIContainer i = new InlineUIContainer(b);
								b.Width = od.OW;
								b.Height = od.OH;
								b.Content = od.OT;
								SelectedDocument.Editor.Document.Blocks.InsertBefore(SelectedDocument.Editor.Document.Blocks.FirstBlock, new Paragraph(i));
							} else if (od.Res == "radiobutton") {
								TextPointer t = SelectedDocument.Editor.CaretPosition;
								RadioButton b = new RadioButton();
								InlineUIContainer i = new InlineUIContainer(b);
								b.Width = od.OW;
								b.Height = od.OH;
								b.Content = od.OT;
								SelectedDocument.Editor.Document.Blocks.InsertBefore(SelectedDocument.Editor.Document.Blocks.FirstBlock, new Paragraph(i));
							} else if (od.Res == "checkbox") {
								TextPointer t = SelectedDocument.Editor.CaretPosition;
								CheckBox b = new CheckBox();
								InlineUIContainer i = new InlineUIContainer(b);
								b.Width = od.OW;
								b.Height = od.OH;
								b.Content = od.OT;
								SelectedDocument.Editor.Document.Blocks.InsertBefore(SelectedDocument.Editor.Document.Blocks.FirstBlock, new Paragraph(i));
							} else if (od.Res == "textblock") {
								TextPointer t = SelectedDocument.Editor.CaretPosition;
								TextBlock b = new TextBlock();
								InlineUIContainer i = new InlineUIContainer(b);
								b.Width = od.OW;
								b.Height = od.OH;
								b.Text = od.OT;
								SelectedDocument.Editor.Document.Blocks.InsertBefore(SelectedDocument.Editor.Document.Blocks.FirstBlock, new Paragraph(i));
							}
						}
					}
				}
			}
			UpdateButtons();
		}

		#endregion

		#region "Shape"

		private void ShapeButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			ShapeDialog sd = new ShapeDialog();
			sd.Owner = this;
			sd.ShowDialog();
			if (sd.Res == "OK") {
				Shape s = null;
				if (sd.TypeComboBox.SelectedIndex == 0) {
					s = new System.Windows.Shapes.Ellipse();
				} else if (sd.TypeComboBox.SelectedIndex == 1) {
					s = new System.Windows.Shapes.Rectangle();
				}
				s.Height = sd.Shape.RenderSize.Height;
				s.Width = sd.Shape.RenderSize.Width;
				s.Stroke = sd.Shape.Stroke;
				s.StrokeThickness = sd.Shape.StrokeThickness;
				InlineUIContainer i = new InlineUIContainer();
				i.Child = s;
				SelectedDocument.Editor.CaretPosition.Paragraph.Inlines.Add(i);
				UpdateButtons();
			}
		}

		#endregion

		#region "Chart"

		private void ChartButton_Click(object sender, RoutedEventArgs e)
		{
			ChartDialog d = new ChartDialog();
			d.Owner = this;
			d.ShowDialog();
			if (d.Res == "OK") {
				RenderTargetBitmap render = new RenderTargetBitmap(d.ChartWidth.Value, d.ChartHight.Value, 96, 96, PixelFormats.Default);
				render.Render(d.PreviewChart);
				BitmapSource bsource = render;
				Image img = new Image();
				img.Source = bsource;
				img.Height = d.ChartHight.Value;
				img.Width = d.ChartWidth.Value;
				InlineUIContainer i = new InlineUIContainer();
				i.Child = img;
				SelectedDocument.Editor.CaretPosition.Paragraph.Inlines.Add(i);
				UpdateButtons();
			}
		}

		#endregion

		#region "Link"

		private void LinkMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			LinkDialog l = new LinkDialog();
			l.Owner = this;
			l.ShowDialog();
			if (!(l.Link == null)) {
				try {
					SelectedDocument.Editor.Focus();
					Hyperlink li = new Hyperlink(SelectedDocument.Editor.CaretPosition.DocumentStart, SelectedDocument.Editor.CaretPosition.DocumentEnd);
					Uri u = new Uri(l.Link);
					li.NavigateUri = u;
					UpdateButtons();
				} catch (Exception ex) {
					MessageBoxDialog m = new MessageBoxDialog(ex.Message, ex.ToString(), null, null);
					m.MessageImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Common/error32.png"));
					m.Owner = this;
					m.ShowDialog();
				}
			}
		}

		#endregion

		#region "FlowDocument"

		private void InsertFlowDocumentButton_Click(object sender, RoutedEventArgs e)
		{
			Microsoft.Win32.OpenFileDialog open = new Microsoft.Win32.OpenFileDialog();
			open.Title = "Insert FlowDocument";
			open.Filter = "FlowDocument(*.xaml)|*.xaml";
			if (open.ShowDialog()) {
				System.IO.FileStream fs = System.IO.File.Open(open.FileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
				FlowDocument content = System.Windows.Markup.XamlReader.Load(fs) as FlowDocument;
				fs.Close();
				fs = null;
				foreach (Block b in content.Blocks) {
					string xaml = System.Windows.Markup.XamlWriter.Save(b);
					Block newblock = System.Windows.Markup.XamlReader.Load(new System.Xml.XmlTextReader(new System.IO.StringReader(xaml))) as Block;
					SelectedDocument.Editor.Document.Blocks.InsertBefore(SelectedDocument.Editor.CaretPosition.Paragraph, newblock);
				}
			}
		}

		#endregion

		#region "Rich Text Document"

		private void InsertRichTextDocumentButton_Click(object sender, RoutedEventArgs e)
		{
			Microsoft.Win32.OpenFileDialog open = new Microsoft.Win32.OpenFileDialog();
			open.Title = "Insert Rich Text Document";
			open.Filter = "Rich Text Document(*.rtf)|*.rtf";
			if (open.ShowDialog()) {
				System.IO.FileStream st = new System.IO.FileStream(open.FileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
				SelectedDocument.Editor.Selection.Load(st, DataFormats.Rtf);
				st.Close();
			}
		}

		#endregion

		#region "Text File"

		private void TextFileButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Microsoft.Win32.OpenFileDialog o = new Microsoft.Win32.OpenFileDialog();
			o.Title = "Insert Text File";
			o.Filter = "Text Document(*.txt)|*.txt|All Files(*.*)|*.*";
			if (o.ShowDialog()) {
				SelectedDocument.Editor.CaretPosition.InsertTextInRun(Document.Editor.My.Computer.FileSystem.ReadAllText(o.FileName));
				UpdateButtons();
			}
		}

		#endregion

		private void EmoticonGrid_Click(BitmapSource img)
		{
			SelectedDocument.Editor.Focus();
			TextPointer t = SelectedDocument.Editor.CaretPosition;
			Image image = new Image();
			image.Tag = new Thickness(0, 1, 1, 0);
			TransformGroup trans = new TransformGroup();
			trans.Children.Add(new RotateTransform(0));
			trans.Children.Add(new ScaleTransform(1, 1));
			image.LayoutTransform = trans;
			image.Stretch = Stretch.Fill;
			image.Height = img.Height;
			image.Width = img.Width;
			image.Source = img;
			InlineUIContainer inline = new InlineUIContainer(image);
			if (SelectedDocument.Editor.CaretPosition.Paragraph != null) {
				t.Paragraph.Inlines.Add(inline);
			} else {
				TableCell tc = SelectedDocument.Editor.CaretPosition.Parent as TableCell;
				if (tc != null) {
					tc.Blocks.Add(new Paragraph(inline));
				} else {
					TableRow tr = SelectedDocument.Editor.CaretPosition.Parent as TableRow;
					if (tr != null) {
						TableRowGroup trg = tr.Parent;
						Table table = trg.Parent;
						SelectedDocument.Editor.Document.Blocks.InsertAfter(table as Block, new Paragraph(inline));
					} else {
						FlowDocument flow = SelectedDocument.Editor.CaretPosition.Parent as FlowDocument;
						if (flow != null) {
							SelectedDocument.Editor.Document.Blocks.InsertBefore(SelectedDocument.Editor.Document.Blocks.FirstBlock, new Paragraph(inline));
						}
					}
				}
			}
			UpdateSelected();
		}

		#region "Symbol"

		private void InsertSymbolGallery_Click(string symbol)
		{
			SelectedDocument.Editor.CaretPosition.InsertTextInRun(symbol);
			UpdateButtons();
		}

		#endregion

		#region "Video"

		private void VideoButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
			dialog.Title = "Insert Video";
			dialog.Filter = "Supported Videos(*.avi;*.mpeg;*.mpg;*.wmv)|*.avi;*.mpeg;*.mpg;*.wmv|AVI Videos(*.avi)|*.avi|MPEG Videos(*.mpeg;*.mpg)|*.mpeg;*.mpg|WMV Videos(*.wmv)|*.wmv|All Files(*.*)|*.*";
			if (dialog.ShowDialog()) {
				try {
					MediaElement m = new MediaElement();
					m.Width = 320;
					m.Height = 240;
					m.Source = new Uri(dialog.FileName);
					m.LoadedBehavior = MediaState.Manual;
					InlineUIContainer i = new InlineUIContainer();
					i.Child = m;
					SelectedDocument.Editor.CaretPosition.Paragraph.Inlines.Add(i);
					UpdateSelected();
				} catch (Exception ex) {
					MessageBoxDialog m = new MessageBoxDialog(ex.Message, ex.ToString(), null, null);
					m.MessageImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Common/error32.png"));
					m.Owner = this;
					m.ShowDialog();
				}
			}
		}

		#endregion

		#region "Horizontal Line"

		private void HorizontalLineButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			InsertLineDialog d = new InsertLineDialog();
			d.Owner = this;
			d.ShowDialog();
			if (d.Res == "OK") {
				Separator line = new Separator();
				InlineUIContainer inline = new InlineUIContainer();
				line.Background = Brushes.Gray;
				line.Width = d.h;
				line.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
				inline.Child = line;
				SelectedDocument.Editor.CaretPosition.Paragraph.Inlines.Add(inline);
				UpdateButtons();
			}
		}

		#endregion

		#region "Header/Footer"

		private void HeaderButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			SelectedDocument.Editor.Document.Blocks.FirstBlock.ContentStart.InsertParagraphBreak();
			SelectedDocument.Editor.Document.Blocks.FirstBlock.ContentStart.InsertTextInRun("Header");
			UpdateButtons();
		}

		private void FooterButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			SelectedDocument.Editor.Document.ContentEnd.InsertParagraphBreak();
			SelectedDocument.Editor.Document.ContentEnd.InsertTextInRun("Footer");
			UpdateButtons();
		}

		#endregion

		#region "Date/Time"

		private void DateMenuItem_Click(System.Object sender, System.Windows.RoutedEventArgs e)
		{
			SelectedDocument.Editor.CaretPosition.InsertTextInRun(System.DateTime.Now.ToString("M/dd/yyyy"));
			UpdateButtons();
		}

		private void MoreDateMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			DateDialog d = new DateDialog();
			d.Owner = this;
			d.ShowDialog();
			if (d.Res == "OK") {
				SelectedDocument.Editor.CaretPosition.InsertTextInRun(d.ListBox1.SelectedItem);
				UpdateButtons();
			}
			e.Handled = true;
		}

		private void TimeMenuItem_Click(System.Object sender, System.Windows.RoutedEventArgs e)
		{
			SelectedDocument.Editor.CaretPosition.InsertTextInRun(System.DateTime.Now.ToString("h:mm tt"));
			UpdateButtons();
		}

		private void MoreTimeButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			TimeDialog d = new TimeDialog();
			d.Owner = this;
			d.ShowDialog();
			if (d.Res == "OK") {
				if (d.RadioButton12.IsChecked) {
					if (d.AMPMCheckBox.IsChecked) {
						if (d.SecCheckBox.IsChecked) {
							SelectedDocument.Editor.CaretPosition.InsertTextInRun(System.DateTime.Now.ToString("h:mm:ss tt"));
						} else {
							SelectedDocument.Editor.CaretPosition.InsertTextInRun(System.DateTime.Now.ToString("h:mm tt"));
						}
					} else {
						if (d.SecCheckBox.IsChecked) {
							SelectedDocument.Editor.CaretPosition.InsertTextInRun(System.DateTime.Now.ToString("h:mm:ss"));
						} else {
							SelectedDocument.Editor.CaretPosition.InsertTextInRun(System.DateTime.Now.ToString("h:mm"));
						}
					}
				} else {
					if (d.SecCheckBox.IsChecked) {
						SelectedDocument.Editor.CaretPosition.InsertTextInRun(System.DateTime.Now.ToString("HH:mm:ss"));
					} else {
						SelectedDocument.Editor.CaretPosition.InsertTextInRun(System.DateTime.Now.ToString("HH:mm"));
					}
				}
				UpdateButtons();
			}
			e.Handled = true;
		}

		#endregion

		#endregion

		#region "-Format"

		private void ClearFormattingButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			SelectedDocument.Editor.Selection.ClearAllProperties();
			UpdateSelected();
		}

		#region "Font/Font Size/Font Color/Hightlight Color"

		private bool UpdatingFont = false;
		private void FontComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (FontComboBox.IsLoaded && !UpdatingFont) {
				SelectedDocument.Editor.Selection.ApplyPropertyValue(TextElement.FontFamilyProperty, FontComboBox.SelectedItem);
				SelectedDocument.Editor.Focus();
				UpdateSelected();
			}
		}

		private void FontSizeComboBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter) {
				SelectedDocument.Editor.Focus();
			}
		}

		private void FontSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (FontSizeComboBox.IsLoaded && !UpdatingFont) {
				try {
					double val = Convert.ToDouble(FontSizeComboBox.SelectedValue);
					SelectedDocument.Editor.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, val);
					UpdateSelected();
				} catch (Exception ex) {
				}
			}
		}

		private void FontColorGallery_SelectedColorChanged(object sender, System.Windows.RoutedEventArgs e)
		{
			if (FontColorGallery.SelectedColor != null) {
				SolidColorBrush c = new SolidColorBrush();
				c.Color = FontColorGallery.SelectedColor;
				SelectedDocument.Editor.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, c);
				FontColorGallery.SelectedColor = null;
			}
		}

		private void HighlightColorGallery_SelectedColorChanged(object sender, System.Windows.RoutedEventArgs e)
		{
			if (HighlightColorGallery.SelectedColor != null) {
				SolidColorBrush c = new SolidColorBrush();
				c.Color = HighlightColorGallery.SelectedColor;
				SelectedDocument.Editor.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, c);
				HighlightColorGallery.SelectedColor = null;
			}
		}

		#endregion

		#region "Blod/Italic/Underline/Strikethrough"

		private void BoldMenuItem_Click(System.Object sender, System.Windows.RoutedEventArgs e)
		{
			EditingCommands.ToggleBold.Execute(null, SelectedDocument.Editor);
			UpdateSelected();
		}

		private void ItalicMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			EditingCommands.ToggleItalic.Execute(null, SelectedDocument.Editor);
			UpdateSelected();
		}

		private void UnderlineMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			EditingCommands.ToggleUnderline.Execute(null, SelectedDocument.Editor);
			UpdateSelected();
		}

		private void StrikethroughButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			SelectedDocument.Editor.ToggleStrikethrough();
			UpdateSelected();
		}

		#endregion

		#region "Subscript/Superscript"

		private void SubscriptButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			SelectedDocument.Editor.ToggleSubscript();
			UpdateSelected();
		}

		private void SuperscriptButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			SelectedDocument.Editor.ToggleSuperscript();
			UpdateSelected();
		}

		#endregion

		#region "Indent More/Indent Less"

		private void IndentMoreButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			EditingCommands.IncreaseIndentation.Execute(null, SelectedDocument.Editor);
		}

		private void IndentLessButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			EditingCommands.DecreaseIndentation.Execute(null, SelectedDocument.Editor);
		}

		#endregion

		#region "Bullet List/Number List"

		private void SetListStyle(TextMarkerStyle s)
		{
			Run run = SelectedDocument.Editor.CaretPosition.Parent as Run;
			if (run != null) {
				ListItem li = run.Parent as ListItem;
				if (li != null) {
					MessageBox.Show(li.Parent.ToString());
				} else {
					Paragraph p = run.Parent as Paragraph;
					if (p != null) {
						ListItem listitem = p.Parent as ListItem;
						if (listitem != null) {
							List list2 = listitem.Parent as List;
							if (list2 != null) {
								list2.MarkerStyle = s;
							}
						} else {
							EditingCommands.ToggleBullets.Execute(null, SelectedDocument.Editor);
							Paragraph p2 = SelectedDocument.Editor.CaretPosition.Parent as Paragraph;
							if (p2 != null) {
								ListItem listitem2 = p2.Parent as ListItem;
								if (listitem2 != null) {
									List list2 = listitem2.Parent as List;
									if (list2 != null) {
										list2.MarkerStyle = s;
									}
								}
							}
						}
					}
				}
			} else {
				Paragraph p = SelectedDocument.Editor.CaretPosition.Parent as Paragraph;
				if (p != null) {
					ListItem listitem = p.Parent as ListItem;
					if (listitem != null) {
						List list1 = listitem.Parent as List;
						if (list1 != null) {
							list1.MarkerStyle = s;
						}
					} else {
						EditingCommands.ToggleBullets.Execute(null, SelectedDocument.Editor);
						Paragraph p2 = SelectedDocument.Editor.CaretPosition.Parent as Paragraph;
						if (p2 != null) {
							ListItem listitem2 = p2.Parent as ListItem;
							if (listitem2 != null) {
								List list2 = listitem2.Parent as List;
								if (list2 != null) {
									list2.MarkerStyle = s;
								}
							}
						}
					}
				} else {
					TableCell tablecell = SelectedDocument.Editor.CaretPosition.Parent as TableCell;
					if (tablecell != null) {
						Paragraph par = new Paragraph();
						tablecell.Blocks.Add(par);
						EditingCommands.ToggleBullets.Execute(null, SelectedDocument.Editor);
						Paragraph p2 = SelectedDocument.Editor.CaretPosition.Parent as Paragraph;
						if (p2 != null) {
							ListItem listitem2 = p2.Parent as ListItem;
							if (listitem2 != null) {
								List list2 = listitem2.Parent as List;
								if (list2 != null) {
									list2.MarkerStyle = s;
								}
							}
						}
					}
				}
			}
			UpdateSelected();
		}

		private void BulletListMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			EditingCommands.ToggleBullets.Execute(null, SelectedDocument.Editor);
			UpdateSelected();
		}

		private void DiskBulletButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			SetListStyle(TextMarkerStyle.Disc);
		}

		private void CircleBulletButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			SetListStyle(TextMarkerStyle.Circle);
		}

		private void BoxBulletButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			SetListStyle(TextMarkerStyle.Box);
		}

		private void SquareBulletButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			SetListStyle(TextMarkerStyle.Square);
		}

		private void NumberListMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			EditingCommands.ToggleNumbering.Execute(null, SelectedDocument.Editor);
			UpdateSelected();
		}

		private void DecimalListButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			SetListStyle(TextMarkerStyle.Decimal);
		}

		private void UpperLatinListButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			SetListStyle(TextMarkerStyle.UpperLatin);
		}

		private void LowerLatinListButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			SetListStyle(TextMarkerStyle.LowerLatin);
		}

		private void UpperRomanListButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			SetListStyle(TextMarkerStyle.UpperRoman);
		}

		private void LowerRomanListButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			SetListStyle(TextMarkerStyle.LowerRoman);
		}

		#endregion

		#region "Align Left/Center/Right/Justify"

		private void AlignLeftMenuItem_Click(System.Object sender, System.Windows.RoutedEventArgs e)
		{
			EditingCommands.AlignLeft.Execute(null, SelectedDocument.Editor);
			UpdateSelected();
		}

		private void AlignCenterMenuItem_Click(System.Object sender, System.Windows.RoutedEventArgs e)
		{
			EditingCommands.AlignCenter.Execute(null, SelectedDocument.Editor);
			UpdateSelected();
		}

		private void AlignRightMenuItem_Click(System.Object sender, System.Windows.RoutedEventArgs e)
		{
			EditingCommands.AlignRight.Execute(null, SelectedDocument.Editor);
			UpdateSelected();
		}

		private void AlignJustifyMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			EditingCommands.AlignJustify.Execute(null, SelectedDocument.Editor);
			UpdateSelected();
		}

		#endregion

		#region "Line Spacing"

		private void LineSpacing1Point0_Click(object sender, RoutedEventArgs e)
		{
			if (SelectedDocument.Editor.CaretPosition.Paragraph != null) {
				SelectedDocument.Editor.CaretPosition.Paragraph.LineHeight = 1.0;
				UpdateSelected();
			}
		}

		private void LineSpacing1Point15_Click(object sender, RoutedEventArgs e)
		{
			if (SelectedDocument.Editor.CaretPosition.Paragraph != null) {
				SelectedDocument.Editor.CaretPosition.Paragraph.LineHeight = 1.15;
				UpdateSelected();
			}
		}

		private void LineSpacing1Point5_Click(object sender, RoutedEventArgs e)
		{
			if (SelectedDocument.Editor.CaretPosition.Paragraph != null) {
				SelectedDocument.Editor.CaretPosition.Paragraph.LineHeight = 1.5;
				UpdateSelected();
			}
		}

		private void LineSpacing2Point0_Click(object sender, RoutedEventArgs e)
		{
			if (SelectedDocument.Editor.CaretPosition.Paragraph != null) {
				SelectedDocument.Editor.CaretPosition.Paragraph.LineHeight = 2.0;
				UpdateSelected();
			}
		}

		private void LineSpacing2Point5_Click(object sender, RoutedEventArgs e)
		{
			if (SelectedDocument.Editor.CaretPosition.Paragraph != null) {
				SelectedDocument.Editor.CaretPosition.Paragraph.LineHeight = 2.5;
				UpdateSelected();
			}
		}

		private void LineSpacing3Point0_Click(object sender, RoutedEventArgs e)
		{
			if (SelectedDocument.Editor.CaretPosition.Paragraph != null) {
				SelectedDocument.Editor.CaretPosition.Paragraph.LineHeight = 3.0;
				UpdateSelected();
			}
		}

		private void CustomLineSpacingMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			LineSpacingDialog d = new LineSpacingDialog();
			if (SelectedDocument.Editor.CaretPosition.Paragraph != null) {
				d.number = SelectedDocument.Editor.CaretPosition.Paragraph.LineHeight;
			}
			d.Owner = this;
			d.ShowDialog();
			if (d.Res == "OK") {
				try {
					SelectedDocument.Editor.CaretPosition.Paragraph.LineHeight = d.number;
					UpdateSelected();
				} catch (Exception ex) {
					MessageBoxDialog m = new MessageBoxDialog(ex.Message, ex.ToString(), null, null);
					m.MessageImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Common/error32.png"));
					m.Owner = this;
					m.ShowDialog();
				}
			}
		}

		#endregion

		#region "ltr/rtl"

		private void LefttoRightButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			SelectedDocument.Editor.Document.FlowDirection = FlowDirection.LeftToRight;
		}

		private void RighttoLeftButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			SelectedDocument.Editor.Document.FlowDirection = FlowDirection.RightToLeft;
		}

		#endregion

		#endregion

		#region "--Page Layout"

		#region "Margins"

		private void NoneMarginsMenuItem_Click(object sender, RoutedEventArgs e)
		{
			SelectedDocument.Editor.SetPageMargins(new Thickness(0, 0, 0, 0));
			UpdateButtons();
		}

		private void NormalMarginsMenuItem_Click(object sender, RoutedEventArgs e)
		{
			SelectedDocument.Editor.SetPageMargins(new Thickness(96, 96, 96, 96));
			UpdateButtons();
		}

		private void NarrowMarginsMenuItem_Click(object sender, RoutedEventArgs e)
		{
			SelectedDocument.Editor.SetPageMargins(new Thickness(48, 48, 48, 48));
			UpdateButtons();
		}

		private void LeftMarginBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (IsLoaded) {
				SelectedDocument.Editor.SetPageMargins(new Thickness(LeftMarginBox.Value, SelectedDocument.Editor.docpadding.Top, SelectedDocument.Editor.docpadding.Right, SelectedDocument.Editor.docpadding.Bottom));
				UpdateButtons();
			}
		}

		private void TopMarginBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (IsLoaded) {
				SelectedDocument.Editor.SetPageMargins(new Thickness(SelectedDocument.Editor.docpadding.Left, TopMarginBox.Value, SelectedDocument.Editor.docpadding.Right, SelectedDocument.Editor.docpadding.Bottom));
				UpdateButtons();
			}
		}

		private void RightMarginBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (IsLoaded) {
				SelectedDocument.Editor.SetPageMargins(new Thickness(SelectedDocument.Editor.docpadding.Left, SelectedDocument.Editor.docpadding.Top, RightMarginBox.Value, SelectedDocument.Editor.docpadding.Bottom));
				UpdateButtons();
			}
		}

		private void BottomMarginBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (IsLoaded) {
				SelectedDocument.Editor.SetPageMargins(new Thickness(SelectedDocument.Editor.docpadding.Left, SelectedDocument.Editor.docpadding.Top, SelectedDocument.Editor.docpadding.Right, BottomMarginBox.Value));
				UpdateButtons();
			}
		}

		#endregion

		#region "Size"

		private void NormalPageSizeMenuItem_Click(object sender, RoutedEventArgs e)
		{
			SelectedDocument.SetPageSize(1056, 816);
			UpdateButtons();
		}

		private void WidePageSizeMenuItem_Click(object sender, RoutedEventArgs e)
		{
			SelectedDocument.SetPageSize(816, 1056);
			UpdateButtons();
		}

		private void PageHeightBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (IsLoaded) {
				SelectedDocument.SetPageSize(PageHeightBox.Value, SelectedDocument.Editor.Document.PageWidth);
				UpdateButtons();
			}
		}

		private void PageWidthBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (IsLoaded) {
				SelectedDocument.SetPageSize(SelectedDocument.Editor.Document.PageHeight, PageWidthBox.Value);
				UpdateButtons();
			}
		}

		#endregion

		private void BackgroundColorGallery_SelectedColorChanged(object sender, RoutedEventArgs e)
		{
			SolidColorBrush b = new SolidColorBrush(BackgroundColorGallery.SelectedColor);
			SelectedDocument.Editor.Document.Background = b;
		}

		#endregion

		#region "--NavigationMenuItem"

		private void LineDownButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			SelectedDocument.Editor.LineDown();
		}

		private void LineUpButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			SelectedDocument.Editor.LineUp();
		}

		private void LineLeftButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			SelectedDocument.Editor.LineLeft();
		}

		private void LineRightButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			SelectedDocument.Editor.LineRight();
		}

		private void PageDownButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			SelectedDocument.Editor.PageDown();
		}

		private void PageUpButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			SelectedDocument.Editor.PageUp();
		}

		private void PageLeftButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			SelectedDocument.Editor.PageLeft();
		}

		private void PageRightButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			SelectedDocument.Editor.PageRight();
		}

		private void StartButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			EditingCommands.MoveToDocumentStart.Execute(null, SelectedDocument.Editor);
		}

		private void EndButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			EditingCommands.MoveToDocumentEnd.Execute(null, SelectedDocument.Editor);
		}

		#endregion

		#region "--ToolsMenuItem"

		private void SpellCheckButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			SpellCheckDialog d = new SpellCheckDialog();
			TextPointer cp = SelectedDocument.Editor.Selection.Start.GetPositionAtOffset(1);
			d.Owner = this;
			SpellingError sp = SelectedDocument.Editor.GetSpellingError(cp);
			if (sp != null) {
				foreach (string i in sp.Suggestions) {
					d.WordListBox.Items.Add(i);
				}
				d.WordListBox.SelectedIndex = 0;
				d.WordListBox.Focus();
			}
			d.ShowDialog();
			if (d.Res == "OK") {
				sp.Correct(d.WordListBox.SelectedItem);
			}
		}

		private void PreviousSpellingErrorMenuItem_Click(object sender, RoutedEventArgs e)
		{
			try {
				TextPointer tp = SelectedDocument.Editor.GetNextSpellingErrorPosition(SelectedDocument.Editor.Selection.Start, LogicalDirection.Backward);
				TextRange tr = SelectedDocument.Editor.GetSpellingErrorRange(tp);
				SelectedDocument.Editor.Selection.Select(tr.Start, tr.End);
			} catch (Exception ex) {
			}
		}

		private void NextSpellingErrorMenuItem_Click(object sender, RoutedEventArgs e)
		{
			try {
				TextPointer tp = SelectedDocument.Editor.GetNextSpellingErrorPosition(SelectedDocument.Editor.Selection.End, LogicalDirection.Forward);
				TextRange tr = SelectedDocument.Editor.GetSpellingErrorRange(tp);
				SelectedDocument.Editor.Selection.Select(tr.Start, tr.End);
			} catch (Exception ex) {
			}
		}

		private void IgnoreSpellingErrorMenuItem_Click(object sender, RoutedEventArgs e)
		{
			TextPointer cp = SelectedDocument.Editor.Selection.Start.GetPositionAtOffset(1);
			SpellingError sp = SelectedDocument.Editor.GetSpellingError(cp);
			if (sp != null) {
				TextRange r = SelectedDocument.Editor.GetSpellingErrorRange(cp);
				if (!Document.Editor.My.Computer.FileSystem.FileExists(Document.Editor.My.Application.Info.DirectoryPath + "\\spellcheck_ignorelist.lex")) {
					System.IO.StreamWriter sr = System.IO.File.CreateText(Document.Editor.My.Application.Info.DirectoryPath + "\\spellcheck_ignorelist.lex");
					sr.Close();
				}
				System.IO.StreamReader fileIn = new System.IO.StreamReader(Document.Editor.My.Application.Info.DirectoryPath + "\\spellcheck_ignorelist.lex");
				string strData = "";
				long lngCount = 1;
				bool canadd = true;
				while ((!(fileIn.EndOfStream))) {
					strData = fileIn.ReadLine();
					if (object.ReferenceEquals(strData, r.Text)) {
						canadd = false;
					}
					lngCount = lngCount + 1;
				}
				fileIn.Close();
				if (canadd) {
					Document.Editor.My.Computer.FileSystem.WriteAllText(Document.Editor.My.Application.Info.DirectoryPath + "\\spellcheck_ignorelist.lex", Constants.vbNewLine, true);
					Document.Editor.My.Computer.FileSystem.WriteAllText(Document.Editor.My.Application.Info.DirectoryPath + "\\spellcheck_ignorelist.lex", r.Text, true);
				}
				sp.IgnoreAll();
			}
		}

		private void CorrectAllMenuItem_Click(object sender, RoutedEventArgs e)
		{
			while (true) {
				TextPointer sp = SelectedDocument.Editor.GetNextSpellingErrorPosition(SelectedDocument.Editor.Document.ContentStart.DocumentStart, LogicalDirection.Forward);
				if (sp == null) {
					break; // TODO: might not be correct. Was : Exit While
				}
				SpellingError se = SelectedDocument.Editor.GetSpellingError(SelectedDocument.Editor.GetNextSpellingErrorPosition(SelectedDocument.Editor.Document.ContentStart.DocumentStart, LogicalDirection.Forward));
				if (se != null) {
					foreach (string i in se.Suggestions) {
						se.Correct(i);
						break; // TODO: might not be correct. Was : Exit For
					}
				} else {
					break; // TODO: might not be correct. Was : Exit While
				}
			}
		}

		private void TextToSpeechButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Speech.Rate = Document.Editor.My.Settings.Options_TTSS;
			if (Speech.State == System.Speech.Synthesis.SynthesizerState.Speaking) {
				Speech.SpeakAsyncCancelAll();
			} else {
				try {
					Speech.SelectVoice(Speech.GetInstalledVoices()[Document.Editor.My.Settings.Options_TTSV].VoiceInfo.Name);
					Speech.SpeakAsync(SelectedDocument.Editor.Selection.Text.ToString());
				} catch (Exception ex) {
					MessageBoxDialog m = new MessageBoxDialog(ex.Message, ex.ToString(), null, null);
					m.MessageImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Common/error32.png"));
					m.Owner = this;
					m.ShowDialog();
				}
			}
		}

		private void TranslateButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			TranslateDialog trans = new TranslateDialog(SelectedDocument.Editor.Selection.Text.ToString());
			trans.Owner = this;
			trans.ShowDialog();
			if (trans.res == true) {
				SelectedDocument.Editor.Selection.Text = trans.TranslatedText.Content as string;
			}
		}

		private void DefinitionsButton_Click(System.Object sender, System.Windows.RoutedEventArgs e)
		{
			if (SelectedDocument.Editor.Selection.Text.Length > 0) {
				Process.Start("http://www.bing.com/Dictionary/search?q=define+" + SelectedDocument.Editor.Selection.Text);
			} else {
				MessageBoxDialog m = new MessageBoxDialog("Select a word first.", "Warning!", null, null);
				m.MessageImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Common/info32.png"));
				m.Owner = this;
				m.ShowDialog();
			}
		}

		private void OptionsMenuItem_Click(System.Object sender, System.Windows.RoutedEventArgs e)
		{
			OptionsDialog o = new OptionsDialog();
			o.Owner = this;
			if (o.ShowDialog() == true) {
				foreach (DocumentTab i in TabCell.Children) {
					if (Document.Editor.My.Settings.Options_SpellCheck == true) {
						SpellCheck.SetIsEnabled(i.Editor, true);
					} else {
						SpellCheck.SetIsEnabled(i.Editor, false);
					}
				}
				if (o.PluginsCheckBox.IsChecked) {
					Plugins plugins = new Plugins();
					PluginsGroup.Items.Clear();
					foreach (string file in Document.Editor.My.Computer.FileSystem.GetFiles(Document.Editor.My.Application.Info.DirectoryPath + "\\Plugins\\")) {
						try {
							System.IO.FileInfo plugin = new System.IO.FileInfo(file);
							string pluginname = plugin.Name.Remove(plugin.Name.Length - 3);
							bool IsEventPlugin = plugins.IsEventPlugin(pluginname, Document.Editor.My.Computer.FileSystem.ReadAllText(file));
							if (IsEventPlugin) {
								Fluent.Button ribbonbutton = new Fluent.Button();
								ribbonbutton.Header = pluginname;
								if (Document.Editor.My.Computer.FileSystem.FileExists(Document.Editor.My.Application.Info.DirectoryPath + "\\Plugins\\" + pluginname + "32.png")) {
									BitmapImage bitmap = new BitmapImage(new Uri(Document.Editor.My.Application.Info.DirectoryPath + "\\Plugins\\" + pluginname + "32.png"));
									ribbonbutton.LargeIcon = bitmap;
								} else {
									BitmapImage bitmap = new BitmapImage(new Uri(Document.Editor.My.Application.Info.DirectoryPath + "\\Images\\Tools\\plugins32.png"));
									ribbonbutton.LargeIcon = bitmap;
								}
								Fluent.ScreenTip tip = new Fluent.ScreenTip();
								tip.Title = pluginname;
								tip.Image = new BitmapImage(new Uri("pack://application:,,,/Images/Tools/plugins48.png"));
								ribbonbutton.Tag = file;
								ribbonbutton.Click += new RoutedEventHandler(RunPlugin);
								PluginsGroup.Items.Add(ribbonbutton);
							}
						} catch (Exception ex) {
							MessageBoxDialog m = new MessageBoxDialog(ex.Message, ex.ToString(), null, null);
							m.MessageImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Common/error32.png"));
							m.Owner = this;
							m.ShowDialog();
						}
					}
				} else {
					PluginsGroup.Items.Clear();
					PluginsGroup.Visibility = System.Windows.Visibility.Collapsed;
				}
			}
		}

		#endregion

		#region "Contextual Groups"

		#region "Update"

		#region "Table Cell"

		private void SetSelectedTableCell(TableCell tablecell)
		{
			SelectedDocument.Editor.SelectedTableCell = tablecell;
			TableRow tr = SelectedDocument.Editor.SelectedTableCell.Parent;
			TableRowGroup trg = tr.Parent;
			Table table = trg.Parent;
			TableBorderSizeBox.Value = table.BorderThickness.Left;
			TableCellSpacingBox.Value = table.CellSpacing;
			TableCellBorderSizeBox.Value = tablecell.BorderThickness.Left;
			EditTableCellGroup.Visibility = System.Windows.Visibility.Visible;
		}

		#endregion

		#region "Image"

		private void SetSelectedImage(Image Img)
		{
			SelectedDocument.Editor.SelectedImage = new Image();
			ImageHeightBox.Value = Img.Height;
			ImageWidthBox.Value = Img.Width;
			if (Img.Stretch == Stretch.Fill) {
				ImageResizeModeComboBox.SelectedIndex = 0;
			} else if (Img.Stretch == Stretch.Uniform) {
				ImageResizeModeComboBox.SelectedIndex = 1;
			} else if (Img.Stretch == Stretch.UniformToFill) {
				ImageResizeModeComboBox.SelectedIndex = 2;
			} else if (Img.Stretch == Stretch.None) {
				ImageResizeModeComboBox.SelectedIndex = 3;
			}
			SelectedDocument.Editor.SelectedImage = Img;
			EditTableCellGroup.Visibility = System.Windows.Visibility.Collapsed;
			EditImageGroup.Visibility = System.Windows.Visibility.Visible;
			EditVideoGroup.Visibility = System.Windows.Visibility.Collapsed;
			EditShapeGroup.Visibility = System.Windows.Visibility.Collapsed;
			EditObjectGroup.Visibility = System.Windows.Visibility.Collapsed;
		}

		#endregion

		#region "Video"

		private void SetSelectedVideo(MediaElement vid)
		{
			SelectedDocument.Editor.SelectedVideo = vid;
			VideoHeightBox.Value = vid.Height;
			VideoWidthBox.Value = vid.Width;
			if (vid.Stretch == Stretch.Fill) {
				VideoResizeModeComboBox.SelectedIndex = 0;
			} else if (vid.Stretch == Stretch.Uniform) {
				VideoResizeModeComboBox.SelectedIndex = 1;
			} else if (vid.Stretch == Stretch.UniformToFill) {
				VideoResizeModeComboBox.SelectedIndex = 2;
			} else if (vid.Stretch == Stretch.None) {
				VideoResizeModeComboBox.SelectedIndex = 3;
			}
			//TODO:
			EditTableCellGroup.Visibility = System.Windows.Visibility.Collapsed;
			EditImageGroup.Visibility = System.Windows.Visibility.Collapsed;
			EditVideoGroup.Visibility = System.Windows.Visibility.Visible;
			EditShapeGroup.Visibility = System.Windows.Visibility.Collapsed;
			EditObjectGroup.Visibility = System.Windows.Visibility.Collapsed;
		}

		#endregion
		//TODO:
		#region "Shape"

		private void SetSelectedShape(Shape s)
		{
			SelectedDocument.Editor.SelectedShape = s;
			ShapeHeightBox.Value = s.Height;
			ShapeWidthBox.Value = s.Width;
			//If vid.Stretch = Stretch.Fill Then
			//    VideoResizeModeComboBox.SelectedIndex = 0
			//ElseIf vid.Stretch = Stretch.Uniform Then
			//    VideoResizeModeComboBox.SelectedIndex = 1
			//ElseIf vid.Stretch = Stretch.UniformToFill Then
			//    VideoResizeModeComboBox.SelectedIndex = 2
			//ElseIf vid.Stretch = Stretch.None Then
			//    VideoResizeModeComboBox.SelectedIndex = 3
			//End If
			//TODO:
			EditTableCellGroup.Visibility = System.Windows.Visibility.Collapsed;
			EditImageGroup.Visibility = System.Windows.Visibility.Collapsed;
			EditVideoGroup.Visibility = System.Windows.Visibility.Collapsed;
			EditShapeGroup.Visibility = System.Windows.Visibility.Visible;
			EditObjectGroup.Visibility = System.Windows.Visibility.Collapsed;
		}

		#endregion

		#region "Object"

		private void SetSelectedObject(UIElement uielement)
		{
			if (object.ReferenceEquals(uielement.GetType(), typeof(Button)) || object.ReferenceEquals(uielement.GetType(), typeof(RadioButton)) || object.ReferenceEquals(uielement.GetType(), typeof(CheckBox)) || object.ReferenceEquals(uielement.GetType(), typeof(TextBlock))) {
				SelectedDocument.Editor.SelectedObject = uielement;
				ObjectBorderGroup.Visibility = System.Windows.Visibility.Visible;
				Button button = uielement as Button;
				if (button != null) {
					ObjectHeightBox.Value = button.Height;
					ObjectWidthBox.Value = button.Width;
					ObjectBorderSizeBox.Value = button.BorderThickness.Left;
					ObjectTextBox.Text = button.Content.ToString();
					if (button.IsEnabled) {
						ObjectEnabledCheckBox.IsChecked = true;
					} else {
						ObjectEnabledCheckBox.IsChecked = false;
					}
				} else {
					RadioButton radiobutton = uielement as RadioButton;
					if (radiobutton != null) {
						ObjectHeightBox.Value = radiobutton.Height;
						ObjectWidthBox.Value = radiobutton.Width;
						ObjectBorderSizeBox.Value = radiobutton.BorderThickness.Left;
						ObjectTextBox.Text = radiobutton.Content.ToString();
						if (radiobutton.IsEnabled) {
							ObjectEnabledCheckBox.IsChecked = true;
						} else {
							ObjectEnabledCheckBox.IsChecked = false;
						}
					} else {
						CheckBox checkbox = uielement as CheckBox;
						if (checkbox != null) {
							ObjectHeightBox.Value = checkbox.Height;
							ObjectWidthBox.Value = checkbox.Width;
							ObjectBorderSizeBox.Value = checkbox.BorderThickness.Left;
							ObjectTextBox.Text = checkbox.Content.ToString();
							if (checkbox.IsEnabled) {
								ObjectEnabledCheckBox.IsChecked = true;
							} else {
								ObjectEnabledCheckBox.IsChecked = false;
							}
						} else {
							TextBlock textblock = uielement as TextBlock;
							if (textblock != null) {
								ObjectHeightBox.Value = textblock.Height;
								ObjectWidthBox.Value = textblock.Width;
								ObjectBorderGroup.Visibility = System.Windows.Visibility.Collapsed;
								ObjectTextBox.Text = textblock.Text;
								if (textblock.IsEnabled) {
									ObjectEnabledCheckBox.IsChecked = true;
								} else {
									ObjectEnabledCheckBox.IsChecked = false;
								}
							}
						}
					}
				}
				EditTableCellGroup.Visibility = System.Windows.Visibility.Collapsed;
				EditImageGroup.Visibility = System.Windows.Visibility.Collapsed;
				EditVideoGroup.Visibility = System.Windows.Visibility.Collapsed;
				EditObjectGroup.Visibility = System.Windows.Visibility.Visible;
			}
		}

		#endregion

		private void UpdateContextualTabs()
		{
			if (!SelectedDocument.Editor.Selection.IsEmpty) {
				foreach (Block block in SelectedDocument.Editor.Document.Blocks) {
					BlockUIContainer blockui = block as BlockUIContainer;
					if (blockui != null) {
						UIElement uielement = blockui.Child as UIElement;
						if (uielement != null) {
							if (object.ReferenceEquals(uielement.GetType(), typeof(Image))) {
								SetSelectedImage(uielement);
								return;
							} else {
								SetSelectedObject(uielement);
								return;
							}
						}
					} else {
						List list = block as List;
						if (list != null) {
							foreach (ListItem litem in list.ListItems) {
								foreach (Block b in litem.Blocks) {
									BlockUIContainer bui = block as BlockUIContainer;
									if (bui != null) {
										//TODO: (20xx.xx) add support for blockui inside of listitems
									} else {
										Paragraph p = b as Paragraph;
										if (p != null) {
											foreach (Inline i in p.Inlines) {
												InlineUIContainer iui = i as InlineUIContainer;
												if (iui != null) {
													if (SelectedDocument.Editor.Selection.Start.CompareTo(iui.ElementStart) == 0 && SelectedDocument.Editor.Selection.End.CompareTo(iui.ElementEnd) == 0) {
														if (object.ReferenceEquals(iui.Child.GetType(), typeof(Image))) {
															SetSelectedImage(iui.Child);
															return;
														} else if (object.ReferenceEquals(iui.Child.GetType(), typeof(MediaElement))) {
															SetSelectedVideo(iui.Child);
															return;
														} else if (object.ReferenceEquals(iui.Child.GetType(), typeof(Shape))) {
															SetSelectedShape(iui.Child);
															return;
														} else {
															SetSelectedObject(iui.Child);
															return;
														}
													}
												}
											}
										} else {
											//TODO: (20xx.xx) add suport for the rest of the "Container" types
										}
									}
								}
							}
						} else {
							Paragraph par = block as Paragraph;
							if (par != null) {
								foreach (Inline inline in par.Inlines) {
									InlineUIContainer inlineui = inline as InlineUIContainer;
									if (inlineui != null) {
										if (SelectedDocument.Editor.Selection.Start.CompareTo(inlineui.ElementStart) == 0 && SelectedDocument.Editor.Selection.End.CompareTo(inlineui.ElementEnd) == 0) {
											if (object.ReferenceEquals(inlineui.Child.GetType(), typeof(Image))) {
												SetSelectedImage(inlineui.Child);
												return;
											} else if (object.ReferenceEquals(inlineui.Child.GetType(), typeof(MediaElement))) {
												SetSelectedVideo(inlineui.Child);
												return;
											} else if (object.ReferenceEquals(inlineui.Child.GetType(), typeof(Shape))) {
												SetSelectedShape(inlineui.Child);
												return;
											} else {
												SetSelectedObject(inlineui.Child);
												return;
											}
										}
									}
								}
							} else {
								Section sec = block as Section;
								if (sec != null) {
									//TODO: (20xx.xx) add support for sections
								} else {
									Table table = block as Table;
									if (table != null) {
										foreach (TableRowGroup rowgroup in table.RowGroups) {
											foreach (TableRow row in rowgroup.Rows) {
												foreach (TableCell cell in row.Cells) {
													foreach (Block b in cell.Blocks) {
														BlockUIContainer bui = b as BlockUIContainer;
														if (bui != null) {
															UIElement uie = bui.Child as UIElement;
															if (uie != null) {
																if (object.ReferenceEquals(uie.GetType(), typeof(Image))) {
																	SetSelectedImage(uie);
																	return;
																} else if (object.ReferenceEquals(uie.GetType(), typeof(MediaElement))) {
																	SetSelectedVideo(uie);
																	return;
																} else if (object.ReferenceEquals(uie.GetType(), typeof(Shape))) {
																	SetSelectedShape(uie);
																	return;
																} else {
																	SetSelectedObject(uie);
																	return;
																}
															}
														} else {
															List l = b as List;
															if (l != null) {
																//For Each litem As ListItem In l.ListItems
																//    For Each b2 As Block In litem.Blocks
																//        Dim bui2 As BlockUIContainer = TryCast(b2, BlockUIContainer)
																//        If bui2 IsNot Nothing Then
																//            'TODO: add support for blockui inside of listitems inside of tables
																//        Else
																//            Dim p As Paragraph = TryCast(b2, Paragraph)
																//            If p IsNot Nothing Then
																//                For Each i As Inline In p.Inlines
																//                    Dim iui As InlineUIContainer = TryCast(i, InlineUIContainer)
																//                    If iui IsNot Nothing Then
																//                        If SelectedDocument.Editor.Selection.Start.CompareTo(iui.ElementStart) = 0 AndAlso SelectedDocument.Editor.Selection.End.CompareTo(iui.ElementEnd) = 0 Then
																//                            If iui.Child.GetType Is GetType(Image) Then
																//                                SetSelectedImage(iui.Child)
																//                                Exit Sub
																//                            Else
																//                                SetSelectedObject(iui.Child)
																//                                Exit Sub
																//                            End If
																//                        End If
																//                    End If
																//                Next
																//            Else
																//                'TODO: add suport for the rest of the "Container" types
																//            End If
																//        End If
																//    Next
																//Next
															} else {
																Paragraph p = b as Paragraph;
																if (p != null) {
																	foreach (Inline inl in p.Inlines) {
																		InlineUIContainer inlui = inl as InlineUIContainer;
																		if (inlui != null) {
																			if (SelectedDocument.Editor.Selection.Start.CompareTo(inlui.ElementStart) == 0 && SelectedDocument.Editor.Selection.End.CompareTo(inlui.ElementEnd) == 0) {
																				if (object.ReferenceEquals(inlui.Child.GetType(), typeof(Image))) {
																					SetSelectedImage(inlui.Child);
																					return;
																				} else if (object.ReferenceEquals(inlui.Child.GetType(), typeof(MediaElement))) {
																					SetSelectedVideo(inlui.Child);
																					return;
																				} else if (object.ReferenceEquals(inlui.Child.GetType(), typeof(Shape))) {
																					SetSelectedShape(inlui.Child);
																					return;
																				} else {
																					SetSelectedObject(inlui.Child);
																					return;
																				}
																			}
																		}
																	}
																} else {
																	Section s = b as Section;
																	if (s != null) {
																		//TODO:(20xx.xx) add support for sections inside of tables
																	}
																}
															}
														}
													}
												}
											}
										}
									} else {
									}
								}
							}
						}
					}
				}
			} else {
				EditTableCellGroup.Visibility = System.Windows.Visibility.Collapsed;
				EditImageGroup.Visibility = System.Windows.Visibility.Collapsed;
				EditVideoGroup.Visibility = System.Windows.Visibility.Collapsed;
				EditShapeGroup.Visibility = System.Windows.Visibility.Collapsed;
				EditObjectGroup.Visibility = System.Windows.Visibility.Collapsed;
				if (SelectedDocument.Editor.CaretPosition.Paragraph != null) {
					Paragraph p = SelectedDocument.Editor.CaretPosition.Paragraph;
					if (object.ReferenceEquals(p.Parent.GetType(), typeof(TableCell))) {
						SetSelectedTableCell(p.Parent);
					}
				}
				if (object.ReferenceEquals(SelectedDocument.Editor.CaretPosition.Parent.GetType(), typeof(TableCell))) {
					SetSelectedTableCell(SelectedDocument.Editor.CaretPosition.Parent);
				}
				//Dim m As New MessageBoxDialog(SelectedDocument.Editor.CaretPosition.GetType.ToString, "Info!", Nothing, Nothing)
				//m.MessageImage.Source = New BitmapImage(New Uri("pack://application:,,,/Images/Common/info32.png"))
				//m.Owner = Me
				//m.ShowDialog()
			}
		}

		#endregion

		#region "EditImageTab"

		#region "Resize"

		private void ImageHeightBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (IsLoaded) {
				SelectedDocument.Editor.SelectedImage.Height = ImageHeightBox.Value;
			}
		}

		private void ImageWidthBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (IsLoaded) {
				SelectedDocument.Editor.SelectedImage.Width = ImageWidthBox.Value;
			}
		}

		private void ImageResizeModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (ImageResizeModeComboBox.SelectedIndex == 0) {
				SelectedDocument.Editor.SelectedImage.Stretch = Stretch.Fill;
			} else if (ImageResizeModeComboBox.SelectedIndex == 1) {
				SelectedDocument.Editor.SelectedImage.Stretch = Stretch.Uniform;
			} else if (ImageResizeModeComboBox.SelectedIndex == 2) {
				SelectedDocument.Editor.SelectedImage.Stretch = Stretch.UniformToFill;
			} else if (ImageResizeModeComboBox.SelectedIndex == 3) {
				SelectedDocument.Editor.SelectedImage.Stretch = Stretch.None;
			}
		}

		#endregion

		#region "Rotate"

		private void RotateImageLeftMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Thickness imgprops = SelectedDocument.Editor.SelectedImage.Tag;
			imgprops.Left -= 90;
			SelectedDocument.Editor.SelectedImage.Tag = imgprops;
			TransformGroup transform = SelectedDocument.Editor.SelectedImage.LayoutTransform;
			RotateTransform rotate = new RotateTransform(imgprops.Left);
			transform.Children[0] = rotate;
			SelectedDocument.Editor.SelectedImage.LayoutTransform = transform;
		}

		private void RotateImageRightMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Thickness imgprops = SelectedDocument.Editor.SelectedImage.Tag;
			imgprops.Left += 90;
			SelectedDocument.Editor.SelectedImage.Tag = imgprops;
			TransformGroup transform = SelectedDocument.Editor.SelectedImage.LayoutTransform;
			RotateTransform rotate = new RotateTransform(imgprops.Left);
			transform.Children[0] = rotate;
			SelectedDocument.Editor.SelectedImage.LayoutTransform = transform;
		}

		#endregion

		#region "Flip"

		private void FlipImageHorizontalMenuItem_Click(object sender, RoutedEventArgs e)
		{
			//TODO: (20xx.xx) add flip image horizontal support
			Thickness imgprops = SelectedDocument.Editor.SelectedImage.Tag;
			if (imgprops.Top == 1) {
				imgprops.Top = -1;
			} else {
				imgprops.Top = 1;
			}
			SelectedDocument.Editor.SelectedImage.Tag = imgprops;
			TransformGroup transform = SelectedDocument.Editor.SelectedImage.LayoutTransform;
			ScaleTransform hflip = new ScaleTransform(imgprops.Top, imgprops.Right);
			transform.Children[1] = hflip;
			SelectedDocument.Editor.SelectedImage.LayoutTransform = transform;
		}

		private void FlipImageVerticalMenuItem_Click(object sender, RoutedEventArgs e)
		{
			//TODO: (20xx.xx) add flip image vertical support
			Thickness imgprops = SelectedDocument.Editor.SelectedImage.Tag;
			if (imgprops.Right == 1) {
				imgprops.Right = -1;
			} else {
				imgprops.Right = 1;
			}
			SelectedDocument.Editor.SelectedImage.Tag = imgprops;
			TransformGroup transform = SelectedDocument.Editor.SelectedImage.LayoutTransform;
			ScaleTransform vflip = new ScaleTransform(imgprops.Top, imgprops.Right);
			transform.Children[2] = vflip;
			SelectedDocument.Editor.SelectedImage.LayoutTransform = vflip;
		}

		#endregion

		#endregion

		#region "EditVideoTab"

		#region "Resize"

		private void VideoHeightBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (IsLoaded) {
				SelectedDocument.Editor.SelectedVideo.Height = VideoHeightBox.Value;
			}
		}

		private void VideoWidthBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (IsLoaded) {
				SelectedDocument.Editor.SelectedVideo.Width = VideoWidthBox.Value;
			}
		}

		private void VideoResizeModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (VideoResizeModeComboBox.SelectedIndex == 0) {
				SelectedDocument.Editor.SelectedVideo.Stretch = Stretch.Fill;
			} else if (VideoResizeModeComboBox.SelectedIndex == 1) {
				SelectedDocument.Editor.SelectedVideo.Stretch = Stretch.Uniform;
			} else if (VideoResizeModeComboBox.SelectedIndex == 2) {
				SelectedDocument.Editor.SelectedVideo.Stretch = Stretch.UniformToFill;
			} else if (VideoResizeModeComboBox.SelectedIndex == 3) {
				SelectedDocument.Editor.SelectedVideo.Stretch = Stretch.None;
			}
		}

		#endregion

		private void VideoPlayMenuItem_Click(object sender, RoutedEventArgs e)
		{
			SelectedDocument.Editor.SelectedVideo.Play();
		}

		#endregion

		#region "EditTableCellTab"

		private int LastSelectedTableEditingTab = 0;
		private void MainBar_SelectedTabChanged(object sender, SelectionChangedEventArgs e)
		{
			if (object.ReferenceEquals(MainBar.SelectedTabItem, EditTableTab)) {
				LastSelectedTableEditingTab = 0;
			} else if (object.ReferenceEquals(MainBar.SelectedTabItem, EditTableCellTab)) {
				LastSelectedTableEditingTab = 1;
			}
		}

		#region "EditTableTab"

		private void TableBackgroundColorGallery_SelectedColorChanged(object sender, RoutedEventArgs e)
		{
			TableRow tr = SelectedDocument.Editor.SelectedTableCell.Parent;
			TableRowGroup trg = tr.Parent;
			Table table = trg.Parent;
			table.Background = new SolidColorBrush(TableBackgroundColorGallery.SelectedColor);
		}

		private void TableBorderColorGallery_SelectedColorChanged(object sender, RoutedEventArgs e)
		{
			TableRow tr = SelectedDocument.Editor.SelectedTableCell.Parent;
			TableRowGroup trg = tr.Parent;
			Table table = trg.Parent;
			table.BorderBrush = new SolidColorBrush(TableBorderColorGallery.SelectedColor);
		}

		private void TableBorderSizeBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (IsLoaded) {
				TableRow tr = SelectedDocument.Editor.SelectedTableCell.Parent;
				TableRowGroup trg = tr.Parent;
				Table table = trg.Parent;
				int size = Convert.ToInt32(TableBorderSizeBox.Value);
				table.BorderThickness = new Thickness(size, size, size, size);
			}
		}

		private void TableCellSpacingBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (IsLoaded) {
				TableRow tr = SelectedDocument.Editor.SelectedTableCell.Parent;
				TableRowGroup trg = tr.Parent;
				Table table = trg.Parent;
				table.CellSpacing = TableCellSpacingBox.Value;
			}
		}

		#endregion

		private void TableCellBackgroundColorGallery_SelectedColorChanged(object sender, RoutedEventArgs e)
		{
			SelectedDocument.Editor.SelectedTableCell.Background = new SolidColorBrush(TableCellBackgroundColorGallery.SelectedColor);
		}

		private void TableCellBorderColorGallery_SelectedColorChanged(object sender, RoutedEventArgs e)
		{
			SelectedDocument.Editor.SelectedTableCell.BorderBrush = new SolidColorBrush(TableCellBorderColorGallery.SelectedColor);
		}

		private void TableCellBorderSizeBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (IsLoaded) {
				double size = TableCellBorderSizeBox.Value;
				SelectedDocument.Editor.SelectedTableCell.BorderThickness = new Thickness(size, size, size, size);
			}
		}

		#endregion

		#region "EditObjectTab"

		private void ObjectHeightBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (IsLoaded) {
				Button button = SelectedDocument.Editor.SelectedObject as Button;
				if (button != null) {
					button.Height = ObjectHeightBox.Value;
				} else {
					RadioButton radiobutton = SelectedDocument.Editor.SelectedObject as RadioButton;
					if (radiobutton != null) {
						radiobutton.Height = ObjectHeightBox.Value;
					} else {
						CheckBox checkbox = SelectedDocument.Editor.SelectedObject as CheckBox;
						if (checkbox != null) {
							checkbox.Height = ObjectHeightBox.Value;
						} else {
							TextBlock textblock = SelectedDocument.Editor.SelectedObject as TextBlock;
							if (textblock != null) {
								textblock.Height = ObjectHeightBox.Value;
							}
						}
					}
				}
			}
		}

		private void ObjectWidthBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (IsLoaded) {
				Button button = SelectedDocument.Editor.SelectedObject as Button;
				if (button != null) {
					button.Width = ObjectWidthBox.Value;
				} else {
					RadioButton radiobutton = SelectedDocument.Editor.SelectedObject as RadioButton;
					if (radiobutton != null) {
						radiobutton.Width = ObjectWidthBox.Value;
					} else {
						CheckBox checkbox = SelectedDocument.Editor.SelectedObject as CheckBox;
						if (checkbox != null) {
							checkbox.Width = ObjectWidthBox.Value;
						} else {
							TextBlock textblock = SelectedDocument.Editor.SelectedObject as TextBlock;
							if (textblock != null) {
								textblock.Width = ObjectWidthBox.Value;
							}
						}
					}
				}
			}
		}

		private void ObjectBackgroundColorGallery_SelectedColorChanged(object sender, RoutedEventArgs e)
		{
			Button button = SelectedDocument.Editor.SelectedObject as Button;
			if (button != null) {
				button.Background = new SolidColorBrush(ObjectBackgroundColorGallery.SelectedColor);
			} else {
				RadioButton radiobutton = SelectedDocument.Editor.SelectedObject as RadioButton;
				if (radiobutton != null) {
					radiobutton.Background = new SolidColorBrush(ObjectBackgroundColorGallery.SelectedColor);
				} else {
					CheckBox checkbox = SelectedDocument.Editor.SelectedObject as CheckBox;
					if (checkbox != null) {
						checkbox.Background = new SolidColorBrush(ObjectBackgroundColorGallery.SelectedColor);
					} else {
						TextBlock textblock = SelectedDocument.Editor.SelectedObject as TextBlock;
						if (textblock != null) {
							textblock.Background = new SolidColorBrush(ObjectBackgroundColorGallery.SelectedColor);
						}
					}
				}
			}
		}

		private void ObjectBorderColorGallery_SelectedColorChanged(object sender, RoutedEventArgs e)
		{
			Button button = SelectedDocument.Editor.SelectedObject as Button;
			if (button != null) {
				button.BorderBrush = new SolidColorBrush(ObjectBorderColorGallery.SelectedColor);
			} else {
				RadioButton radiobutton = SelectedDocument.Editor.SelectedObject as RadioButton;
				if (radiobutton != null) {
					radiobutton.BorderBrush = new SolidColorBrush(ObjectBorderColorGallery.SelectedColor);
				} else {
					CheckBox checkbox = SelectedDocument.Editor.SelectedObject as CheckBox;
					if (checkbox != null) {
						checkbox.BorderBrush = new SolidColorBrush(ObjectBorderColorGallery.SelectedColor);
					}
				}
			}
		}

		private void ObjectBorderSizeBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (IsLoaded) {
				int size = Convert.ToInt32(ObjectBorderSizeBox.Value);
				Button button = SelectedDocument.Editor.SelectedObject as Button;
				if (button != null) {
					button.BorderThickness = new Thickness(size, size, size, size);
				} else {
					RadioButton radiobutton = SelectedDocument.Editor.SelectedObject as RadioButton;
					if (radiobutton != null) {
						radiobutton.BorderThickness = new Thickness(size, size, size, size);
					} else {
						CheckBox checkbox = SelectedDocument.Editor.SelectedObject as CheckBox;
						if (checkbox != null) {
							checkbox.BorderThickness = new Thickness(size, size, size, size);
						}
					}
				}
			}
		}

		private void ObjectTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			Button button = SelectedDocument.Editor.SelectedObject as Button;
			if (button != null) {
				button.Content = ObjectTextBox.Text;
			} else {
				RadioButton radiobutton = SelectedDocument.Editor.SelectedObject as RadioButton;
				if (radiobutton != null) {
					radiobutton.Content = ObjectTextBox.Text;
				} else {
					CheckBox checkbox = SelectedDocument.Editor.SelectedObject as CheckBox;
					if (checkbox != null) {
						checkbox.Content = ObjectTextBox.Text;
					} else {
						TextBlock textblock = SelectedDocument.Editor.SelectedObject as TextBlock;
						if (textblock != null) {
							textblock.Text = ObjectTextBox.Text;
						}
					}
				}
			}
		}

		private void ObjectEnabledCheckBox_Click(object sender, RoutedEventArgs e)
		{
			if (ObjectEnabledCheckBox.IsChecked) {
				Button button = SelectedDocument.Editor.SelectedObject as Button;
				if (button != null) {
					button.IsEnabled = true;
				} else {
					RadioButton radiobutton = SelectedDocument.Editor.SelectedObject as RadioButton;
					if (radiobutton != null) {
						radiobutton.IsEnabled = true;
					} else {
						CheckBox checkbox = SelectedDocument.Editor.SelectedObject as CheckBox;
						if (checkbox != null) {
							checkbox.IsEnabled = true;
						} else {
							TextBlock textblock = SelectedDocument.Editor.SelectedObject as TextBlock;
							if (textblock != null) {
								textblock.IsEnabled = true;
							}
						}
					}
				}
			} else {
				Button button = SelectedDocument.Editor.SelectedObject as Button;
				if (button != null) {
					button.IsEnabled = false;
				} else {
					RadioButton radiobutton = SelectedDocument.Editor.SelectedObject as RadioButton;
					if (radiobutton != null) {
						radiobutton.IsEnabled = false;
					} else {
						CheckBox checkbox = SelectedDocument.Editor.SelectedObject as CheckBox;
						if (checkbox != null) {
							checkbox.IsEnabled = false;
						} else {
							TextBlock textblock = SelectedDocument.Editor.SelectedObject as TextBlock;
							if (textblock != null) {
								textblock.IsEnabled = false;
							}
						}
					}
				}
			}
		}

		#endregion

		#endregion

		#region "Toolbar Items"

		#region "--HelpMenuItem"

		private void GetHelpButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Process.Start("http://documenteditor.net/documentation/");
		}

		private void ReportMenuItem_Click(System.Object sender, System.Windows.RoutedEventArgs e)
		{
			try {
				Process.Start("mailto:semagsoft@gmail.com");
			} catch (Exception ex) {
				MessageBoxDialog m = new MessageBoxDialog(ex.Message, ex.ToString(), null, null);
				m.MessageImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Common/error32.png"));
				m.Owner = this;
				m.ShowDialog();
			}
		}

		private void DonateMenuItem_Click(System.Object sender, System.Windows.RoutedEventArgs e)
		{
			Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=K4QJBR4UJ3W5E&lc=US&item_name=Semagsoft&currency_code=USD&bn=PP%2dDonationsBF%3abtn_donateCC_LG%2egif%3aNonHosted");
		}

		private void GetPluginsButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Process.Start("http://documenteditor.net/plugins");
		}

		private void WebsiteMenuItem_Click(System.Object sender, System.Windows.RoutedEventArgs e)
		{
			Process.Start("http://documenteditor.net/");
		}

		private void AboutMenuItem_Click(System.Object sender, System.Windows.RoutedEventArgs e)
		{
			AboutDialog ad = new AboutDialog();
			ad.Owner = this;
			ad.ShowDialog();
		}

		#endregion

		#endregion

		#endregion

		#region "TabCell"

		private void CloseDoc(object sender, System.Windows.RoutedEventArgs e)
		{
			TabHeader b = e.Source;
			DocumentTab i = b.Parent;
			if (i.Editor.FileChanged) {
				SaveFileDialog SaveDialog = new SaveFileDialog();
				System.IO.FileStream fs = System.IO.File.Open(Document.Editor.My.Computer.FileSystem.SpecialDirectories.Temp + "\\TVPre.xaml", System.IO.FileMode.Create);
				TextRange tr = new TextRange(i.Editor.Document.ContentStart, i.Editor.Document.ContentEnd);
				SaveDialog.Owner = this;
				SaveDialog.SetFileInfo(i.DocName, i.Editor);
				System.Windows.Markup.XamlWriter.Save(i.Editor.Document, fs);
				fs.Close();
				SaveDialog.ShowDialog();
				if (SaveDialog.Res == "Yes") {
					SaveMenuItem_Click(this, null);
					CloseDocument(i);
				} else if (SaveDialog.Res == "No") {
					CloseDocument(i);
				}
			} else {
				CloseDocument(i);
			}
		}

		//Dim t As DocumentTab
		//Dim oldPosition As Integer = -1
		//Private Sub TabCell_PreviewMouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles TabCell.PreviewMouseDown
		//    If e.Source.ToString = "Document.Editor.TabHeader" Then
		//        Dim h As TabHeader = e.Source
		//        t = h.Parent
		//        Dim o As Object = TabCell.ItemContainerGenerator.IndexFromContainer(TryCast(h.Parent, DocumentTab))
		//        oldPosition = Int16.Parse(o.ToString)
		//    End If
		//End Sub

		//Private Sub TabCell_PreviewMouseUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles TabCell.PreviewMouseUp
		//    If e.Source.ToString = "Document.Editor.TabHeader" Then
		//        Dim h As TabHeader = e.Source
		//        Dim o As Object = TabCell.ItemContainerGenerator.IndexFromContainer(TryCast(h.Parent, DocumentTab))
		//        Dim i As Integer = Int16.Parse(o.ToString)
		//        If o IsNot Nothing AndAlso Not oldPosition = i Then
		//            TabCell.Items.RemoveAt(oldPosition)
		//            TabCell.Items.Insert(i, t)
		//            TabCell.SelectedItem = t
		//        End If
		//    End If
		//End Sub

		//Private Sub TabCell_SelectionChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles TabCell.SelectionChanged
		//    If TabCell.SelectedIndex = -1 Then
		//        SelectedDocument = Nothing
		//        UpdateButtons()
		//    Else
		//        If SelectedDocument IsNot Nothing Then
		//            SelectedDocument.HeaderContent.Padding = New Thickness(4, 5, 4, 5)
		//            SelectedDocument.HeaderContent.FileTypeImage.Margin = New Thickness(0, 1, 0, 0)
		//            SelectedDocument.HeaderContent.TabTitle.Margin = New Thickness(0, 0, 0, 0)
		//            SelectedDocument.HeaderContent.CloseButton.Margin = New Thickness(0, 1, 0, 0)
		//        End If
		//        SelectedDocument = TabCell.SelectedItem
		//        If SelectedDocument IsNot Nothing Then
		//            SelectedDocument.HeaderContent.Padding = New Thickness(4, 4, 4, 5)
		//            SelectedDocument.HeaderContent.FileTypeImage.Margin = New Thickness(0, 1, 0, 0)
		//            SelectedDocument.HeaderContent.TabTitle.Margin = New Thickness(0, 1, 0, 0)
		//            SelectedDocument.HeaderContent.CloseButton.Margin = New Thickness(0, 3, 0, 0)
		//        End If
		//        UpdateUI()
		//        UpdateButtons()
		//    End If
		//End Sub

		#endregion

		#region "Statusbar"

		private void StatusbarInfo_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2) {
				BackStageMenu.IsOpen = true;
				PropertiesMenuItem.IsSelected = true;
				PropertiesTabControl.SelectedIndex = 1;
			}
		}

		private void zoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (zoomSlider.IsLoaded) {
				ScaleTransform t = new ScaleTransform(zoomSlider.Value / 100, zoomSlider.Value / 100);
				ScaleTransform rulerzoom = new ScaleTransform(zoomSlider.Value / 100, 1);
				SelectedDocument.Ruler.LayoutTransform = rulerzoom;
				SelectedDocument.Editor.LayoutTransform = t;
				SelectedDocument.Editor.ZoomLevel = zoomSlider.Value / 100;
				UpdateButtons();
			}
		}

		#endregion

		#region "Misc"

		private void DocPreviewScrollViewer_Loaded(object sender, RoutedEventArgs e)
		{
			DocPreviewScrollViewer.Visibility = System.Windows.Visibility.Collapsed;
		}

		#endregion

		private void dockingManager_ActiveContentChanged(object sender, EventArgs e)
		{
			Grid grid = dockingManager.ActiveContent as Grid;
			if (grid != null) {
				SelectedDocument = dockingManager.Layout.ActiveContent;
			} else {
				SelectedDocument = null;
			}
			UpdateButtons();
		}

		private void dockingManager_DocumentClosing(object sender, Xceed.Wpf.AvalonDock.DocumentClosingEventArgs e)
		{
			DocumentTab b = e.Document;
			if (b.Editor.FileChanged) {
				SaveFileDialog SaveDialog = new SaveFileDialog();
				System.IO.FileStream fs = System.IO.File.Open(Document.Editor.My.Computer.FileSystem.SpecialDirectories.Temp + "\\TVPre.xaml", System.IO.FileMode.Create);
				TextRange tr = new TextRange(b.Editor.Document.ContentStart, b.Editor.Document.ContentEnd);
				SaveDialog.Owner = this;
				SaveDialog.SetFileInfo(b.DocName, b.Editor);
				System.Windows.Markup.XamlWriter.Save(b.Editor.Document, fs);
				fs.Close();
				SaveDialog.ShowDialog();
				if (SaveDialog.Res == "Yes") {
					SaveMenuItem_Click(this, null);
				} else if (SaveDialog.Res == null) {
					e.Cancel = true;
				}
			}
			UpdateButtons();
		}
	}
}

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
	public class DocumentEditor : RichTextBox
	{

		public bool FileChanged = false;
		public string DocumentName = null;
		public double ZoomLevel = 1;
		public Thickness docpadding = new Thickness(96, 96, 96, 96);
		public TableCell SelectedTableCell = null;
		public Image SelectedImage = null;
		public MediaElement SelectedVideo = null;
		public Shape SelectedShape = null;

		public UIElement SelectedObject = null;
		public int SelectedLineNumber = 0;
		public int LineCount = 0;
		public int SelectedColumnNumber = 0;
		public int ColumnCount = 0;
		public int WordCount = 0;

		public DocumentEditor()
		{
			TextChanged += DocumentEditor_TextChanged;
			SelectionChanged += DocumentEditor_SelectionChanged;
			LayoutUpdated += DocumentEditor_LayoutUpdated;
			this.IsDocumentEnabled = true;
			Document.PageWidth = 816;
			Document.PageHeight = 1056;
			CaretPosition.Paragraph.LineHeight = 1.15;
			FontFamily = Document.Editor.My.Settings.Options_DefaultFont;
			FontSize = Document.Editor.My.Settings.Options_DefaultFontSize;
			AcceptsTab = true;
			Margin = new Thickness(-2, -3, 0, 0);
			SetPageMargins(docpadding);
			if (Document.Editor.My.Computer.FileSystem.FileExists(Document.Editor.My.Application.Info.DirectoryPath + "\\spellcheck_ignorelist.lex")) {
				IList dictionaries = SpellCheck.GetCustomDictionaries(this);
				dictionaries.Add(new Uri(Document.Editor.My.Application.Info.DirectoryPath + "\\spellcheck_ignorelist.lex"));
			}
		}

		public void SetupMarginsManager(FlowDocument document, Thickness margin)
		{
			docpadding = margin;
			System.ComponentModel.DependencyPropertyDescriptor dpd = System.ComponentModel.DependencyPropertyDescriptor.FromProperty(FlowDocument.PagePaddingProperty, typeof(FlowDocument));
			dpd.AddValueChanged(document, new EventHandler(SetPadding));
		}

		public void SetPageMargins(Thickness thickness)
		{
			docpadding = thickness;
			Document.PagePadding = docpadding;
		}

		private void SetPadding(object sender, EventArgs e)
		{
			FlowDocument fd = (FlowDocument)sender;
			if (fd.PagePadding != new Thickness(docpadding.Left, docpadding.Top, docpadding.Right, docpadding.Bottom)) {
				fd.PagePadding = docpadding;
			}
		}

		#region "LoadDocument"

		public void LoadDocument(string filename)
		{
			System.IO.FileInfo f = new System.IO.FileInfo(filename);
			TextRange tr = new TextRange(Document.ContentStart, Document.ContentEnd);
			System.IO.FileStream fs = null;
			bool isreadonlyfile = false;
			if (f.IsReadOnly) {
				isreadonlyfile = true;
			}
			if (f.Extension.ToLower() == ".xamlpackage") {
				TextRange t = new TextRange(Document.ContentStart, Document.ContentEnd);
				System.IO.FileStream file = new System.IO.FileStream(filename, System.IO.FileMode.Open);
				t.Load(file, System.Windows.DataFormats.XamlPackage);
				file.Close();
			} else if (f.Extension.ToLower() == ".xaml") {
				fs = System.IO.File.Open(f.FullName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
				FlowDocument content = System.Windows.Markup.XamlReader.Load(fs) as FlowDocument;
				Thickness thi = content.PagePadding;
				try {
					int leftmargin = thi.Left;
					int topmargin = thi.Top;
					int rightmargin = thi.Right;
					int bottommargin = thi.Bottom;
					SetPageMargins(new Thickness(leftmargin, topmargin, rightmargin, bottommargin));
				} catch (Exception ex) {
					SetPageMargins(new Thickness(0, 0, 0, 0));
				}
				Document = content;
			} else if (f.Extension.ToLower() == ".docx") {
				OpenXMLtoFlowDocument converter = new OpenXMLtoFlowDocument(f.FullName);
				FlowDocument content = converter.Convert();
				Document = content;
				converter.Close();
				//ElseIf f.Extension.ToLower = ".odt" Then
				//    fs.Close()
				//    fs = Nothing
				//    Dim converter As New OpenDocumenttoFlowDocument
				//    Document = converter.Convert(f.FullName)
			} else if (f.Extension.ToLower() == ".html" || f.Extension.ToLower() == ".htm") {
				try {
					FlowDocument content = System.Windows.Markup.XamlReader.Parse(HTMLConverter.HtmlToXamlConverter.ConvertHtmlToXaml(Document.Editor.My.Computer.FileSystem.ReadAllText(filename), true)) as FlowDocument;
					Document = content;
				} catch (Exception ex) {
					MessageBoxDialog m = new MessageBoxDialog("Error loading html document", "Error", null, null);
					m.MessageImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Common/error32.png"));
					m.Owner = Document.Editor.My.Windows.MainWindow;
					m.ShowDialog();
				}
			} else if (f.Extension.ToLower() == ".rtf") {
				if (isreadonlyfile) {
					fs = System.IO.File.Open(f.FullName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
				} else {
					fs = System.IO.File.Open(f.FullName, System.IO.FileMode.Open);
				}
				tr.Load(fs, System.Windows.DataFormats.Rtf);
			} else {
				if (isreadonlyfile) {
					fs = System.IO.File.Open(f.FullName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
				} else {
					fs = System.IO.File.Open(f.FullName, System.IO.FileMode.Open);
				}
				tr.Load(fs, System.Windows.DataFormats.Text);
			}
			if (fs != null) {
				fs.Close();
				fs = null;
			}
			if (f.IsReadOnly) {
				IsReadOnly = true;
			}
			DocumentName = filename;
			Semagsoft.HyperlinkHelper helper = new Semagsoft.HyperlinkHelper();
			helper.SubscribeToAllHyperlinks(Document);
			//Dim helper2 As New Semagsoft.ImageHelper
			//helper2.AddImageResizers(Me)
			//p.SetDocumentTitle(f.Name)
			FileChanged = false;
		}

		#endregion

		#region "SaveDocument"

		public void SaveDocument(string filename)
		{
			if (Document.Editor.My.Computer.FileSystem.FileExists(filename)) {
				Document.Editor.My.Computer.FileSystem.DeleteFile(filename);
			}
			System.IO.FileInfo file = new System.IO.FileInfo(filename);
			System.IO.FileStream fs = System.IO.File.Open(filename, System.IO.FileMode.OpenOrCreate);
			TextRange tr = new TextRange(Document.ContentStart, Document.ContentEnd);
			if (file.Extension.ToLower() == ".xamlpackage") {
				fs.Close();
				fs = null;
				TextRange range = null;
				System.IO.FileStream fStream = null;
				range = new TextRange(Document.ContentStart, Document.ContentEnd);
				fStream = new System.IO.FileStream(filename, System.IO.FileMode.Create);
				range.Save(fStream, DataFormats.XamlPackage, true);
				fStream.Close();
			} else if (file.Extension.ToLower() == ".xaml") {
				System.Windows.Markup.XamlWriter.Save(Document, fs);
				fs.Close();
				fs = null;
				System.Xml.XmlDocument xd = new System.Xml.XmlDocument();
				xd.LoadXml(Document.Editor.My.Computer.FileSystem.ReadAllText(filename));
				System.Text.StringBuilder sb = new System.Text.StringBuilder();
				System.IO.StringWriter sw = new System.IO.StringWriter(sb);
				System.Xml.XmlTextWriter xtw = null;
				try {
					xtw = new System.Xml.XmlTextWriter(sw);
					xtw.Formatting = System.Xml.Formatting.Indented;
					xd.WriteTo(xtw);
				} finally {
					if (xtw != null) {
						xtw.Close();
					}
				}
				string tex = sb.ToString();
				//Dim final As String
				//Try
				//    final = tex.Remove(tex.IndexOf("</FlowDocument>"), tex.Length)
				//Catch ex As Exception
				//End Try
				Document.Editor.My.Computer.FileSystem.WriteAllText(filename, tex, false);
			} else if (file.Extension.ToLower() == ".docx") {
				fs.Close();
				fs = null;
				FlowDocumenttoOpenXML converter = new FlowDocumenttoOpenXML();
				converter.Convert(Document, filename);
				converter.Close();
				//ElseIf file.Extension.ToLower = ".odt" Then
				//    fs.Close()
				//    fs = Nothing
				//    Dim converter As New FlowDocumenttoOpenDocument
				//    Dim opendoc As AODL.Document.TextDocuments.TextDocument = converter.Convert(Document)
				//    opendoc.SaveTo(filename)
			} else if (file.Extension.ToLower() == ".html" || file.Extension.ToLower() == ".htm") {
				fs.Close();
				fs = null;
				string s = System.Windows.Markup.XamlWriter.Save(Document);
				try {
					Document.Editor.My.Computer.FileSystem.WriteAllText(filename, HTMLConverter.HtmlFromXamlConverter.ConvertXamlToHtml(s), false);
				} catch (Exception ex) {
					MessageBoxDialog m = new MessageBoxDialog("Error saving document", "Error", null, null);
					m.MessageImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Common/error32.png"));
					m.Owner = Document.Editor.My.Windows.MainWindow;
					m.ShowDialog();
				}
			} else if (file.Extension.ToLower() == ".rtf") {
				tr.Save(fs, System.Windows.DataFormats.Rtf);
			} else {
				tr.Save(fs, System.Windows.DataFormats.Text);
			}
			if (fs != null) {
				fs.Close();
				fs = null;
			}
			//doctab.SetDocumentTitle(file.Name)
			DocumentName = filename;
			FileChanged = false;
		}

		#endregion

		private void DocumentEditor_LayoutUpdated(object sender, EventArgs e)
		{
			SetPageMargins(docpadding);
		}

		private void DocumentEditor_SelectionChanged(object sender, RoutedEventArgs e)
		{
			TextPointer ls = CaretPosition.GetLineStartPosition(0);
			TextPointer p = Document.ContentStart.GetLineStartPosition(0);
			int @int = 1;
			int int2 = 1;
			while (true) {
				if (ls.CompareTo(p) < 1) {
					break; // TODO: might not be correct. Was : Exit While
				}
				int r = 0;
				p = p.GetLineStartPosition(1, out r);
				if (r == 0) {
					break; // TODO: might not be correct. Was : Exit While
				}
				@int += 1;
			}
			TextPointer ls2 = Document.ContentStart.DocumentEnd.GetLineStartPosition(0);
			TextPointer p2 = Document.ContentEnd.DocumentStart.GetLineStartPosition(0);
			while (true) {
				if (ls2.CompareTo(p2) < 1) {
					break; // TODO: might not be correct. Was : Exit While
				}
				int r = 0;
				p2 = p2.GetLineStartPosition(1, out r);
				if (r == 0) {
					break; // TODO: might not be correct. Was : Exit While
				}
				int2 += 1;
			}
			SelectedLineNumber = @int;
			LineCount = int2;
			TextRange t = new TextRange(Document.ContentStart, Document.ContentEnd);
			TextPointer caretPos = CaretPosition;
			TextPointer poi = CaretPosition.GetLineStartPosition(0);
			int currentColumnNumber = Math.Max(p.GetOffsetToPosition(caretPos) - 1, 0) + 1;
			int currentColumnCount = currentColumnNumber;
			currentColumnCount += CaretPosition.GetTextRunLength(System.Windows.Documents.LogicalDirection.Forward);
			SelectedColumnNumber = currentColumnNumber;
			ColumnCount = currentColumnCount;
		}

		private void DocumentEditor_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			if (FileChanged == false) {
				FileChanged = true;
			}
			Semagsoft.HyperlinkHelper helper = new Semagsoft.HyperlinkHelper();
			helper.SubscribeToAllHyperlinks(Document);
			WordCount = GetWordCount();
		}

		#region "Find"

		public TextRange FindWordFromPosition(TextPointer position, string word)
		{
			while (position != null) {
				if (position.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text) {
					string textRun = position.GetTextInRun(LogicalDirection.Forward);
					// Find the starting index of any substring that matches "word".
					int indexInRun = textRun.IndexOf(word);
					if (indexInRun >= 0) {
						TextPointer start = position.GetPositionAtOffset(indexInRun);
						TextPointer end = start.GetPositionAtOffset(word.Length);
						return new TextRange(start, end);
					}
				}
				position = position.GetNextContextPosition(LogicalDirection.Forward);
			}
			// position will be null if "word" is not found.
			return null;
		}

		#endregion

		#region "Subscript/Superscript"

		public void ToggleSubscript()
		{
			var currentAlignment = this.Selection.GetPropertyValue(Inline.BaselineAlignmentProperty);
			BaselineAlignment newAlignment = ((BaselineAlignment)currentAlignment == BaselineAlignment.Subscript) ? BaselineAlignment.Baseline : BaselineAlignment.Subscript;
			this.Selection.ApplyPropertyValue(Inline.BaselineAlignmentProperty, newAlignment);
		}

		public void ToggleSuperscript()
		{
			var currentAlignment = this.Selection.GetPropertyValue(Inline.BaselineAlignmentProperty);
			BaselineAlignment newAlignment = ((BaselineAlignment)currentAlignment == BaselineAlignment.Superscript) ? BaselineAlignment.Baseline : BaselineAlignment.Superscript;
			this.Selection.ApplyPropertyValue(Inline.BaselineAlignmentProperty, newAlignment);
		}

		#endregion

		#region "Strikethrough"

		public void ToggleStrikethrough()
		{
			TextRange range = new TextRange(Selection.Start, Selection.End);
			TextDecorationCollection t = (TextDecorationCollection)Selection.GetPropertyValue(Inline.TextDecorationsProperty);
			if (t == null || !t.Equals(TextDecorations.Strikethrough)) {
				t = TextDecorations.Strikethrough;
			} else {
				t = new TextDecorationCollection();
			}
			range.ApplyPropertyValue(Inline.TextDecorationsProperty, t);
		}

		#endregion

		#region "GetWordCount"

		public int GetWordCount()
		{
			int SpacePos = 0;
			int X = 1;
			int WordCount = 0;
			bool NoMore = false;
			int CharValue = 0;
			TextRange tr = new TextRange(Document.ContentStart, Document.ContentEnd);
			string content = tr.Text;
			content = content.Replace(Constants.vbCr, " ");
			content = content.Replace(Constants.vbLf, " ");
			if (content.Trim().Length > 0) {
				while (NoMore == false) {
					SpacePos = Strings.InStr(X, Strings.Trim(content), " ");
					if (SpacePos > 0) {
						CharValue = Strings.Asc(content.Substring(X - 1, 1));
						if (CharValue > 64 && CharValue < 91 || CharValue > 96 && CharValue < 123 || CharValue > 47 && CharValue < 58) {
							WordCount += 1;
						}
						X = SpacePos + 1;
						while (Strings.InStr(X, (content.Substring(X - 1, 1)), " ") > 0) {
							X += 1;
						}
					} else {
						if (X <= content.Length) {
							CharValue = Strings.Asc(content.Substring(X - 1, 1));
							if (CharValue > 64 && CharValue < 91 || CharValue > 96 && CharValue < 123 || CharValue > 47 && CharValue < 58) {
								WordCount += 1;
							}
						}
						NoMore = true;
					}
				}
			}
			return WordCount;
		}

		#endregion

		#region "GoToLine"

		public void GoToLine(int linenumber)
		{
			if (linenumber == 1) {
				this.CaretPosition = Document.ContentStart.DocumentStart.GetLineStartPosition(0);
			} else {
				TextPointer ls = Document.ContentStart.DocumentStart.GetLineStartPosition(0);
				TextPointer p = Document.ContentStart.GetLineStartPosition(0);
				int @int = 2;
				while (true) {
					int r = 0;
					p = p.GetLineStartPosition(1, out r);
					if (r == 0) {
						this.CaretPosition = p;
						break; // TODO: might not be correct. Was : Exit While
					}
					if (linenumber == @int) {
						this.CaretPosition = p;
						break; // TODO: might not be correct. Was : Exit While
					}
					@int += 1;
				}
			}
		}

		#endregion

	}
}

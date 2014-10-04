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
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Cooperativeness.Documents.Editor
{
	public class OpenXMLtoFlowDocument
	{

		private WordprocessingDocument OpenXMLFile = null;
		public OpenXMLtoFlowDocument(string openxmlfilename)
		{
			if (Document.Editor.My.Computer.FileSystem.FileExists(openxmlfilename)) {
				DocumentFormat.OpenXml.Validation.OpenXmlValidator validator = new DocumentFormat.OpenXml.Validation.OpenXmlValidator();
				int count = 0;
				//For Each [error] As Validation.ValidationErrorInfo In validator.Validate(WordprocessingDocument.Open(openxmlfilename, True))
				//    count += 1
				//    MessageBox.Show([error].Description)
				//Next
				if (count == 0) {
					OpenXMLFile = WordprocessingDocument.Open(openxmlfilename, true);
				} else {
					MessageBoxDialog m = new MessageBoxDialog("OpenXML File is invalid!", "Error", null, null);
					m.MessageImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Common/error32.png"));
					m.Owner = Document.Editor.My.Windows.MainWindow;
					m.ShowDialog();
				}
			} else {
				MessageBoxDialog m = new MessageBoxDialog("File Not Found!", "Error", null, null);
				m.MessageImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/Common/error32.png"));
				m.Owner = Document.Editor.My.Windows.MainWindow;
				m.ShowDialog();
				throw new Exception();
			}
		}

		public void Close()
		{
			OpenXMLFile.Dispose();
		}

		public object Convert()
		{
			FlowDocument flowdoc = new FlowDocument();
			//For Each p As OpenXmlPart In OpenXMLFile.MainDocumentPart.
			//Next
			flowdoc.PageWidth = 816;
			flowdoc.PageHeight = 1056;
			DocumentFormat.OpenXml.Wordprocessing.Body body = OpenXMLFile.MainDocumentPart.Document.Body;
			foreach (DocumentFormat.OpenXml.OpenXmlElement oxmlobject in body.ChildElements) {
				DocumentFormat.OpenXml.Wordprocessing.Paragraph par = oxmlobject as DocumentFormat.OpenXml.Wordprocessing.Paragraph;
				if (par != null) {
					System.Windows.Documents.Paragraph p = new System.Windows.Documents.Paragraph();
					flowdoc.Blocks.Add(p);
					foreach (DocumentFormat.OpenXml.OpenXmlElement paroxmlobject in par.ChildElements) {
						DocumentFormat.OpenXml.Wordprocessing.Run ru = paroxmlobject as DocumentFormat.OpenXml.Wordprocessing.Run;
						if (ru != null) {
							System.Windows.Documents.Run r = new System.Windows.Documents.Run();
							r.Text = ru.InnerText;
							p.Inlines.Add(r);
						}
					}
					//flowdoc.ContentStart.InsertTextInRun(par.InnerText)
				}
			}
			return flowdoc;
		}
	}
}

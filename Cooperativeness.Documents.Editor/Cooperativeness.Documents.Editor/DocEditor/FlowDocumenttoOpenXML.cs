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
	public class FlowDocumenttoOpenXML
	{

		private FlowDocument flow = null;

		public FlowDocumenttoOpenXML()
		{
		}

		public void Close()
		{
			flow = null;
		}

		public object Convert(FlowDocument flowdoc, string fileloc)
		{
			using (WordprocessingDocument myDoc = WordprocessingDocument.Create(fileloc, WordprocessingDocumentType.Document)) {
				// Add a new main document part. 
				MainDocumentPart mainPart = myDoc.AddMainDocumentPart();
				//Create Document tree for simple document. 
				mainPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document();
				//Create Body (this element contains
				//other elements that we want to include 
				Body body = new Body();
				//Create paragraph
				foreach (System.Windows.Documents.Paragraph par in flowdoc.Blocks) {
					Paragraph paragraph = new Paragraph();
					Run run_paragraph = new Run();
					// we want to put that text into the output document
					TextRange t = new TextRange(par.ElementStart, par.ElementEnd);
					Text text_paragraph = new Text(t.Text);
					//Append elements appropriately.
					run_paragraph.Append(text_paragraph);
					paragraph.Append(run_paragraph);
					body.Append(paragraph);
				}
				mainPart.Document.Append(body);
				// Save changes to the main document part.
				mainPart.Document.Save();
			}
		}
	}
}

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
	public class OpenDocumenttoFlowDocument
	{


		public OpenDocumenttoFlowDocument()
		{
		}

		public FlowDocument Convert(string filename)
		{
			AODL.Document.TextDocuments.TextDocument opendocument = new AODL.Document.TextDocuments.TextDocument();
			opendocument.Load(filename);
			FlowDocument flowdocument = new FlowDocument();
			foreach (AODL.Document.Content.Text.Paragraph p in opendocument.Content) {
				Paragraph par = new Paragraph();
				foreach (AODL.Document.Content.Text.SimpleText t in p.TextContent) {
					//TODO: (20xx.xx) Add OpenDocument Format support
					par.ElementStart.InsertTextInRun(t.Text);
				}
				flowdocument.Blocks.Add(par);
			}
			return flowdocument;
		}
	}
}

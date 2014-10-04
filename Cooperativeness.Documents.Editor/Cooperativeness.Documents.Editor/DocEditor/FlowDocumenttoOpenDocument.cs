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
	public class FlowDocumenttoOpenDocument
	{


		public FlowDocumenttoOpenDocument()
		{
		}

		public AODL.Document.TextDocuments.TextDocument Convert(FlowDocument flowdoc)
		{
			AODL.Document.TextDocuments.TextDocument textdoc = new AODL.Document.TextDocuments.TextDocument();
			textdoc.New();
			foreach (Paragraph p in flowdoc.Blocks) {
				AODL.Document.Content.Text.Paragraph par = new AODL.Document.Content.Text.Paragraph(textdoc);
				TextRange t = new TextRange(p.ElementStart, p.ElementEnd);
				par.TextContent.Add(new AODL.Document.Content.Text.SimpleText(textdoc, t.Text));
			}
			return textdoc;
		}
	}
}

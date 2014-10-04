/*
 * Created by SharpDevelop.
 * User: darius.damalakas
 * Date: 2009.05.04
 * Time: 08:41
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using AODL.Document.Content.Tables;
using AODL.Document.Import.OpenDocument;
using AODL.Document.SpreadsheetDocuments;
using AODL.IO;
using NUnit.Framework;

namespace AODLTest.Document.Content.Tables
{
	[TestFixture]
	public class Test1
	{
		[Test]
		public void CellHasReferenceToRow()
		{
			string file = AARunMeFirstAndOnce.inPutFolder + @"bigtable.ods";
			SpreadsheetDocument textDocument = new SpreadsheetDocument();
            using (IPackageReader reader = new OnDiskPackageReader())
            {
                textDocument.Load(file, new OpenDocumentImporter(reader));
            }

			Table table = textDocument.Content[1] as Table;
			Row row = table.Rows[0];
			Cell cell = row.Cells[0];
			Assert.AreEqual(row, cell.Row);
			Assert.AreEqual(table, row.Table);
			
		}
	}
}

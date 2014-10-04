/*************************************************************************
 *
 * DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER
 * 
 * Copyright 2008 Sun Microsystems, Inc. All rights reserved.
 * 
 * Use is subject to license terms.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy
 * of the License at http://www.apache.org/licenses/LICENSE-2.0. You can also
 * obtain a copy of the License at http://odftoolkit.org/docs/license.txt
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * 
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 ************************************************************************/

using System;
using System.Xml.Linq;
using AODL.Document.Content.Charts;
using AODL.Document.Forms;
using AODL.Document.Forms.Controls;
using AODL.Document.Styles;

namespace AODL.Document.Content.Tables
{
    /// <summary>
    /// Table represent a table that is used within a spreadsheet document
    /// or a TextDocument!
    /// </summary>
    public class Table : IContent, IHtml
    {
        private ODFFormCollection _forms;
        private RowCollection _rows;

        /// <summary>
        /// Initializes a new instance of the <see cref="Table"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="node">The node.</param>
        public Table(IDocument document, XElement node)
        {
            Document = document;
            Node = node;

            Rows = new RowCollection();
            ColumnCollection = new ColumnCollection();
            Forms = new ODFFormCollection();
            _forms.Clearing += FormsCollection_Clear;
            _forms.Removed += FormCollection_Removed;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Table"/> class.
        /// </summary>
        /// <param name="document">The spreadsheet document.</param>
        /// <param name="name">The name.</param>
        /// <param name="styleName">Name of the style.</param>
        public Table(IDocument document, string name, string styleName)
        {
            Document = document;
            Node = new XElement(Ns.Table + "table");
            Node.SetAttributeValue(Ns.Table + "style-name", styleName);
            Node.SetAttributeValue(Ns.Table + "name", name);
            TableStyle = document.StyleFactory.Request<TableStyle>(styleName);

            Rows = new RowCollection();
            ColumnCollection = new ColumnCollection();
            Forms = new ODFFormCollection();
        }

        /// <summary>
        /// Gets or sets the row header.
        /// </summary>
        /// <value>The row header.</value>
        public RowHeader RowHeader { get; set; }

        /// <summary>
        /// Gets or sets the table style.
        /// </summary>
        /// <value>The table style.</value>
        public TableStyle TableStyle
        {
            get { return (TableStyle) Style; }
            set { Style = value; }
        }

        public ODFFormCollection Forms
        {
            get { return _forms; }
            set { _forms = value; }
        }

        /// <summary>
        /// Gets or sets the row collection.
        /// </summary>
        /// <value>The row collection.</value>
        public RowCollection Rows
        {
            get { return _rows; }
            set { _rows = value; }
        }

        /// <summary>
        /// Gets or sets the column collection.
        /// </summary>
        /// <value>The column collection.</value>
        public ColumnCollection ColumnCollection { get; set; }

        /// <summary>
        /// Gets or sets the name of the table.
        /// </summary>
        /// <value>The name of the table.</value>
        public string TableName
        {
            get { return (string) Node.Attribute(Ns.Table + "name"); }
            set { Node.SetAttributeValue(Ns.Table + "name", value); }
        }

        /// <summary>
        /// Create a new cell within this table which use the standard style.
        /// The cell isn't part of the table until you insert it
        /// via the InsertCellAt(int rowIndex, int columnIndex, Cell cell)
        /// </summary>
        /// <returns>The new cell</returns>
        public Cell CreateCell()
        {
            Cell cell = new Cell(Document);
            return cell;
        }

        /// <summary>
        /// Inserts the cell at the specified position.
        /// The RowCollection, the rows CellCollection and the ColumnCollection
        /// will be resized automatically.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="columnIndex">Index of the column.</param>
        /// <param name="cell">The cell.</param>
        public void InsertCellAt(int rowIndex, int columnIndex, Cell cell)
        {
            while (_rows.Count <= rowIndex)
            {
                _rows.Add(new Row(this, String.Format("row{0}", _rows.Count)));
            }

            Row row = _rows[rowIndex];
            row.InsertCellAt(columnIndex, cell);
            cell.Row = row;
        }

        /// <summary>
        /// Builds the node.
        /// </summary>
        /// <returns></returns>
        public XElement BuildNode()
        {
            if (Forms.Count != 0)
            {
                XElement nodeForms = Node.Element(Ns.Office + "forms");
                if (nodeForms == null)
                {
                    nodeForms = new XElement(Ns.Office + "forms");
                }

                foreach (ODFForm f in Forms)
                {
                    nodeForms.Add(f.Node);
                }
                Node.Add(nodeForms);
            }


            foreach (Column column in ColumnCollection)
                Node.Add(column.Node);

            if (RowHeader != null)
                Node.Add(RowHeader.Node);

            foreach (Row row in Rows)
            {
                //check for nested tables
                foreach (Cell cell in row.Cells)
                {
                    foreach (IContent iContent in cell.Content)
                    {
                        if (iContent is Table)
                        {
                            Table table = iContent as Table;
                            table.BuildNode();
                        }
                    }
                }
                //now, append the row node
                Node.Add(row.Node);
            }

            return Node;
        }

        /// <summary>
        /// Resets the table node.
        /// </summary>
        public void Reset()
        {
            string name = TableName;
            string styleName = StyleName;

            Node.RemoveAll();
            Node = new XElement(Ns.Table + "table");
            Node.SetAttributeValue(Ns.Table + "style-name", styleName);
            Node.SetAttributeValue(Ns.Table + "name", name);
        }

        /// <summary>
        /// Looks for a specific control through all the forms by its ID
        /// </summary>
        /// <param name="id">Control ID</param>
        /// <returns>The control</returns>
        public ODFFormControl FindControlById(string id)
        {
            foreach (ODFForm f in Forms)
            {
                ODFFormControl fc = f.FindControlById(id, true);
                if (fc != null)
                    return fc;
            }
            return null;
        }

        /// <summary>
        /// Looks for a specific control through all the forms by its name
        /// </summary>
        /// <param name="name">Control name</param>
        /// <returns>The control</returns>
        public ODFFormControl FindControlByName(string name)
        {
            foreach (ODFForm f in Forms)
            {
                ODFFormControl fc = f.FindControlByName(name, true);
                if (fc != null)
                    return fc;
            }
            return null;
        }

        /// <summary>
        /// Adds new form to the forms collection
        /// </summary>
        /// <param name="name">Form name</param>
        /// <returns></returns>
        public ODFForm AddNewForm(string name)
        {
            ODFForm f = new ODFForm(Document, name);
            Forms.Add(f);
            return f;
        }

        private static void FormCollection_Removed(int index, object value)
        {
            ODFForm child = (value as ODFForm);
            if (child != null)
            {
                child.Controls.Clear();
            }
        }

        private void FormsCollection_Clear()
        {
            for (int i = 0; i < _forms.Count; i++)
            {
                ODFForm f = _forms[i];
                f.Controls.Clear();
            }
        }

        public void InsertChartAt(string cellName, Chart chart)
        {
            string endCell = chart.EndCellAddress;

            if (endCell == null)
            {
                int curRowIndex = chart.GetCellPos(cellName, this).rowIndex;
                int curColIndex = chart.GetCellPos(cellName, this).columnIndex;
                int endRowIndex = curRowIndex + 15;
                int endColIndex = curColIndex + 5;

                string endCellRow = endRowIndex.ToString();
                string endCellCol = null;

                if (endColIndex <= 26)
                {
                    char col = (char) (endColIndex + 'A' - 1);
                    endCellCol = col.ToString();
                }

                else if (curColIndex > 26 && curColIndex <= 260)
                {
                    int firstCha = curColIndex/26;
                    char firstCharacter = (char) (firstCha + 'A' - 1);
                    int secondCha = curColIndex%2;
                    char secondChatacter = (char) (secondCha + 'A' - 1);
                    endCellCol = firstCharacter + secondChatacter.ToString();
                }

                endCell = TableName + "." + endCellCol + endCellRow;
                chart.EndCellAddress = endCell;
            }

            Cell cell = (chart.GetCellPos(cellName, this)).cell;
            cell.Content.Add(chart.Frame);
        }

        #region IContent Member

        private IStyle _style;

        /// <summary>
        /// Gets or sets the node.
        /// </summary>
        /// <value>The node.</value>
        public XElement Node { get; set; }

        /// <summary>
        /// Gets or sets the name of the style.
        /// </summary>
        /// <value>The name of the style.</value>
        public string StyleName
        {
            get { return (string) Node.Attribute(Ns.Table + "style-name"); }
            set { Node.SetAttributeValue(Ns.Table + "style-name", value); }
        }

        /// <summary>
        /// Every object (typeof(IContent)) have to know his document.
        /// </summary>
        /// <value></value>
        public IDocument Document { get; set; }

        /// <summary>
        /// A Style class wich is referenced with the content object.
        /// If no style is available this is null.
        /// </summary>
        /// <value></value>
        public IStyle Style
        {
            get { return _style; }
            set
            {
                StyleName = value.StyleName;
                _style = value;
            }
        }

        /// <summary>
        /// Gets or sets the node.
        /// </summary>
        /// <value>The node.</value>
        XNode IContent.Node
        {
            get { return Node; }
            set { Node = (XElement) value; }
        }

        #endregion

        #region IHtml Member

        /// <summary>
        /// Return the content as Html string
        /// </summary>
        /// <returns>The html string</returns>
        public string GetHtml()
        {
            bool isRightAlign = false;
            const string outerHtml =
                "<table border=0 cellspacing=0 cellpadding=0 border=0 style=\"width: 16.55cm; \">\n\n<tr>\n<td align=right>\n";
            string html =
                "<table hspace=\"14\" vspace=\"14\" cellpadding=\"2\" cellspacing=\"2\" border=\"0\" bgcolor=\"#000000\" ";
            const string htmlRight = "<table cellpadding=\"2\" cellspacing=\"2\" border=\"0\" bgcolor=\"#000000\" ";

            if (TableStyle.TableProperties != null)
            {
                if (TableStyle.TableProperties.Align != null)
                {
                    string align = TableStyle.TableProperties.Align.ToLower();
                    if (align == "right")
                    {
                        isRightAlign = true;
                        html = htmlRight;
                    }
                    else if (align == "margin")
                        align = "left";
                    html += " align=\"" + align + "\" ";
                }
                html += TableStyle.TableProperties.GetHtmlStyle();
            }

            html += ">\n";

            if (RowHeader != null)
                html += RowHeader.GetHtml();

            foreach (Row	row in Rows)
                html += row.GetHtml() + "\n";

            html += "</table>\n";

            //Wrapp a right align table with outer table,
            //because following content will be right to
            //the table!
            if (isRightAlign)
                html = outerHtml + html + "\n</td>\n</tr>\n</table>\n";

            return html;
        }

        #endregion
    }
}

/*
 * $Log: Table.cs,v $
 * Revision 1.7  2008/04/29 15:39:46  mt
 * new copyright header
 *
 * Revision 1.6  2008/04/10 17:33:15  larsbehr
 * - Added several bug fixes mainly for the table handling which are submitted by Phil  Jollans
 *
 * Revision 1.5  2008/02/08 07:12:19  larsbehr
 * - added initial chart support
 * - several bug fixes
 *
 * Revision 1.4  2007/07/15 09:29:55  yegorov
 * Issue number:
 * Submitted by:
 * Reviewed by:
 *
 * Revision 1.3  2007/06/20 17:37:18  yegorov
 * Issue number:
 * Submitted by:
 * Reviewed by:
 *
 * Revision 1.1  2007/02/25 08:58:37  larsbehr
 * initial checkin, import from Sourceforge.net to OpenOffice.org
 *
 * Revision 1.5  2007/02/04 22:52:57  larsbm
 * - fixed bug in resize algorithm for rows and cells
 * - extending IDocument, overload Save to accept external exporter impl.
 * - initial version of AODL PDF exporter add on
 *
 * Revision 1.4  2006/02/08 16:37:36  larsbm
 * - nested table test
 * - AODC spreadsheet
 *
 * Revision 1.3  2006/02/05 20:02:25  larsbm
 * - Fixed several bugs
 * - clean up some messy code
 *
 * Revision 1.2  2006/01/29 18:52:14  larsbm
 * - Added support for common styles (style templates in OpenOffice)
 * - Draw TextBox import and export
 * - DrawTextBox html export
 *
 * Revision 1.1  2006/01/29 11:28:22  larsbm
 * - Changes for the new version. 1.2. see next changelog for details
 *
 */
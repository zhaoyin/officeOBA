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
using AODL.Document.Styles;

namespace AODL.Document.Content.Tables
{
    /// <summary>
    /// Cell represent a table cell.
    /// </summary>
    public class Cell : IContent, IContentContainer, IHtml
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Cell"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="node">The node.</param>
        public Cell(IDocument document, XElement node)
        {
            Document = document;
            Node = node;
            InitStandards();
        }

        public Cell(IDocument document) : this(document, new XElement(Ns.Table + "table-cell"))
        {
        }

        public Cell(IDocument document, string styleName, string officeValueTyp)
            : this(document, styleName)
        {
            if (officeValueTyp != null)
                OfficeValue = officeValueTyp;
        }

        public Cell(IDocument document, string styleName)
            : this(document)
        {
            CellStyle = Document.StyleFactory.Request<CellStyle>(styleName);
            /*if (styleName != null)
			{
				this.StyleName		= styleName;
				this.CellStyle		= new CellStyle(this.Document, styleName);
				this.Document.Styles.Add(this.CellStyle);
			}*/
        }

        /// <summary>
        /// Gets or sets the cell style.
        /// </summary>
        /// <value>The cell style.</value>
        public CellStyle CellStyle
        {
            get { return (CellStyle) Style; }
            set
            {
                StyleName = value.StyleName;
                Style = value;
            }
        }

        /// <summary>
        /// Use this to merge cells, the count of ColumnRepeating,
        /// need the same count of following CellSpan objects e.g. "2"
        /// </summary>
        /// <value>Count of Columns to be repeated</value>
        public string ColumnRepeating
        {
            get { return (string) Node.Attribute(Ns.Table + "number-columns-spanned"); }
            set { Node.SetAttributeValue(Ns.Table + "number-columns-spanned", value); }
        }

        /// <summary>
        /// Gets or sets the row.
        /// </summary>
        /// <value>The row.</value>
        [Obsolete]
        public Table Table { get; set; }

        /// <summary>
        /// Gets or sets the row.
        /// </summary>
        /// <value>The row.</value>
        public Row Row { get; set; }

        /// <summary>
        /// Gets or sets the type of the office value.
        /// See class OfficeValueTypes for possible
        /// settings.
        /// </summary>
        /// <value>The type of the office value.</value>
        public string OfficeValueType
        {
            get { return (string) Node.Attribute(Ns.Office + "value-type"); }
            set { Node.SetAttributeValue(Ns.Office + "value-type", value); }
        }

        /// <summary>
        /// Gets or sets the office value.
        /// </summary>
        /// <value>The office value.</value>
        public string OfficeValue
        {
            get { return (string) Node.Attribute(Ns.Office + "value"); }
            set { Node.SetAttributeValue(Ns.Office + "value", value); }
        }

        /// <summary>
        /// Gets or sets the office date value.
        /// </summary>
        /// <value>The office date value.</value>
        public DateTime? OfficeDateValue
        {
            get { return (DateTime?)Node.Attribute(Ns.Office + "date-value"); }
            set { Node.SetAttributeValue(Ns.Office + "date-value", value); }
        }

        /// <summary>
        /// Gets or set a formula for this cell.
        /// </summary>
        /// <value>The formula.</value>
        public string Formula
        {
            get { return (string) Node.Attribute(Ns.Table + "formula"); }
            set { Node.SetAttributeValue(Ns.Table + "formula", value); }
        }

        /// <summary>
        /// Inits the standards.
        /// </summary>
        private void InitStandards()
        {
            Content = new ContentCollection();
            Content.Inserted += Content_Inserted;
            Content.Removed += Content_Removed;
        }

        /// <summary>
        /// Content_s the inserted.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        private void Content_Inserted(int index, object value)
        {
            Node.Add(((IContent) value).Node);
        }

        /// <summary>
        /// Content_s the removed.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        private static void Content_Removed(int index, object value)
        {
            ((IContent) value).Node.Remove();
        }

        #region IHtml Member

        /// <summary>
        /// Return the content as Html string
        /// </summary>
        /// <returns>The html string</returns>
        public string GetHtml()
        {
            string html = "<td ";

            if (ColumnRepeating != null)
                html += "colspan=" + ColumnRepeating + " ";

            if (((CellStyle) Style).CellProperties != null)
                html += ((CellStyle) Style).CellProperties.GetHtmlStyle();

            string htmlwidth = GetHtmlWidth();
            if (htmlwidth != null)
                if (html.IndexOf("style=") == -1)
                    html += "style=\"" + htmlwidth + "\"";
                else
                    html = html.Substring(0, html.Length - 1) + htmlwidth + "\"";

            html += ">\n";

            foreach (IContent content in Content)
                if (content is IHtml)
                    html += ((IHtml) content).GetHtml();

            if (Content != null)
                if (Content.Count == 0)
                    html += "&nbsp;";

            html += "\n</td>\n";

            return html;
        }

        /// <summary>
        /// Gets the width of the HTML.
        /// </summary>
        /// <returns></returns>
        private string GetHtmlWidth()
        {
            int index = 0;
            foreach (Cell cell in Row.Cells)
            {
                if (cell == this)
                {
                    if (Row.Table.ColumnCollection != null)
                        if (index <= Row.Table.ColumnCollection.Count)
                        {
                            Column column = Row.Table.ColumnCollection[index];
                            if (column != null)
                                if (column.ColumnStyle.ColumnProperties.Width != null)
                                    return " width: " + column.ColumnStyle.ColumnProperties.Width.Replace(",", ".") +
                                           "; ";
                        }
                }
                index++;
            }
            return string.Empty;
        }

        #endregion

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

        #region IContentContainer Member

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        public ContentCollection Content { get; set; }

        #endregion
    }
}

/*
 * $Log: Cell.cs,v $
 * Revision 1.2  2008/04/29 15:39:45  mt
 * new copyright header
 *
 * Revision 1.1  2007/02/25 08:58:35  larsbehr
 * initial checkin, import from Sourceforge.net to OpenOffice.org
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
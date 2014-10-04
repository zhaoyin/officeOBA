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

using System.Xml.Linq;
using AODL.Document.SpreadsheetDocuments;
using AODL.Document.Styles;

namespace AODL.Document.Content.Tables
{
    /// <summary>
    /// Column represent a table column.
    /// </summary>
    public class Column : IContent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Column"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="node">The node.</param>
        public Column(IDocument document, XElement node)
        {
            Document = document;
            Node = node;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Column"/> class.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="styleName">Name of the style.</param>
        public Column(Table table, string styleName)
        {
            Table = table;
            Document = table.Document;
            Node = new XElement(Ns.Table + "table-column");

            XAttribute xa = new XAttribute(Ns.Table + "style-name", styleName);
            Node.Add(xa);

            if (Document is SpreadsheetDocument)
                ParentCellStyleName = "Default";
            ColumnStyle = Document.StyleFactory.Request<ColumnStyle>(styleName);
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

        /// <summary>
        /// Gets or sets the name of the parent cell style.
        /// </summary>
        /// <value>The name of the parent cell style.</value>
        public string ParentCellStyleName
        {
            get { return (string) Node.Attribute(Ns.Table + "default-cell-style-name"); }
            set { Node.SetAttributeValue(Ns.Table + "default-cell-style-name", value); }
        }

        /// <summary>
        /// Gets or sets the number columns repeated.
        /// </summary>
        /// <value>The number columns repeated.</value>
        public string NumberColumnsRepeated
        {
            get { return (string) Node.Attribute(Ns.Table + "number-columns-repeated"); }
            set { Node.SetAttributeValue(Ns.Table + "number-columns-repeated", value); }
        }

        /// <summary>
        /// Gets or sets the node.
        /// </summary>
        /// <value>The node.</value>
        public Table Table { get; set; }

        /// <summary>
        /// Gets or sets the column style.
        /// </summary>
        /// <value>The column style.</value>
        public ColumnStyle ColumnStyle
        {
            get { return (ColumnStyle) Style; }
            set
            {
                StyleName = value.StyleName;
                Style = value;
            }
        }
    }
}

/*
 * $Log: Column.cs,v $
 * Revision 1.2  2008/04/29 15:39:45  mt
 * new copyright header
 *
 * Revision 1.1  2007/02/25 08:58:36  larsbehr
 * initial checkin, import from Sourceforge.net to OpenOffice.org
 *
 * Revision 1.4  2007/02/13 17:58:47  larsbm
 * - add first part of implementation of master style pages
 * - pdf exporter conversations for tables and images and added measurement helper
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
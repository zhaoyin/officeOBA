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

using System.Text.RegularExpressions;
using System.Xml.Linq;
using AODL.Document.Styles;

namespace AODL.Document.Content.Tables
{
    /// <summary>
    /// RowHeader represent a table row header.
    /// </summary>
    public class RowHeader : IContent, IHtml
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RowHeader"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="node">The node.</param>
        public RowHeader(IDocument document, XElement node)
        {
            Document = document;
            Node = node;
            InitStandards();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RowHeader"/> class.
        /// </summary>
        /// <param name="table">The table.</param>
        public RowHeader(Table table)
        {
            Table = table;
            Document = table.Document;
            InitStandards();
//			this.RowCollection		= new RowCollection();
            Node = new XElement(Ns.Table + "table-header-rows");
        }

        /// <summary>
        /// Gets or sets the table.
        /// </summary>
        /// <value>The table.</value>
        public Table Table { get; set; }

        /// <summary>
        /// Gets or sets the row collection.
        /// </summary>
        /// <value>The row collection.</value>
        public RowCollection RowCollection { get; set; }

        /// <summary>
        /// Inits the standards.
        /// </summary>
        private void InitStandards()
        {
            RowCollection = new RowCollection();

            RowCollection.Inserted += RowCollection_Inserted;
            RowCollection.Removed += RowCollection_Removed;
        }

        /// <summary>
        /// Rows the collection_ inserted.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        private void RowCollection_Inserted(int index, object value)
        {
            Node.Add(((Row) value).Node);
        }

        /// <summary>
        /// Rows the collection_ removed.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        private static void RowCollection_Removed(int index, object value)
        {
            ((Row) value).Node.Remove();
        }

        #region IHtml Member

        /// <summary>
        /// Return the content as Html string
        /// </summary>
        /// <returns>The html string</returns>
        public string GetHtml()
        {
            string html = "";

            foreach (Row	row in RowCollection)
                html += row.GetHtml() + "\n";

            return HtmlCleaner(html);
        }

        /// <summary>
        /// Table row header cleaner, this is needed,
        /// because in OD, the style of the table header
        /// row is used for to and bottom margin, but
        /// some brother use this from the text inside
        /// the cells. Which result in to large height
        /// settings.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>The cleaned text</returns>
        private static string HtmlCleaner(string text)
        {
            const string pat = @"margin-top: \d\.\d\d\w\w;";
            const string pat1 = @"margin-bottom: \d\.\d\d\w\w;";
            Regex r = new Regex(pat, RegexOptions.IgnoreCase);
            text = r.Replace(text, "");
            r = new Regex(pat1, RegexOptions.IgnoreCase);
            text = r.Replace(text, "");
            return text;
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
    }
}

/*
 * $Log: RowHeader.cs,v $
 * Revision 1.2  2008/04/29 15:39:46  mt
 * new copyright header
 *
 * Revision 1.1  2007/02/25 08:58:37  larsbehr
 * initial checkin, import from Sourceforge.net to OpenOffice.org
 *
 * Revision 1.2  2006/02/05 20:02:25  larsbm
 * - Fixed several bugs
 * - clean up some messy code
 *
 * Revision 1.1  2006/01/29 11:28:22  larsbm
 * - Changes for the new version. 1.2. see next changelog for details
 *
 * Revision 1.1  2005/12/12 19:39:17  larsbm
 * - Added Paragraph Header
 * - Added Table Row Header
 * - Fixed some bugs
 * - better whitespace handling
 * - Implmemenation of HTML Exporter
 *
 */
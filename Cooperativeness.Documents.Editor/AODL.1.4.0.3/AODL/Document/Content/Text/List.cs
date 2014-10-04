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
using AODL.Document.Styles;

namespace AODL.Document.Content.Text
{
    /// <summary>
    /// Represent a list which could be a numbered or bullet style list.
    /// </summary>
    public class List : IContent, IContentContainer, IHtml
    {
        private readonly ListStyles _type;

        /// <summary>
        /// Initializes a new instance of the <see cref="List"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="node">The node.</param>
        public List(IDocument document, XElement node)
        {
            Document = document;
            Node = node;
            InitStandards();
        }

        /// <summary>
        /// Create a new List object
        /// </summary>
        /// <param name="document">The IDocument</param>
        /// <param name="styleName">The style name</param>
        /// <param name="typ">The list typ bullet, ..</param>
        /// <param name="paragraphStyleName">The style name for the ParagraphStyle.</param>
        public List(IDocument document, string styleName, ListStyles typ, string paragraphStyleName)
        {
            Document = document;
            Node = new XElement(Ns.Text + "list");
            InitStandards();

            Style = new ListStyle(Document, styleName);
            ParagraphStyle = new ParagraphStyle(Document, paragraphStyleName);
            Document.Styles.Add(Style);
            Document.Styles.Add(ParagraphStyle);

            ParagraphStyle.ListStyleName = styleName;
            _type = typ;

            ((ListStyle) Style).AutomaticAddListLevelStyles(typ);
        }

        /// <summary>
        /// Create a new List which is used to represent a inner list.
        /// </summary>
        /// <param name="document">The IDocument</param>
        /// <param name="outerlist">The List to which this List belongs.</param>
        public List(IDocument document, List outerlist)
        {
            Document = document;
            ParagraphStyle = outerlist.ParagraphStyle;
            InitStandards();
            _type = outerlist.ListType;
            //Create an inner list node, don't need a style
            //use the parents list style
            Node = new XElement(Ns.Text + "list");
        }

        /// <summary>
        /// The ParagraphStyle to which this List belongs.
        /// There is only one ParagraphStyle per List and
        /// its ListItems.
        /// </summary>
        public ParagraphStyle ParagraphStyle { get; set; }

        /// <summary>
        /// Gets or sets the list style.
        /// </summary>
        /// <value>The list style.</value>
        public ListStyle ListStyle
        {
            get { return (ListStyle) Style; }
            set { Style = value; }
        }

        /// <summary>
        /// Gets the type of the list.
        /// </summary>
        /// <value>The type of the list.</value>
        public ListStyles ListType
        {
            get { return _type; }
        }

        #region IContentContainer Members

        /// <summary>
        /// The ContentCollection of access
        /// to their list items.
        /// </summary>
        public ContentCollection Content { get; set; }

        #endregion

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
            string html = null;

            if (ListType == ListStyles.Bullet)
                html = "<ul>\n";
            else if (ListType == ListStyles.Number)
                html = "<ol>\n";

            foreach (IContent content in Content)
                if (content is IHtml)
                    html += ((IHtml) content).GetHtml();

            if (ListType == ListStyles.Bullet)
                html += "</ul>\n";
            else if (ListType == ListStyles.Number)
                html += "</ol>\n";
            //html				+= "</ul>\n";

            return html;
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
            get { return (string) Node.Attribute(Ns.Text + "style-name"); }
            set { Node.SetAttributeValue(Ns.Text + "style-name", value); }
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